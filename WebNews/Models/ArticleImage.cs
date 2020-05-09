using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Models
{
    public class ArticleImage : Image
    {
        public int ArticleId { get; set; }
        public virtual Article Article { get; set; }
    }
}
