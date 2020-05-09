using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Models
{
    public class NewsImage : Image
    {
        public int NewsId { get; set; }
        public virtual News News { get; set; }
    }
}
