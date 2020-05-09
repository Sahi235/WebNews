using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<NewsCategory> News { get; set; } = new HashSet<NewsCategory>();
        public virtual ICollection<GalleryCategory> Galleries { get; set; } = new HashSet<GalleryCategory>();
        public virtual ICollection<ArticleCategory> Articles { get; set; } = new HashSet<ArticleCategory>();
        public virtual ICollection<VideoCategory> Videos { get; set; } = new HashSet<VideoCategory>();
    }
}
