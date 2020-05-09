using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Models
{
    public class GalleryTag
    {
        public int GalleryId { get; set; }
        public int TagId { get; set; }
        public virtual Gallery Gallery { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
