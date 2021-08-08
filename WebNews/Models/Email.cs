using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace WebNews.Models
{
    public class Email
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(255)]
        public string Subject { get; set; }
        public string Body { get; set; }
        [ForeignKey("SenderId_FK")]
        public string SenderId { get; set; }
        [ForeignKey("ReceiverId_FK")]
        public string ReceiverId { get; set; }
        public virtual ApplicationUser Sender { get; set; }
        public virtual ApplicationUser Receiver { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime SentDate { get; set; }
        public bool IsRead { get; set; } = false;
        public virtual ICollection<Attachments> Attachments { get; set; } = new HashSet<Attachments>();
        public virtual ICollection<EmailAnswer> Answers { get; set; }

    }
}
