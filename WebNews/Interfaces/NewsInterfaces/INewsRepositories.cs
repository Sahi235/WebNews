using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebNews.Interfaces.BaseInterfaces;
using WebNews.Models;

namespace WebNews.Interfaces.NewsInterfaces
{
    public interface INewsRepositories<News> : IRepositoryBase<News>, IListRepository<News>, IIQueryableRepository<News>, ISingleRepository<News>
    {
    }
}
