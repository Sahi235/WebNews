using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ReflectionIT.Mvc.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebNews.Data;
using WebNews.Models;
using WebNews.ViewModels.Home.Read;

namespace WebNews.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseContext context;
        private readonly IMemoryCache cache;

        public HomeController(DatabaseContext context,
                              IMemoryCache cache)
        {
            this.context = context;
            this.cache = cache;
        }


        public async Task<IActionResult> Index()
        {
            var news = await context.News
            .AsNoTracking()
                .OrderByDescending(c => c.PublishedDate)
                    .Where(c => c.IsPublished == true)
                        .Take(3)
                            .Include(c => c.Categories)
                                .ThenInclude(c => c.Category)
                                    .Include(c => c.User)
                                        .Include(c => c.Comments)
                                            .Select(c => new News
                                            {
                                                Id = c.Id,
                                                User = c.User,
                                                Likes = c.Likes,
                                                Title = c.Title,
                                                SeoUrl = c.SeoUrl,
                                                Comments = c.Comments
                                                .Where(e => e.IsApproved == true)
                                                .ToList(),
                                                ViewCount = c.ViewCount,
                                                MainImage = c.MainImage,
                                                Categories = c.Categories
                                                .Select(e => new NewsCategory
                                                {
                                                    Category = e.Category,
                                                    CategoryId = e.CategoryId,
                                                }).ToList(),
                                                IsPublished = c.IsPublished,
                                                PublishedDate = c.PublishedDate,
                                                ShortDescription = c.ShortDescription,
                                            })
                                            .ToListAsync();



            var randomNews = await context.News
                                        .AsNoTracking()
                                            .Where(c => c.PublishedDate > DateTime.Now.AddDays(-7)
                                                    && c.IsPublished == true)
                                                .Skip(3)
                                                    .OrderByDescending(c => Guid.NewGuid())
                                                        .Take(5)
                                                            .Include(c => c.Categories)
                                                                .ThenInclude(c => c.Category)
                                                                    .Select(c => new News
                                                                    {
                                                                        Id = c.Id,
                                                                        Title = c.Title,
                                                                        SeoUrl = c.SeoUrl,
                                                                        MainImage = c.MainImage,
                                                                        Categories = c.Categories
                                                                        .Select(e => new NewsCategory
                                                                        {
                                                                            Category = e.Category,
                                                                            CategoryId = e.CategoryId,
                                                                        }).ToList(),
                                                                        PublishedDate = c.PublishedDate,
                                                                    })
                                                                    .ToListAsync();

            var galleries = await context.Galleries
                                        .AsNoTracking()
                                            .Where(c => c.IsPublished == true)
                                                .OrderByDescending(c => c.PublishedDate)
                                                    .Take(4)
                                                        .Include(c => c.Categories)
                                                            .ThenInclude(c => c.Category)
                                                                .Select(c => new Gallery
                                                                {
                                                                    Title = c.Title,
                                                                    Likes = c.Likes,
                                                                    Comments = c.Comments,
                                                                    MainImage = c.MainImage,
                                                                    Categories = c.Categories
                                                                    .Select(e => new GalleryCategory
                                                                    {
                                                                        Category = e.Category,
                                                                        CategoryId = e.CategoryId,
                                                                    }).ToList(),
                                                                })
                                                                .ToListAsync();



            var popularNews = await context.News
                                        .AsNoTracking()
                                            .Where(c => c.IsPublished == true)
                                                .OrderByDescending(c => c.ViewCount)
                                                    .Take(5)
                                                        .Select(c => new News
                                                        {
                                                            Id = c.Id,
                                                            Title = c.Title,
                                                            SeoUrl = c.SeoUrl,
                                                            PublishedDate = c.PublishedDate,
                                                        })
                                                        .ToListAsync();


            var videos = await context.Videos
                                        .AsNoTracking()
                                            .Where(c => c.IsPublished == true)
                                                .OrderByDescending(c => c.PublishedDate)
                                                    .Take(3)
                                                        .Select(c => new Video
                                                        {
                                                            Id = c.Id,
                                                            Title = c.Title,
                                                            ImageUrl = c.ImageUrl,
                                                            VideoUrl = c.VideoUrl,
                                                        })
                                                        .ToListAsync();



            var articles = await context.Articles
                                        .AsNoTracking()
                                            .Where(c => c.IsPublished == true)
                                                .OrderByDescending(c => c.PublishedDate)
                                                    .Take(6)
                                                        .Select(c => new Article
                                                        {
                                                            Id = c.Id,
                                                            Title = c.Title,
                                                            MainImage = c.MainImage,
                                                            PublishedDate = c.PublishedDate,
                                                        })
                                                        .ToListAsync();



            IndexVM model = new IndexVM
            {
                News = news,
                RandomNews = randomNews,
                Videos = videos,
                Articles = articles,
                Galleries = galleries,
                PopularNews = popularNews,
            };


            return View(model);
        }







        //BlogDetailsView
        [HttpGet]
        [Route("NewsDetails/{seoUrl}")]
        public async Task<IActionResult> NewsDetails(string seoUrl)
        {

            BlogDetailsVM model = new BlogDetailsVM();


            News newsDetails = new News();
            if (!cache.TryGetValue("NewsDetails", out newsDetails))
            {
                if (newsDetails == null)
                {
                    #region NewsDetailsCreater
                    newsDetails = await context.News
                                                .AsNoTracking()
                                                .Where(c => c.SeoUrl == seoUrl && c.IsPublished == true)
                                                .Include(c => c.Categories)
                                                .ThenInclude(c => c.Category)
                                                .Include(c => c.Image)
                                                .Include(c => c.Tags)
                                                .ThenInclude(c => c.Tag)
                                                .Include(c => c.User)
                                                .Select(c => new News
                                                {
                                                    Id = c.Id,
                                                    Tags = c.Tags,
                                                    User = c.User,
                                                    Body = c.Body,
                                                    Title = c.Title,
                                                    Image = c.Image,
                                                    Likes = c.Likes,
                                                    SeoUrl = c.SeoUrl,
                                                    Comments = c.Comments
                                                    .Where(c => c.IsApproved == true)
                                                    .Select(e => new Comment
                                                    {
                                                        Id = e.Id,
                                                        Name = e.Name,
                                                        Description = e.Description,
                                                        PublishedDate = e.PublishedDate,
                                                    }).OrderByDescending(e => e.PublishedDate).ToList(),
                                                    ViewCount = c.ViewCount,
                                                    MainImage = c.MainImage,
                                                    Categories = c.Categories
                                                    .Select(e => new NewsCategory
                                                    {
                                                        Category = e.Category,
                                                        CategoryId = e.CategoryId,
                                                    }).ToList(),
                                                    PublishedDate = c.PublishedDate,
                                                }).SingleOrDefaultAsync();
                    #endregion
                }
                var cacheEntryOption = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(3));

                cache.Set("NewsDetails", newsDetails, cacheEntryOption);
            }




            var randomNews = await context.News
                                            .AsNoTracking()
                                                .Where(c => c.PublishedDate > DateTime.Now.AddDays(-7)
                                                        && c.IsPublished == true)
                                                            .Skip(3)
                                                                .OrderByDescending(c => Guid.NewGuid())
                                                                    .Take(8)
                                                                        .Include(c => c.Categories)
                                                                            .ThenInclude(c => c.Category)
                                                                                .Select(c => new News
                                                                                {
                                                                                    Id = c.Id,
                                                                                    Title = c.Title,
                                                                                    SeoUrl = c.SeoUrl,
                                                                                    MainImage = c.MainImage,
                                                                                    Categories = c.Categories
                                                                                    .Select(e => new NewsCategory
                                                                                    {
                                                                                        Category = e.Category,
                                                                                        CategoryId = e.CategoryId,
                                                                                    }).ToList(),
                                                                                    PublishedDate = c.PublishedDate,
                                                                                })
                                                                                .ToListAsync();



            var popularNews = await context.News
                                            .AsNoTracking()
                                                .Where(c => c.IsPublished == true)
                                                    .OrderByDescending(c => c.ViewCount)
                                                        .Take(6)
                                                            .Select(c => new News
                                                            {
                                                                Id = c.Id,
                                                                Title = c.Title,
                                                                SeoUrl = c.SeoUrl,
                                                                PublishedDate = c.PublishedDate,
                                                            })
                                                            .ToListAsync();




            var latestComments = await context.Comments
                                                .AsNoTracking()
                                                    .OrderByDescending(c => c.PublishedDate)
                                                        .Where(c => c.IsApproved == true)
                                                            .Take(6)
                                                                .Include(c => c.News)
                                                                    .Select(c => new Comment
                                                                    {
                                                                        Id = c.Id,
                                                                        News = c.News,
                                                                        Name = c.Name,
                                                                        Description = c.Description,
                                                                        PublishedDate = c.PublishedDate,
                                                                    })
                                                                    .ToListAsync();

            if (newsDetails.Categories != null)
            {
                foreach (var cate in newsDetails.Categories)
                {
                    var relatedNews = await context.NewsCategories
                                        .AsNoTracking()
                                            .Where(c => c.CategoryId == cate.CategoryId)
                                                .Include(c => c.News)
                                                    .Select(c => c.News)
                                                        .OrderByDescending(c => c.PublishedDate)
                                                                //.SkipWhile(c => c.Id == newsDetails.Id)
                                                                .Take(4)
                                                                    .Select(c => new News
                                                                    {
                                                                        Id = c.Id,
                                                                        Title = c.Title,
                                                                        SeoUrl = c.SeoUrl,
                                                                        MainImage = c.MainImage,
                                                                        PublishedDate = c.PublishedDate,
                                                                    }).ToListAsync();
                    model.RelatedNews = relatedNews;

                    break;
                }
            }
            else
            {
                foreach (var tag in newsDetails.Tags)
                {
                    var relatedNews = await context.NewsTags
                                            .AsNoTracking()
                                                .Where(c => c.TagId == tag.TagId)
                                                    .Include(c => c.News)
                                                        .Select(c => c.News)
                                                            .OrderByDescending(c => c.PublishedDate)
                                                                    //.SkipWhile(c => c.Id == newsDetails.Id)
                                                                    .Take(4)
                                                                        .Select(c => new News
                                                                        {
                                                                            Id = c.Id,
                                                                            Title = c.Title,
                                                                            SeoUrl = c.SeoUrl,
                                                                            MainImage = c.MainImage,
                                                                            PublishedDate = c.PublishedDate,
                                                                        }).ToListAsync();
                    model.RelatedNews = relatedNews;
                    break;
                }
            }


            model.News = newsDetails;
            model.PopularNews = popularNews;
            model.RandomNews = randomNews;
            model.Comments = latestComments;


            var newsCount = await context.News.FindAsync(newsDetails.Id);
            newsCount.ViewCount += 1;
            context.News.Attach(newsCount);
            await context.SaveChangesAsync();

            return View(model);
        }


        

        [HttpPost]
        [Route("NewsDetails/{seoUrl}")]
        public async Task<IActionResult> NewsDetails([Bind("Name,Description,Email")] Comment comment, string seoUrl)
        {
            var news = await context.News.Where(c => c.SeoUrl == seoUrl).SingleOrDefaultAsync();
            if (news == null)
            {
                return NotFound();
            }
            comment.News = news;
            comment.NewsId = news.Id;
            comment.PublishedDate = DateTime.Now;
            await context.Comments.AddAsync(comment);
            await context.SaveChangesAsync();
            return RedirectToAction("NewsDetails", new RouteValueDictionary(new { controller = "Home", action = "NewsDetails", seoUrl = news.SeoUrl }));
        }



        [Route("categorynews/{name}")]
        [Route("categorynews/{name}/{page}")]
        public async Task<IActionResult> CategoryNews(string name, int page = 1)
        {

            var category = await context.Categories.Where(c => c.Name == name).SingleOrDefaultAsync();

            var news = context.NewsCategories
               .AsNoTracking()
               .Where(c => c.CategoryId == category.Id)
               .Select(c => c.News)
               .Select(c => new News
               {
                   Id = c.Id,
                   User = c.User,
                   Likes = c.Likes,
                   Title = c.Title,
                   SeoUrl = c.SeoUrl,
                   Comments = c.Comments
                   .Where(e => e.IsApproved == true)
                   .ToList(),
                   MainImage = c.MainImage,
                   Categories = c.Categories
                   .Select(e => new NewsCategory
                   {
                       Category = e.Category,
                       CategoryId = e.CategoryId,
                   }).ToList(),
                   IsPublished = c.IsPublished,
                   PublishedDate = c.PublishedDate,
                   ShortDescription = c.ShortDescription,
               })
               .Where(c => c.IsPublished == true)
               .OrderByDescending(c => c.PublishedDate);

            var newsModel = await PagingList.CreateAsync(news, 4, page);
            newsModel.Action = nameof(CategoryNews);
            newsModel.PageParameterName = "page";



            var randomNews = await context.News
                                            .AsNoTracking()
                                                .Where(c => c.PublishedDate > DateTime.Now.AddDays(-3)
                                                        && c.IsPublished == true)
                                                            .Skip(3)
                                                                .OrderByDescending(c => Guid.NewGuid())
                                                                    .Take(8)
                                                                        .Include(c => c.Categories)
                                                                            .ThenInclude(c => c.Category)
                                                                                .Select(c => new News
                                                                                {
                                                                                    Id = c.Id,
                                                                                    Title = c.Title,
                                                                                    SeoUrl = c.SeoUrl,
                                                                                    MainImage = c.MainImage,
                                                                                    Categories = c.Categories
                                                                                    .Select(e => new NewsCategory
                                                                                    {
                                                                                        Category = e.Category,
                                                                                        CategoryId = e.CategoryId,
                                                                                    }).ToList(),
                                                                                    PublishedDate = c.PublishedDate,
                                                                                })
                                                                                .ToListAsync();



            var popularNews = await context.News
                                            .AsNoTracking()
                                                .Where(c => c.IsPublished == true)
                                                    .OrderByDescending(c => c.ViewCount)
                                                        .Take(6)
                                                            .Select(c => new News
                                                            {
                                                                Id = c.Id,
                                                                Title = c.Title,
                                                                SeoUrl = c.SeoUrl,
                                                                PublishedDate = c.PublishedDate,
                                                            })
                                                            .ToListAsync();

            var latestComments = await context.Comments
                                    .AsNoTracking()
                                        .OrderByDescending(c => c.PublishedDate)
                                            .Where(c => c.IsApproved == true)
                                                .Take(6)
                                                    .Include(c => c.News)
                                                        .Select(c => new Comment
                                                        {
                                                            Id = c.Id,
                                                            News = c.News,
                                                            Name = c.Name,
                                                            Description = c.Description,
                                                            PublishedDate = c.PublishedDate,
                                                        })
                                                        .ToListAsync();


            CategoryNewsVM model = new CategoryNewsVM
            {
                Comments = latestComments,
                News = newsModel,
                PopularNews = popularNews,
                RandomNews = randomNews,
            };
            return View(model);
        }
    }
}
