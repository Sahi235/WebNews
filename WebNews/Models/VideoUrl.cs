using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Models
{
    public class VideoUrl
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int VideoId { get; set; }
        public Video Video { get; set; }
    }
}
