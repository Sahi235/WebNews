using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebNews.Models;
using WebNews.ViewModels.Accounts;

namespace WebNews.ViewModels.Home.Read
{
    public class IndexVM
    {
        public IndexVM()
        {
            News = new List<News>();
            RandomNews = new List<News>();
            PopularNews = new List<News>();
            Videos = new List<Video>();
            Galleries = new List<Gallery>();
            Articles = new List<Article>();
        }
        public List<News> News { get; set; }
        public List<News> RandomNews { get; set; }
        public List<News> PopularNews { get; set; }
        public List<Video> Videos { get; set; }
        public List<Gallery> Galleries { get; set; }
        public List<Article> Articles { get; set; }
    }
}
