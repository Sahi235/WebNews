using System;
using System.ComponentModel.DataAnnotations;

namespace WebNews.Models
{
    public class VisitorHistory
    {
        public int Id { get; set; }
        public virtual Visitor Visitor { get; set; }
        public int VisitorId { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime VisitTime { get; set; }
    }
}