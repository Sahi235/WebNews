using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Models
{
    public class Article
    {
        public int Id { get; set; }
        [Remote(action: "IsSeoUrlTaken", controller: "Articles", areaName: "Panel")]
        public string SeoUrl { get; set; }
        public string Title { get; set; }
        public DateTime PublishedDate { get; set; }
        public bool IsPublished { get; set; }
        public bool IsApproved { get; set; }
        public string Body { get; set; }
        public string ShortBody { get; set; }
        public string MainImage { get; set; }
        [Display(Name = "is deleted")]
        public bool IsDeleted { get; set; } = false;
        public string UserId { get; set; }
        [Display(Name = "Author")]
        public ApplicationUser User { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifierId { get; set; }
        public bool IsModified { get; set; }
        public int ViewCount { get; set; }
        public virtual ApplicationUser Modifier { get; set; }
        public virtual ICollection<ArticleImage> Images { get; set; } = new HashSet<ArticleImage>();
        public virtual ICollection<ArticleCategory> Categories { get; set; } = new HashSet<ArticleCategory>();
        public virtual ICollection<ArticleTag> Tags { get; set; } = new HashSet<ArticleTag>();
    }
}
