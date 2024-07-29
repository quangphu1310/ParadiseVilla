using Newtonsoft.Json.Linq;
using ParadiseVilla_Utility;
using ParadiseVilla_Web.Models;
using ParadiseVilla_Web.Models.DTO;
using ParadiseVilla_Web.Services.IServices;

namespace ParadiseVilla_Web.Services
{
    public class VillaNumberService : BaseService, IVillaNumberService
    {
        private IHttpClientFactory _httpClientFactory;
        private string villaUrl;
        public VillaNumberService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            _httpClientFactory = httpClient;
            villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
        }

        public async Task<T> CreateAsync<T>(VillaNumberCreateDTO obj, string token)
        {
            return await SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = obj,
                Url = villaUrl + $"/api/{SD.CurrentAPIVersion}/VillaNumberAPI",
                Token = token
            });
        }

        public async Task<T> DeleteAsync<T>(int id, string token)
        {
            return await SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.DELETE,
                Url = villaUrl + $"/api/{SD.CurrentAPIVersion}/VillaNumberAPI/" + id,
                Token = token
            });
        }

        public async Task<T> GetAllAsync<T>(string token)
        {
            return await SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = villaUrl + $"/api/{SD.CurrentAPIVersion}/VillaNumberAPI/",
                Token = token
            });
        }

        public async Task<T> GetAsync<T>(int id, string token)
        {
            return await SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = villaUrl + $"/api/{SD.CurrentAPIVersion}/VillaNumberAPI/" + id,
                Token = token
            }) ;
        }

        public async Task<T> UpdateAsync<T>(VillaNumberUpdateDTO obj, string token)
        {
            return await SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.PUT,
                Url = villaUrl + "/api/{SD.CurrentAPIVersion}/VillaNumberAPI/" + obj.VillaNo,
                Data = obj,
                Token = token
            }) ;
        }
    }
}
