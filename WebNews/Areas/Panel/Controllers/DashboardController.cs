using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WebNews.Areas.Panel.ViewModels.Dashboard;
using WebNews.Data;
using WebNews.Models;

namespace WebNews.Areas.Panel.Controllers
{
    [Area("Panel")]
    public class DashboardController : Controller
    {
        private readonly DatabaseContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;

        public DashboardController(DatabaseContext context,
                                   UserManager<ApplicationUser> userManager,
                                   RoleManager<ApplicationRole> roleManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            int userCount = userManager.Users.AsNoTracking().Count();
            int newsCount = await context.News.AsNoTracking().CountAsync();
            int galleriesCount = await context.Galleries.AsNoTracking().CountAsync();
            int articlesCount = await context.Articles.AsNoTracking().CountAsync();
            int allCommentsCount = await context.Comments.AsNoTracking().CountAsync();
            int approvedCommentsCount = await context.Comments.AsNoTracking().Where(c => c.IsApproved == true).CountAsync();
            int unapprovedCommensCount = await context.Comments.AsNoTracking().Where(c => c.IsApproved == false).CountAsync();

            var lastFoutNews = await context.News
                                                .AsNoTracking()
                                                    .Select(c => new News
                                                    {
                                                        Id = c.Id,
                                                        Title = c.Title,
                                                        MainImage = c.MainImage,
                                                        PublishedDate = c.PublishedDate,
                                                        ShortDescription = c.ShortDescription,
                                                    })
                                                        .OrderByDescending(c => c.PublishedDate)
                                                             .Take(4)
                                                                 .ToListAsync();
            var lastFoutArticles = await context.Articles
                                                .AsNoTracking()
                                                    .Select(c => new Article
                                                    {
                                                        Id = c.Id,
                                                        Title = c.Title,
                                                        ShortBody = c.ShortBody,
                                                        MainImage = c.MainImage,
                                                        PublishedDate = c.PublishedDate,
                                                    })
                                                        .OrderByDescending(c => c.PublishedDate)
                                                             .Take(4)
                                                                 .ToListAsync();
            var lastFourGalleries = await context.Galleries
                                                .AsNoTracking()
                                                    .Select(c => new Gallery
                                                    {
                                                        Id = c.Id,
                                                        Title = c.Title,
                                                        MainImage = c.MainImage,
                                                        PublishedDate = c.PublishedDate,
                                                    })
                                                        .OrderByDescending(c => c.PublishedDate)
                                                            .Take(4)
                                                                .ToListAsync();
            var allComments = await context.Comments
                                                .AsNoTracking()
                                                    .Select(c => new Comment
                                                    {
                                                        Id = c.Id,
                                                        Name = c.Name,
                                                        NewsId = c.NewsId,
                                                        IsApproved = c.IsApproved,
                                                        Description = c.Description,
                                                        PublishedDate = c.PublishedDate,
                                                    })
                                                         .OrderByDescending(c => c.PublishedDate)
                                                            .Take(20)
                                                                .ToListAsync();
            var popularNews = await context.News
                                               .AsNoTracking()
                                                    .Select(c => new News
                                                    {
                                                        Id = c.Id,
                                                        Title = c.Title,
                                                        ViewCount = c.ViewCount,
                                                        PublishedDate = c.PublishedDate,
                                                    })
                                                        .OrderByDescending(c => c.PublishedDate)
                                                            .Take(6)
                                                                .ToListAsync();

            //var adminRole = await roleManager.FindByNameAsync("Admin");
            //var stuff = from s in userManager.Users
            //            .Include(c => c.Roles).ThenInclude(c => c.Role)
            //            from n in context.UserRoles
            //            where (n.RoleId == adminRole.Id)
            //            select new ApplicationUser
            //            {
            //                Id = s.Id,
            //                UserName = s.UserName,
            //                Roles = s.Roles,
            //            };

            var stuff = await userManager.GetUsersInRoleAsync("Admin");
            var roles = await roleManager.Roles.Include(c => c.Users).ThenInclude(c => c.Role).SingleOrDefaultAsync(c => c.Name == "Admin");
            DashboardIndexVm model = new DashboardIndexVm()
            {
                UserCount = userCount,
                NewsCount = newsCount,
                GalleriesCount = galleriesCount,
                AllCommentsCount = allCommentsCount,
                UnApprovedCommentsCount = unapprovedCommensCount,
                ApprovedCommentsCount = approvedCommentsCount,
                LastFourNews = lastFoutNews,
                LastFourGalleries = lastFourGalleries,
                LastFourArticles = lastFoutArticles,
                AllComments = allComments,
                Stuff = roles,
                PopularNews = popularNews,
            };


            return View(model);
        }
    }
}