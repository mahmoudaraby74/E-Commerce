using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreDbContext context;
        private Hashtable repository;

        public UnitOfWork(StoreDbContext context)
        {
            this.context = context;
        }
        public async Task<int> Complete()
        =>  await context.SaveChangesAsync();

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            if (repository == null)
                repository = new Hashtable();
            var type = typeof(TEntity).Name;
            if (!repository.ContainsKey(type))
            {
                var repositoryType = typeof(GenericRepository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)),context);
                repository.Add(type, repositoryInstance);
            }
            return (IGenericRepository<TEntity>)repository[type];
        }
    }
}
