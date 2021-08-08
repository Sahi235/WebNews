using System;

namespace WebNews.Models
{
    public class EmailAnswer
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsRead { get; set; }
        public int EmailId { get; set; }
        public Email Email { get; set; }
        public DateTime SentDate { get; set; }
    }
}