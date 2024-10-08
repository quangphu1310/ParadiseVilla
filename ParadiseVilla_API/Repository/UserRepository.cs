﻿using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using ParadiseVilla_API.Data;
using ParadiseVilla_API.Models;
using ParadiseVilla_API.Models.DTO;
using ParadiseVilla_API.Repository.IRepository;
using ParadiseVilla_Utility;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace ParadiseVilla_API.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly string secretKey;
        public UserRepository(ApplicationDbContext db, IConfiguration configuration,
            UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }
        public bool IsUniqueUser(string username)
        {
            if (_db.ApplicationUsers.FirstOrDefault(x => x.UserName == username) != null) 
                return false;
            return true;
        }

        public async Task<TokenDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(x => x.UserName.ToLower() == loginRequestDTO.UserName.ToLower());
            
            var isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
            if (user == null || !isValid)
            {
                return new TokenDTO()
                {
                    AccessToken = ""
                };
            }
            //if user was found generate JWT Token
            var jwtTokenId = $"JTI{Guid.NewGuid()}";
            var accessToken = await GetAccessToken(user, jwtTokenId);
            var refreshToken = await CreateNewRefreshToken(user.Id, jwtTokenId);

            TokenDTO tokenDTO = new TokenDTO()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };
            return tokenDTO;
        }

        public async Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO)
        {
            ApplicationUser user = new()
            {
                UserName = registerationRequestDTO.UserName,
                Name = registerationRequestDTO.Name,
                Email = registerationRequestDTO.UserName,
                NormalizedEmail = registerationRequestDTO.UserName.ToUpper()
            };
            try
            {
                var result = await _userManager.CreateAsync(user, registerationRequestDTO.Password);
                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync(registerationRequestDTO.Role).GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole(registerationRequestDTO.Role));
                    }
                    await _userManager.AddToRoleAsync(user, registerationRequestDTO.Role);
                    var userToReturn = _db.ApplicationUsers
                        .FirstOrDefault(x => x.UserName == registerationRequestDTO.UserName);
                    return _mapper.Map<UserDTO>(userToReturn);
                }
                else
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                }
            }catch (Exception ex)
            {

            }
            return new UserDTO();
        }

        public async Task<string> GetAccessToken(ApplicationUser user, string jwtTokenId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var roles = await _userManager.GetRolesAsync(user);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                    new Claim(JwtRegisteredClaimNames.Jti, jwtTokenId),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id)
                }),
                Expires = DateTime.UtcNow.AddMinutes(2),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<TokenDTO> RefreshAccessToken(TokenDTO tokenDTO)
        {
            // Find an existing refresh token
            var existingRefreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(x=>x.Refresh_Token == tokenDTO.RefreshToken);
            if (existingRefreshToken == null) {
                existingRefreshToken.IsValid = false;
                return new TokenDTO();
            }

            // Compare data from existing refresh and access token provided and if there is any missmatch then consider it as a fraud
            var result = GetAccessTokenData(tokenDTO.AccessToken);
            if(!result.isSuccess || result.jwtTokenId != existingRefreshToken.JwtTokenId || 
                result.userId != existingRefreshToken.UserId)
            {
                existingRefreshToken.IsValid = false;
                _db.SaveChanges();
                return new TokenDTO();
            }
            // When someone tries to use not valid refresh token, fraud possible
            if (!existingRefreshToken.IsValid) {
                await _db.RefreshTokens.Where(x => x.UserId == existingRefreshToken.UserId &&
                x.JwtTokenId == existingRefreshToken.JwtTokenId).ExecuteUpdateAsync(x=>x.SetProperty(u=>u.IsValid, false));

                return new TokenDTO();
            }
            // If just expired then mark as invalid and return empty
            if(existingRefreshToken.ExpiresAt < DateTime.UtcNow) {
                existingRefreshToken.IsValid = false;
                _db.SaveChanges();
                return new TokenDTO();
            }
            // replace old refresh with a new one with updated expire date
            var newRefreshToken = await CreateNewRefreshToken(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
            // revoke existing refresh token
            existingRefreshToken.IsValid = false;
            _db.SaveChanges();
            // generate new access token
            var applicationUser = await _db.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == existingRefreshToken.UserId);
            if (applicationUser == null) { 
                return new TokenDTO();
            }
            var newAccessToken = await GetAccessToken(applicationUser, existingRefreshToken.JwtTokenId);
            return new TokenDTO
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };

        }
        private async Task<string> CreateNewRefreshToken(string userId, string jwtTokenId)
        {
            RefreshToken refreshToken = new()
            {
                JwtTokenId = jwtTokenId,
                UserId = userId,
                IsValid = true,
                ExpiresAt = DateTime.UtcNow.AddMinutes(2),
                Refresh_Token = Guid.NewGuid() + "-" + Guid.NewGuid(),
            };
            await _db.RefreshTokens.AddAsync(refreshToken);
            await _db.SaveChangesAsync();
            return refreshToken.Refresh_Token;
        }

        private (bool isSuccess, string userId, string jwtTokenId) GetAccessTokenData(string accessToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwt = tokenHandler.ReadJwtToken(accessToken);

                var jwtTokenId = jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Jti).Value;
                var userId = jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value;

                return (true, userId, jwtTokenId); 
            }
            catch
            {
                return (false, null, null);
            }
        }
    }
    
}
