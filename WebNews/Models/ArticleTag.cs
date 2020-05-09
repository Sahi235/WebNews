using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Models
{
    public class ArticleTag
    {
        public int TagId { get; set; }
        public int ArticleId { get; set; }
        public virtual Article Article { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
