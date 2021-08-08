using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebNews.Data;
using WebNews.Interfaces.ArticleInterface;
using WebNews.Models;
using WebNews.SQLRepositories.BaseSQLRepositories;

namespace WebNews.SQLRepositories.ArticleSQLRepository
{
    public class ArticleSQLRepository : SQLBaseAllImplementations<Article>, IArticleRepository
    {
        private readonly DatabaseContext context;

        public ArticleSQLRepository(DatabaseContext context): base(context)
        {
            this.context = context;
        }
    }
}
