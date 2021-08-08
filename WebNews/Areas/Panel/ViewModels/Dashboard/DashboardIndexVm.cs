using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebNews.Models;

namespace WebNews.Areas.Panel.ViewModels.Dashboard
{
    public class DashboardIndexVm
    {
        public int NewsCount { get; set; }
        public int GalleriesCount { get; set; }
        public int UserCount { get; set; }
        public int AllCommentsCount { get; set; }
        public int ApprovedCommentsCount { get; set; }
        public int UnApprovedCommentsCount { get; set; }
        public List<Comment> AllComments { get; set; } = new List<Comment>();
        public List<News> LastFourNews { get; set; } = new List<News>();
        public List<Gallery> LastFourGalleries { get; set; } = new List<Gallery>();
        public List<Article> LastFourArticles { get; set; } = new List<Article>();
        public ApplicationRole Stuff { get; set; }
        public List<News> PopularNews { get; set; } = new List<News>();

    }
}
