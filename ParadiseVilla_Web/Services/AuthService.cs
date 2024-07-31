using Microsoft.AspNetCore.Identity.Data;
using ParadiseVilla_Utility;
using ParadiseVilla_Web.Models;
using ParadiseVilla_Web.Models.DTO;
using ParadiseVilla_Web.Services.IServices;

namespace ParadiseVilla_Web.Services
{
    public class AuthService : IAuthService
    {
        private IHttpClientFactory _httpClientFactory;
        private string villaUrl;
        private readonly IBaseService _baseService;

        public AuthService(IHttpClientFactory httpClient, IConfiguration configuration, IBaseService baseService)
        {
            _httpClientFactory = httpClient;
            villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
            _baseService = baseService;
        }

        public async Task<T> LoginAsync<T>(LoginRequestDTO loginRequest)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = loginRequest,
                Url = villaUrl + $"/api/{SD.CurrentAPIVersion}/UserAuth/login"
            });
        }

        public async Task<T> RegisterAsync<T>(RegisterationRequestDTO registeration)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = registeration,
                Url = villaUrl + $"/api/{SD.CurrentAPIVersion}/UserAuth/register"
            });
        }
    }
}
