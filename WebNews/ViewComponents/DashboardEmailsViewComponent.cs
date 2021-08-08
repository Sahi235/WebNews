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
    public class DashboardEmailsViewComponent : ViewComponent
    {
        private readonly DatabaseContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public DashboardEmailsViewComponent(DatabaseContext context,
                                            UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var emails = await context.Emails
                .AsNoTracking()
                .Select(c => new Email
                {
                    Id = c.Id,
                    Body = c.Body,
                    Sender = userManager.Users.Select(e => new ApplicationUser
                    {
                        Id = e.Id,
                        ImageUrl = e.ImageUrl,
                        UserName = e.UserName,
                    }).SingleOrDefault(r => r.Id == c.SenderId),
                    SentDate = c.SentDate,
                    IsRead = c.IsRead,
                    ReceiverId = c.ReceiverId,
                })
                .Where(c => c.ReceiverId == user.Id)
                .OrderByDescending(c => c.SentDate)
                .Take(4)
                .ToListAsync();

            return View(emails);
        }
    }
}
