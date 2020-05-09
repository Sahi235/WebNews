using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Models
{
    public class GalleryImage : Image
    {
        public int GalleryId { get; set; }
        public virtual Gallery Gallery { get; set; }
    }
}
