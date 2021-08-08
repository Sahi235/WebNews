using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WebNews.Interfaces.IListInterfaces
{
    public interface IConditionRepository<T>
    {
        Task<List<T>> GetTByConditionInList(Expression<Func<T, bool>> expression);
        Task<List<T>> GetTByConditionTakeInList(Expression<Func<T, bool>> expression, int take);
    }
}
