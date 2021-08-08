using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebNews.Interfaces.BaseInterfaces;
using WebNews.Models;

namespace WebNews.Interfaces.ArticleInterface
{
    public interface IArticleRepository : IRepositoryBase<Article>, IListRepository<Article>, IIQueryableRepository<Article>, ISingleRepository<Article>
    {
    }
}
