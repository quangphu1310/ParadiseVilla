using ParadiseVilla_API.Models;
using System.Linq.Expressions;

namespace ParadiseVilla_API.Repository.IRepository
{
    public interface IVillaNumberRepository : IReponsitory<VillaNumber>
    {
        Task<VillaNumber> UpdateAsync(VillaNumber entity);
    }
}
