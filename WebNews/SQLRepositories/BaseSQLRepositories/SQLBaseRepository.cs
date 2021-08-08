using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebNews.Data;
using WebNews.Interfaces;
using WebNews.Interfaces.BaseInterfaces;

namespace WebNews.SQLRepositories.BaseSQLRepositories
{
    public abstract class SQLBaseRepository<T> : IRepositoryBase<T> 
        where T : class
    {
        private DatabaseContext context { get; set; }

        public SQLBaseRepository(DatabaseContext context)
        {
            this.context = context;
        }

        public void Add(T entity)
        {
            context.Set<T>().AddAsync(entity);
        }

        public void AddRange(List<T> entities)
        {
            context.Set<T>().AddRange(entities);
        }

        public async Task<T> FindT(int id)
        {
            return await context.Set<T>().FindAsync(id);
        }

        public void RemoveRange(List<T> entities)
        {
            context.Set<T>().RemoveRange(entities);
        }

        public void RemoveT(T entity)
        {
            context.Set<T>().Remove(entity);
        }

        public void Update(T entity)
        {
            context.Set<T>().Remove(entity);
        }

        public void UpdateRange(List<T> entities)
        {
            context.Set<T>().UpdateRange(entities);
        }
    }
}
