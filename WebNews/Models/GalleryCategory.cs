using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Models
{
    public class GalleryCategory
    {
        public int GalleryId { get; set; }
        public int CategoryId { get; set; }
        public virtual Gallery Gallery { get; set; }
        public virtual Category Category { get; set; }
    }
}
