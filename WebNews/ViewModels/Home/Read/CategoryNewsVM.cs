using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReflectionIT;
using WebNews.Models;
using WebNews.ViewModels.Accounts;

namespace WebNews.ViewModels.Home.Read
{
    public class CategoryNewsVM
    {
        public CategoryNewsVM()
        {
            RandomNews = new List<News>();
            Comments = new List<Comment>();
            PopularNews = new List<News>();
        }
        public ReflectionIT.Mvc.Paging.PagingList<News> News { get; set; }
        public List<News> RandomNews { get; set; }
        public List<News> PopularNews { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
