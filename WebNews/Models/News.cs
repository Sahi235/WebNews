using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Models
{
    public class News
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Remote(action: "IsSeoUrlTaken", controller: "News", areaName:"Panel")]
        public string SeoUrl { get; set; }
        [Required]
        public string Title { get; set; }
        public string Body { get; set; }
        public string ShortDescription { get; set; }
        public string MainImage { get; set; }
        [Display(Name = "Published Date")]
        public DateTime PublishedDate { get; set; } = DateTime.Now;
        [Display(Name = "View Count")]
        public int ViewCount { get; set; }
        [Display(Name = "Is modified")]
        public bool IsModified { get; set; }
        [Display(Name = "Modified Date")]
        public DateTime ModifiedDate { get; set; }
        public string ModfierId { get; set; }
        public ApplicationUser Modifier { get; set; }
        [Display(Name = "Is Published")]
        public bool IsPublished { get; set; }
        [Display(Name = "is deleted")]
        public bool IsDeleted { get; set; }
        public int Likes { get; set; }
        public string UserId { get; set; }
        [Display(Name = "Author")]
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<NewsImage> Image { get; set; } = new HashSet<NewsImage>();
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public virtual ICollection<NewsCategory> Categories { get; set; } = new HashSet<NewsCategory>();
        public virtual ICollection<NewsTag> Tags { get; set; } = new HashSet<NewsTag>();


    }
}
