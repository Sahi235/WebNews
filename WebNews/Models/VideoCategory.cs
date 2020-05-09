using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Models
{
    public class VideoCategory
    {
        public int VideoId { get; set; }
        public int CategoryId { get; set; }
        public virtual Video Video { get; set; }
        public virtual Category Category { get; set; }
    }
}
