using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WebNews.Models;

namespace WebNews.ViewComponents
{
    public class UserLayoutNEViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UserLayoutNEViewComponent(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var user = await userManager.Users.Include(c => c.ReceivedEmail).ThenInclude(c => c.Answers).SingleOrDefaultAsync(c => c.Id == userId.Id);

            return View(user);
        }
    }
}
