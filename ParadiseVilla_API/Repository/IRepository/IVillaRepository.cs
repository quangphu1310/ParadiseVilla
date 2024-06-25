using ParadiseVilla_API.Models;
using System.Linq.Expressions;

namespace ParadiseVilla_API.Repository.IRepository
{
    public interface IVillaRepository: IReponsitory<Villa>
    {
        Task<Villa> UpdateAsync(Villa entity);
    }
}
