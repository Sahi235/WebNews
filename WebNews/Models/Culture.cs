using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Models
{
    public class Culture
    {
        public int Id { get; set; }
        public string CultureName { get; set; }
        public int VisitCount { get; set; } = 0;
        public virtual ICollection<Visitor> Visitors { get; set; } = new HashSet<Visitor>();

    }
}
