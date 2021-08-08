using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Models
{
    public class FavouriteCateUser
    {
        public int? CategoryId { get; set; }
        public string UserId { get; set; }
        public virtual Category Category { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
