using Core.Entities;
using Core.Specifications;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Core.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> ListAllAsync();
        Task<T> GetEntityWithSpecifications(ISpecifications<T> specifications);
        Task<IReadOnlyList<T>> ListAsync(ISpecifications<T> specifications);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<IReadOnlyList<T>> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties);
        Task<int> CountAsync(ISpecifications<T> specifications);
    }
}
