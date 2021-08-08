using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Utilities
{
    public class EmailOptionDTO
    {
        public string Host { get; set; }
        public string ApiKey { get; set; }
        public string ApiKeySecret { get; set; }
        public int Port { get; set; }
        public string SenderEmail { get; set; }
    }
}
