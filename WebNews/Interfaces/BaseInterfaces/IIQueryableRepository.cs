using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WebNews.Interfaces.BaseInterfaces
{
    public interface IIQueryableRepository<T>
    {
        IQueryable<T> GetTByConditionInIQueryable(Expression<Func<T, bool>> expression);
        IQueryable<T> GetTByOrderByDescInQueryable(Expression<Func<T, object>> expression);
        IQueryable<T> GetTByOrderByInQueryable(Expression<Func<T, object>> expression);
    }
}
