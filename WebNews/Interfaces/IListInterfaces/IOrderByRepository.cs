using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WebNews.Interfaces.IListInterfaces
{
    public interface IOrderByRepository<T>
    {
        Task<List<T>> GetTTByOrderByDescInList(Expression<Func<T, object>> expression);
        Task<List<T>> GetTByOrderByInList(Expression<Func<T, DateTime>> expression);
        Task<List<T>> GetTByOrderByInList(Expression<Func<T, object>> expression);
        Task<List<T>> GetTByOrderByDescTakeInList(Expression<Func<T, object>> expression, int take);
        Task<List<T>> GetTByOrderByTakeInList(Expression<Func<T, object>> expression, int take);
    }
}