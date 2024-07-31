using ParadiseVilla_Web.Models;

namespace ParadiseVilla_Web.Services.IServices
{
    public interface IBaseService
    {
        APIResponse reponseModel { get; set; }
        Task<T> SendAsync<T>(APIRequest apiRequest, bool withBearer = true);
    }
}
