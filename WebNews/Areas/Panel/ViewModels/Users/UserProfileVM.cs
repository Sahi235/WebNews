using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebNews.Models;

namespace WebNews.Areas.Panel.ViewModels.Users
{
    public class UserProfileVM
    {
        public ApplicationUser User { get; set; }
        public List<ApplicationRole> Roles { get; set; } = new List<ApplicationRole>();
        public List<News> News { get; set; } = new List<News>();
        public List<Gallery> Galleries { get; set; } = new List<Gallery>();
        public List<Article> Articles { get; set; } = new List<Article>();
        [TempData]
        public string Message { get; set; }
    }
}
