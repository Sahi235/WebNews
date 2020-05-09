using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Models
{
    public class PaginationSetting
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Display(Name = "Item number in page")]
        public int ItemNumber { get; set; }
    }
}
