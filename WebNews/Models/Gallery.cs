using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Models
{
    public class Gallery
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [Remote(action: "IsSeoUrlTaken", controller:"Galleries", areaName: "Panel")]
        public string SeoUrl { get; set; }
        public DateTime PublishedDate { get; set; } = DateTime.Now;
        public bool IsModified { get; set; } = false;
        public int Likes { get; set; }
        public string ModifierId { get; set; }
        public ApplicationUser Modifier { get; set; }
        public string Description { get; set; }
        [Display(Name = "is deleted")]
        public bool IsDeleted { get; set; } = false;
        public bool IsPublished { get; set; }
        public string MainImage { get; set; }
        [Display(Name = "View")]
        public int ViewCount { get; set; }
        public string UserId { get; set; }
        [Display(Name = "Author")]
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<GalleryImage> Images { get; set; } = new HashSet<GalleryImage>();
        public virtual ICollection<GalleryCategory> Categories { get; set; } = new HashSet<GalleryCategory>();
        public virtual ICollection<GalleryTag> Tags { get; set; } = new HashSet<GalleryTag>();
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

    }
}
