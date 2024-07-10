using ParadiseVilla_Web.Models.DTO;

namespace ParadiseVilla_Web.Services.IServices
{
    public interface IVillaService
    {
        Task<T> GetAllAsync<T>(string token);
        Task<T> GetAsync<T>(int id, string token);
        Task<T> CreateAsync<T>(VillaCreateDTO obj, string token);
        Task<T> UpdateAsync<T>(VillaUpdateDTO obj, string token);
        Task<T> DeleteAsync<T>(int id, string token);
    }
}
