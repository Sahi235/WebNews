using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebNews.Data;
using WebNews.Models;

namespace WebNews.ViewComponents
{
    public class LeftNavEmailsViewComponent : ViewComponent
    {
        private readonly DatabaseContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public LeftNavEmailsViewComponent(DatabaseContext context,
                                          UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var emails = await context.Emails.
                                    AsNoTracking()
                                            .Where(c => c.ReceiverId == user.Id)
                                                    .Include(c => c.Answers)
                                                            .ToListAsync();
            return View(emails);
        }
    }
}
