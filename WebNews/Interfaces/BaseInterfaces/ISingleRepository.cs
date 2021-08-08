using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WebNews.Interfaces.BaseInterfaces
{
    public interface ISingleRepository<T>
    {
        Task<T> GetT(int id);
        Task<T> GetTConditionSingle(Expression<Func<T, bool>> expression);
    }
}
