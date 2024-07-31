using ParadiseVilla_Web.Models.DTO;

namespace ParadiseVilla_Web.Services.IServices
{
    public interface IVillaService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(VillaCreateDTO obj);
        Task<T> UpdateAsync<T>(VillaUpdateDTO obj);
        Task<T> DeleteAsync<T>(int id);
    }
}
