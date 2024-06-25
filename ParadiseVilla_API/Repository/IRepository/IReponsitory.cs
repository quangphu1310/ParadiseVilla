﻿using ParadiseVilla_API.Models;
using System.Linq.Expressions;

namespace ParadiseVilla_API.Repository.IRepository
{
    public interface IReponsitory<T> where T : class
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);
        Task<T> GetAsync(Expression<Func<T, bool>> filter = null, bool tracked = true);
        Task CreateAsync(T entity);
        Task RemoveAsync(T entity);
        Task SaveAsync();
    }
}
