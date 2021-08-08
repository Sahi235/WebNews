using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebNews.Data;
using WebNews.Interfaces.BaseInterfaces;
using WebNews.Interfaces.NewsInterfaces;
using WebNews.Models;
using WebNews.SQLRepositories.BaseSQLRepositories;

namespace WebNews.SQLRepositories.NewsSQLRepositories
{
    public class NewsSQLRepositoryBase : SQLBaseAllImplementations<News>, INewsRepositories<News>
    {
        private readonly DatabaseContext context;

        public NewsSQLRepositoryBase(DatabaseContext context) : base(context)
        {
            this.context = context;
        }

        
    }
}
