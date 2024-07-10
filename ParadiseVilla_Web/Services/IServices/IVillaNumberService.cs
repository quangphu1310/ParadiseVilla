using ParadiseVilla_Web.Models.DTO;

namespace ParadiseVilla_Web.Services.IServices
{
    public interface IVillaNumberService
    {
        Task<T> GetAllAsync<T>(string token);
        Task<T> GetAsync<T>(int id, string token);
        Task<T> CreateAsync<T>(VillaNumberCreateDTO obj, string token);
        Task<T> UpdateAsync<T>(VillaNumberUpdateDTO obj, string token);
        Task<T> DeleteAsync<T>(int id, string token);
    }
}
