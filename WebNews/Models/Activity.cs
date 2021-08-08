using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ApplicationUser User { get; set; }
        public DateTime ActivityDate { get; set; }
    }
}
