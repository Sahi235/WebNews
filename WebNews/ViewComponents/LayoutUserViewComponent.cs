using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WebNews.Models;

namespace WebNews.ViewComponents
{
    public class LayoutUserViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> userManager;

        public LayoutUserViewComponent(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            return View(user);
        }
    }
}
