using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Models
{
    public class VideoTag
    {
        public int VideoId { get; set; }
        public int TagId { get; set; }
        public virtual Video Video { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
