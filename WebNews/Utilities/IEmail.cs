using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Utilities
{
    public interface IEmail
    {
        Task Send(string emailAdress, string body, EmailOptionDTO option);
    }
}
