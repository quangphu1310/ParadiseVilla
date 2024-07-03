using ParadiseVilla_Web.Models.DTO;

namespace ParadiseVilla_Web.Services.IServices
{
    public interface IVillaNumberService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(VillaNumberCreateDTO obj);
        Task<T> UpdateAsync<T>(VillaNumberUpdateDTO obj);
        Task<T> DeleteAsync<T>(int id);
    }
}
