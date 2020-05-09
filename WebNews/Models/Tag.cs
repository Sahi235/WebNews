using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        public virtual ICollection<NewsTag> News { get; set; } = new HashSet<NewsTag>();
        public virtual ICollection<GalleryTag> Galleries { get; set; } = new HashSet<GalleryTag>();
        public virtual ICollection<ArticleTag> Articles { get; set; } = new HashSet<ArticleTag>();
        public virtual ICollection<VideoTag> Videos { get; set; } = new HashSet<VideoTag>();

    }
}
