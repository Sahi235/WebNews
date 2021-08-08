using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
        public string Description { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Published date")]
        public DateTime PublishedDate { get; set; }
        [Display(Name = "Is Approved")]
        public bool IsApproved { get; set; } = false;
        [DataType(DataType.EmailAddress)]
        [MaxLength(255)]
        public string Email { get; set; }
        public int? NewsId { get; set; }
        public virtual News News { get; set; }
        public int? VideoId { get; set; }
        public virtual Video Video { get; set; }
        public virtual Gallery Gallery { get; set; }
        public int? GalleryId { get; set; }
        public virtual ICollection<CommentAnswer> Answers { get; set; } = new HashSet<CommentAnswer>();
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
    }
}
