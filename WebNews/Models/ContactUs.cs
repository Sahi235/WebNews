using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Models
{
    public class ContactUs
    {
        public int Id { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(1024)]
        public string Subject { get; set; }
        [MaxLength(255)]
        public string Email { get; set; }
        public string Body { get; set; }
        public string Answer { get; set; }
        public bool IsAnswered { get; set; }
    }
}
