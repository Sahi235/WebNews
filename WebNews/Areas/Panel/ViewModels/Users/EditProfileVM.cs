using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebNews.Models;

namespace WebNews.Areas.Panel.ViewModels.Users
{
    public class EditProfileVM
    {
        public ApplicationUser User { get; set; }
        public IFormFile NewImage { get; set; }
        [TempData]
        public string Message { get; set; }
    }
}
