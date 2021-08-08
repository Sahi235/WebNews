using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebNews.Models;
using WebNews.ViewModels.Accounts;

namespace WebNews.ViewModels.Home.Read
{
    public class BlogDetailsVM
    {
        public News News { get; set; }
        public List<News> RandomNews { get; set; }
        public List<News> PopularNews { get; set; }
        public List<News> RelatedNews { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
