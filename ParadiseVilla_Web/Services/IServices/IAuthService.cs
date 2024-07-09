using ParadiseVilla_Web.Models.DTO;

namespace ParadiseVilla_Web.Services.IServices
{
    public interface IAuthService
    {
        Task<T> LoginAsync<T>(LoginRequestDTO loginRequest);
        Task<T> RegisterAsync<T>(RegisterationRequestDTO registeration);
    }
}
