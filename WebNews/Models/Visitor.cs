using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Models
{
    public class Visitor
    {
        public int Id { get; set; }
        public string IP { get; set; }
        public int VisitCount { get; set; } = 1;
        public virtual Culture Culture { get; set; }
        public int CultureId { get; set; }
        public virtual ICollection<VisitorHistory> VisitorHistories { get; set; } = new HashSet<VisitorHistory>();
    }
}
