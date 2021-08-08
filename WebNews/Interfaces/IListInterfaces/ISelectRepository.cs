using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WebNews.Interfaces.IListInterfaces
{
    public interface ISelectRepository<T>
    {
        Task<List<T>> GetTBySelectInList(Expression<Func<T, T>> expression);
        Task<List<T>> GetTBySelectTakeInList(Expression<Func<T, T>> expression, int take);
        Task<List<T>> GetTBySelectTakeOrderByDescInList(Expression<Func<T, T>> expression, int take, Expression<Func<T, bool>> condition);
        Task<List<T>> GetTBySelectTakeOrderByInList(Expression<Func<T, T>> expression, int take, Expression<Func<T, bool>> condition);
        Task<List<T>> GetOrderByConditionTakeSelectInList(Expression<Func<T, object>> orderByExpression, Expression<Func<T, bool>> conditionExpression, int take, Expression<Func<T, T>> selectExpression);
        Task<List<T>> GetWhereSkipOrderByDescTakeSelectInList(Expression<Func<T, bool>> where, int skip, Expression<Func<T, object>> orderByDesc, int take, Expression<Func<T, T>> select );
        Task<List<T>> GetWhereOrderByDescTakeSelect(Expression<Func<T, bool>> where, Expression<Func<T, object>> orderByDesc, int take, Expression<Func<T, T>> select);
    }
}
