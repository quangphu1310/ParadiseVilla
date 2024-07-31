using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ParadiseVilla_Utility;
using ParadiseVilla_Web.Models;
using ParadiseVilla_Web.Models.DTO;
using ParadiseVilla_Web.Services.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ParadiseVilla_Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IAuthService _authService;
        private IMapper _mapper;
        public UserController(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }
        public ActionResult Login() 
        {
            LoginRequestDTO obj = new();
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDTO obj)
        {
            APIResponse response = await _authService.LoginAsync<APIResponse>(obj);
            if(response != null && response.IsSuccess)
            {
                TokenDTO login = JsonConvert.DeserializeObject<TokenDTO>(Convert.ToString(response.Result));
            //Lấy giá trị từ sub trong token
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(login.Token);

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(x=>x.Type== "unique_name").Value));
                identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(x=>x.Type=="role").Value));
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                HttpContext.Session.SetString(SD.AccessToken, login.Token);
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("CustomError", response.Errors.FirstOrDefault());
            return View(obj);
            
        }
        public ActionResult Register()
        {
            var RoleList = new List<SelectListItem>()
            {
                new SelectListItem{Text = SD.Role_Admin, Value = SD.Role_Admin},
                new SelectListItem{Text=SD.Role_Customer, Value = SD.Role_Customer }
            };
            ViewBag.RoleList = RoleList;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterationRequestDTO obj)
        {
            if (string.IsNullOrEmpty(obj.Role))
            {
                obj.Role = SD.Role_Customer;
            }
            var response = await _authService.RegisterAsync<APIResponse>(obj);
            if(response != null && response.IsSuccess) 
            {
                return RedirectToAction("Login");
            }
            var RoleList = new List<SelectListItem>()
            {
                new SelectListItem{Text = SD.Role_Admin, Value = SD.Role_Admin},
                new SelectListItem{Text=SD.Role_Customer, Value = SD.Role_Customer }
            };
            ViewBag.RoleList = RoleList;
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            //HttpContext.Session.SetString(SD.AccessToken, "");
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
