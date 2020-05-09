using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Models
{
    public class Video
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string SeoUrl { get; set; }
        public bool MainPage { get; set; }
        public string VideoUrl { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        [Display(Name = "Published date")]
        public DateTime PublishedDate { get; set; }
        [Display(Name = "Is published")]
        public bool IsPublished { get; set; }
        public string UserId { get; set; }
        [Display(Name = "Author")]
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public virtual ICollection<VideoCategory> Categories { get; set; } = new HashSet<VideoCategory>();
        public virtual ICollection<VideoUrl> Videos { get; set; } = new HashSet<VideoUrl>();
        public virtual ICollection<VideoTag> Tags { get; set; } = new HashSet<VideoTag>();
    }
}
