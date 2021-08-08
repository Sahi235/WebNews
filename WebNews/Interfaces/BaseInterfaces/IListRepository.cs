using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WebNews.Interfaces.IListInterfaces;

namespace WebNews.Interfaces.BaseInterfaces
{
    public interface IListRepository<T> : IConditionRepository<T>, IOrderByRepository<T>, ISelectRepository<T>
    {
    }
}
