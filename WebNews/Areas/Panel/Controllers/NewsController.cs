using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using WebNews.Data;
using WebNews.Models;
using WebNews.Utilities;

namespace WebNews.Areas.Panel.Controllers
{
    [Area("Panel")]
    public class NewsController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly IImageHandler _imageHandler;

        public NewsController(DatabaseContext context,
                              IImageHandler imageHandler)
        {
            _context = context;
            _imageHandler = imageHandler;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("panel/news/cardindex")]
        [Route("panel/news/cardindex/{pageNumber}")]

        public async Task<IActionResult> CardIndex(int pageNumber = 1)
        {
            var news =  _context.News
                                  .AsNoTracking()
                                                .Select(c => new News
                                                {
                                                    Id = c.Id,
                                                    Title = c.Title,
                                                    MainImage = c.MainImage,
                                                    ViewCount = c.ViewCount,
                                                    IsPublished = c.IsPublished,
                                                    PublishedDate = c.PublishedDate,
                                                    ShortDescription = c.ShortDescription,
                                                })
                                                .OrderByDescending(c => c.PublishedDate)
                                                      .ThenBy(c => c.IsPublished);
            var model = await PagingList.CreateAsync(news, 15, pageNumber);
            model.PageParameterName = "pageNumber";
            return View(model);
        }

        [Route("panel/news/listindex")]
        [Route("panel/news/listindex/{take}/{skip}")]
        public async Task<IActionResult> ListIndex(int pageNumber = 1)
        {
            var news = _context.News
                                  .AsNoTracking()
                                       .Include(c => c.Comments)
                                               .Select(c => new News
                                               {
                                                   Id = c.Id,
                                                   Title = c.Title,
                                                   Comments = c.Comments,
                                                   MainImage = c.MainImage,
                                                   ViewCount = c.ViewCount,
                                                   IsPublished = c.IsPublished,
                                                   PublishedDate = c.PublishedDate,
                                                   ShortDescription = c.ShortDescription,
                                               })
                                               .OrderByDescending(c => c.PublishedDate)
                                                   .ThenBy(c => c.IsPublished);
            var model = await PagingList.CreateAsync(news, 50, pageNumber);
            model.PageParameterName = "pageNumber";
            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["Categories"] = new MultiSelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SeoUrl,Title,Body,ShortDescription,PublishedDate,IsPublished")] News news, string tags, IFormFile mainImage, List<IFormFile> images, int[] categoryId)
        {
            if (ModelState.IsValid)
            {
                var newSeoUrl2 = news.SeoUrl.Replace(" ", "-").Replace("'", "-").Replace(":", "-").Replace(";", "-");
                var newSeoUrl = newSeoUrl2.Replace("?", "-");

                string extension = null;
                string fileName = null;
                extension = "." + mainImage.FileName.Split('.')[mainImage.FileName.Split('.').Length - 1];
                fileName = Guid.NewGuid().ToString() + extension;
                await _imageHandler.UploadImage(mainImage, Constant.NewsFolder, fileName);
                news.MainImage = fileName;

                if (images != null)
                {
                    for (int i = 0; i < images.Count(); i++)
                    {
                        extension = "." + images[i].FileName.Split('.')[images[i].FileName.Split('.').Length - 1];
                        fileName = Guid.NewGuid().ToString() + extension;
                        await _imageHandler.UploadImage(images[i], Constant.NewsFolder, fileName);
                        NewsImage picture = new NewsImage
                        {
                            News = news,
                            NewsId = news.Id,
                            ImageUrl = fileName,
                        };
                        news.Image.Add(picture);
                    }
                }
                if (categoryId.Any())
                {
                    //ICollection<NewsCategory> newsCateAll = new List<NewsCategory>();
                    Category category;
                    foreach (var cate in categoryId)
                    {
                        category = await _context.Categories.FindAsync(cate);
                        NewsCategory newsCategory = new NewsCategory
                        {
                            News = news,
                            NewsId = news.Id,
                            Category = category,
                            CategoryId = category.Id,
                        };
                        news.Categories.Add(newsCategory);
                    }
                }
                ICollection<Tag> listOfTags = new List<Tag>();
                string[] arrayTag = tags.Split(",");

                if (tags.Any())
                {
                    Tag existedTag;
                    bool isTagExists = false;
                    foreach (var tag in arrayTag)
                    {
                        isTagExists = await _context.Tags.Where(c => c.Name == tag).AnyAsync();
                        if (isTagExists)
                        {
                            existedTag = await _context.Tags.Where(c => c.Name == tag).SingleOrDefaultAsync();
                            NewsTag newsTag1 = new NewsTag
                            {
                                News = news,
                                NewsId = news.Id,
                                Tag = existedTag,
                                TagId = existedTag.Id
                            };
                            news.Tags.Add(newsTag1);
                        }
                        else
                        {
                            Tag newTag = new Tag
                            {
                                Name = tag
                            };
                            listOfTags.Add(newTag);
                        }
                    }
                }
                ICollection<NewsTag> newsTag = new List<NewsTag>();
                if (listOfTags.Any())
                {
                    foreach (var tag in listOfTags)
                    {
                        NewsTag newNewsTag = new NewsTag
                        {
                            News = news,
                            NewsId = news.Id,
                            Tag = tag,
                            TagId = tag.Id
                        };
                        news.Tags.Add(newNewsTag);
                    }
                }

                news.SeoUrl = newSeoUrl;

                await _context.AddAsync(news);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(ListIndex));
            }
            ViewData["Categories"] = new MultiSelectList(await _context.Categories.ToListAsync(), "Id", "Name", news.Categories);
            return View(news);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                                        .Where(c => c.Id == id)
                                        .Include(c => c.Image)
                                        .Include(c => c.User)
                                        .Include(c => c.Modifier)
                                        .Include(c => c.Tags)
                                        .ThenInclude(c => c.Tag)
                                        .FirstOrDefaultAsync();


            var categories = from newsCate in await _context.NewsCategories
                             .AsNoTracking()
                             .ToListAsync()
                             where newsCate.NewsId == news.Id
                             select newsCate.CategoryId;


            var tags = from tag in await _context.NewsTags
                            .AsNoTracking()
                            .Include(c => c.Tag)
                            .ToListAsync()
                       where tag.NewsId == news.Id
                       select tag.Tag;
            string tagNames = null;
            int i = tags.Count();
            foreach (var tag in tags)
            {
                tagNames += tag.Name;
                if (i != 1)
                {
                    tagNames += ",";
                }
                i--;
            }
            ViewBag.TagNames = tagNames;

            ViewData["Categories"] = new MultiSelectList(await _context.Categories.ToListAsync(), "Id", "Name", categories.ToList());


            if (news == null)
            {
                return NotFound();
            }
            return View(news);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SeoUrl,Title,Body,ShortDescription,IsPublished")] News news, string tags, IFormFile mainImage, List<IFormFile> images, int[] categoryId)
        {
            if (id != news.Id)
            {
                return NotFound();
            }

            News changeNews = await _context.News
                                            .Where(c => c.Id == news.Id)
                                            .Include(c => c.Image)
                                            .Include(c => c.Categories)
                                            .ThenInclude(c => c.Category)
                                            .Include(c => c.Tags)
                                            .SingleOrDefaultAsync();
            if (changeNews == null) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    changeNews.Title = news.Title;
                    changeNews.Body = news.Body;
                    changeNews.ShortDescription = news.ShortDescription;
                    changeNews.IsPublished = news.IsPublished;
                    changeNews.IsModified = true;
                    changeNews.ModifiedDate = DateTime.Now;


                    string extension = null;
                    string fileName = null;
                    if (mainImage != null)
                    {
                        _imageHandler.RemoveImage(Constant.NewsFolder, changeNews.MainImage);
                        extension = "." + mainImage.FileName.Split('.')[mainImage.FileName.Split('.').Length - 1];
                        fileName = Guid.NewGuid().ToString() + extension;

                        await _imageHandler.UploadImage(mainImage, Constant.NewsFolder, fileName);
                        changeNews.MainImage = fileName;
                    }
                    if (images != null)
                    {
                        foreach (var pic in changeNews.Image)
                        {
                            _imageHandler.RemoveImage(Constant.NewsFolder, pic.ImageUrl);
                        }

                        for (int i = 0; i < images.Count; i++)
                        {
                            extension = "." + images[i].FileName.Split('.')[images[i].FileName.Split('.').Length - 1];
                            fileName = Guid.NewGuid().ToString() + extension;
                            await _imageHandler.UploadImage(images[i], Constant.NewsFolder, fileName);
                            NewsImage newsImage = new NewsImage
                            {
                                ImageUrl = fileName,
                                News = news,
                                NewsId = news.Id,
                            };
                            changeNews.Image.Add(newsImage);
                        }
                    }

                    if (changeNews.Categories.Any())
                    {
                        _context.NewsCategories.RemoveRange(changeNews.Categories);
                    }

                    Category category;
                    foreach (var cate in categoryId)
                    {
                        category = await _context.Categories.FindAsync(cate);
                        NewsCategory newsCategory = new NewsCategory
                        {
                            Category = category,
                            CategoryId = category.Id,
                            News = changeNews,
                            NewsId = changeNews.Id
                        };
                        changeNews.Categories.Add(newsCategory);
                    }
                    _context.NewsTags.RemoveRange(changeNews.Tags);

                    ICollection<Tag> listOfTags = new List<Tag>();
                    if (tags.Any())
                    {
                        string[] arrayTag = tags.Split(",");
                        Tag existedTag;
                        bool isTagExists = false;
                        foreach (var tag in arrayTag)
                        {
                            isTagExists = await _context.Tags.Where(c => c.Name == tag).AnyAsync();
                            if (isTagExists)
                            {
                                existedTag = await _context.Tags.Where(c => c.Name == tag).SingleOrDefaultAsync();
                                NewsTag newsTag1 = new NewsTag
                                {
                                    News = changeNews,
                                    NewsId = changeNews.Id,
                                    Tag = existedTag,
                                    TagId = existedTag.Id
                                };
                                changeNews.Tags.Add(newsTag1);
                            }
                            else
                            {
                                Tag newTag = new Tag
                                {
                                    Name = tag
                                };
                                listOfTags.Add(newTag);
                            }
                        }
                    }
                    ICollection<NewsTag> newsTag = new List<NewsTag>();
                    if (listOfTags.Any())
                    {
                        foreach (var tag in listOfTags)
                        {
                            NewsTag newNewsTag = new NewsTag
                            {
                                News = changeNews,
                                NewsId = changeNews.Id,
                                Tag = tag,
                                TagId = tag.Id
                            };
                            changeNews.Tags.Add(newNewsTag);
                        }
                    }
                    _context.Update(changeNews);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsExists(news.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ListIndex));
            }
            return View(news);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .Include(n => n.Modifier)
                .Include(n => n.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var news = await _context.News
                    .Where(c => c.Id == id)
                        .Include(c => c.Image)
                            .SingleOrDefaultAsync();
            foreach (var pic in news.Image)
            {
                _imageHandler.RemoveImage(Constant.NewsFolder, pic.ImageUrl);
            }
            _imageHandler.RemoveImage(Constant.NewsFolder, news.MainImage);
            _context.News.Remove(news);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListIndex));
        }

        private bool NewsExists(int id)
        {
            return _context.News.Any(e => e.Id == id);
        }

        public async Task<IActionResult> IsSeoUrlTaken(string seoUrl)
        {
            string newSeoUrl = seoUrl.Replace(" ", "-").Replace("?", "-");
            var news = await _context.News
                                        .Where(c => c.SeoUrl == newSeoUrl)
                                            .SingleOrDefaultAsync();
            if (news == null)
            {
                return Json(true);
            }
            else
            {
                return Json("This SeoUrl is taken");
            }
        }
    }
}