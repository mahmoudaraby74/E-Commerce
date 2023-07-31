using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Data
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreDbContext context;

        public GenericRepository(StoreDbContext context)
        {
            this.context = context;
        }
        public void Add(T entity)
            => context.Add(entity);
        public void Delete(T entity)
            => context.Remove(entity);
        public async Task<T> GetByIdAsync(int id)
            => await context.Set<T>().FindAsync(id);

        public async Task<T> GetEntityWithSpecifications(ISpecifications<T> specifications)
            => await SpecificationEvalutor<T>.GetQuery(context.Set<T>(), specifications).FirstOrDefaultAsync();

        public async Task<IReadOnlyList<T>> ListAllAsync()
            => await context.Set<T>().ToListAsync();

        public async Task<IReadOnlyList<T>> ListAsync(ISpecifications<T> specifications)
            => await SpecificationEvalutor<T>.GetQuery(context.Set<T>(), specifications).ToListAsync();

        public void Update(T entity)
            => context.Update(entity);
        public async Task<IReadOnlyList<T>> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> queryable = context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                queryable = queryable.Include(includeProperty);
            }
            return await queryable.ToListAsync();
        }

        public async Task<int> CountAsync(ISpecifications<T> specifications)
            => await SpecificationEvalutor<T>.GetQuery(context.Set<T>(), specifications).CountAsync();
    }
}
