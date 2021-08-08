using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Interfaces.BaseInterfaces
{
    public interface IRepositoryBase<T>
    {
        Task<T> FindT(int id);
        void Add(T entity);
        void AddRange(List<T> entities);
        void Update(T entity);
        void UpdateRange(List<T> entities);
        void RemoveT(T entity);
        void RemoveRange(List<T> entities);
    }
}
