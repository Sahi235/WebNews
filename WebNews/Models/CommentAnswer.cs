using System;
using System.ComponentModel.DataAnnotations;

namespace WebNews.Models
{
    public class CommentAnswer
    {
        public int Id { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Body { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime PublishedDate { get; set; }
        public int CommentId { get; set; }
        public virtual Comment Comment { get; set; }
    }
}