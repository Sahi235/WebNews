using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Models
{
    public class NewsTag
    {
        public int NewsId { get; set; }
        public int TagId { get; set; }
        public virtual News News { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
