using System;
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
    public class ArticlesController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly IImageHandler _imageHandler;


        public ArticlesController(DatabaseContext context,
                                  IImageHandler imageHandler)
        {
            _context = context;
            _imageHandler = imageHandler;
        }

        // GET: Panel/Articles
        public async Task<IActionResult> Index()
        {
            var databaseContext = _context.Articles.Include(a => a.Modifier).Include(a => a.User);
            return View(await databaseContext.ToListAsync());
        }




        [Route("panel/articles/cardindex")]
        [Route("panel/articles/cardindex/{pageNumber}")]
        public async Task<IActionResult> CardIndex(int pageNumber = 1)
        {
            var articles = _context.Articles
                                  .AsNoTracking()
                                                .Select(c => new Article
                                                {
                                                    Id = c.Id,
                                                    Title = c.Title,
                                                    ShortBody = c.ShortBody,
                                                    MainImage = c.MainImage,
                                                    ViewCount = c.ViewCount,
                                                    IsPublished = c.IsPublished,
                                                    PublishedDate = c.PublishedDate,

                                                })
                                                .OrderByDescending(c => c.PublishedDate)
                                                    .ThenBy(c => c.IsPublished);
            var model = await PagingList.CreateAsync(articles, 20, pageNumber);
            model.PageParameterName = "pageNumber";

            return View(model);
        }


        [Route("panel/articles/listindex")]
        [Route("panel/articles/listindex/{pageNumber}")]
        public async Task<IActionResult> ListIndex(int pageNumber = 1)
        {
            var articles = _context.Articles
                                        .AsNoTracking()
                                                    .Select(c => new Article
                                                    {
                                                        Id = c.Id,
                                                        Title = c.Title,
                                                        MainImage = c.MainImage,
                                                        ViewCount = c.ViewCount,
                                                        IsPublished = c.IsPublished,
                                                        PublishedDate = c.PublishedDate,
                                                        ShortBody = c.ShortBody,
                                                    })
                                                    .OrderByDescending(c => c.PublishedDate)
                                                           .ThenBy(c => c.IsPublished);
            var model = await PagingList.CreateAsync(articles, 50, pageNumber);
            return View(model);
        }
        public async Task<IActionResult> Create()
        {
            ViewData["Categories"] = new MultiSelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SeoUrl,Title,Body,ShortBody,PublishedDate,IsPublished")] Article article, int[] categoryId, IFormFile mainImage, List<IFormFile> images, string tags)
        {
            ViewData["Categories"] = new MultiSelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            if (ModelState.IsValid)
            {
                var newSeoUrl = article.SeoUrl.Replace(" ", "-").Replace("?", "-");
                string extension = null;
                string fileName = null;
                extension = "." + mainImage.FileName.Split('.')[mainImage.FileName.Split('.').Length - 1];
                fileName = newSeoUrl + "-" + extension;
                await _imageHandler.UploadImage(mainImage, Constant.ArticleFolder, fileName);
                article.MainImage = fileName;

                if (images != null)
                {
                    for (int i = 0; i < images.Count(); i++)
                    {
                        extension = "." + images[i].FileName.Split('.')[images[i].FileName.Split('.').Length - 1];
                        fileName = newSeoUrl + "-" + (i + 1).ToString() + extension;
                        await _imageHandler.UploadImage(images[i], Constant.ArticleFolder, fileName);
                        ArticleImage picture = new ArticleImage
                        {
                            Article = article,
                            ArticleId = article.Id,
                            ImageUrl = fileName,
                        };
                        article.Images.Add(picture);
                    }
                }

                if (categoryId.Any())
                {
                    Category category;
                    foreach (var cate in categoryId)
                    {
                        category = await _context.Categories.FindAsync(cate);
                        ArticleCategory newsCategory = new ArticleCategory
                        {
                            Article = article,
                            ArticleId = article.Id,
                            Category = category,
                            CategoryId = category.Id,
                        };
                        article.Categories.Add(newsCategory);
                    }
                }

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
                            ArticleTag newsTag1 = new ArticleTag
                            {
                                Article = article,
                                ArticleId = article.Id,
                                Tag = existedTag,
                                TagId = existedTag.Id
                            };
                            article.Tags.Add(newsTag1);
                        }
                        else
                        {
                            Tag newTag = new Tag
                            {
                                Name = tag
                            };
                            ArticleTag articleTag = new ArticleTag
                            {
                                Article = article,
                                ArticleId = article.Id,
                                Tag = newTag,
                                TagId = newTag.Id
                            };
                            article.Tags.Add(articleTag);
                        }
                    }
                }
                article.SeoUrl = newSeoUrl;
                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ListIndex));
            }
            return View(article);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                                        .Where(c => c.Id == id)
                                        .Include(c => c.Images)
                                        .Include(c => c.User)
                                        .Include(c => c.Modifier)
                                        .Include(c => c.Tags)
                                        .ThenInclude(c => c.Tag)
                                        .FirstOrDefaultAsync();
            if (article == null)
            {
                return NotFound();
            }


            var categories = from articleCate in await _context.ArticleCategories
                             .AsNoTracking()
                             .ToListAsync()
                             where articleCate.ArticleId == article.Id
                             select articleCate.CategoryId;


            var tags = from tag in await _context.ArticleTags
                            .AsNoTracking()
                            .Include(c => c.Tag)
                            .ToListAsync()
                            where tag.ArticleId == article.Id
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
            return View(article);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SeoUrl,Title,Body,ShortBody,IsPublished,IsApproved")] Article article, string tags, IFormFile mainImage, List<IFormFile> images, int[] categoryId)
        {
            if (id != article.Id)
            {
                return NotFound();
            }


            Article changedArticle = await _context.Articles
                                            .Where(c => c.Id == article.Id)
                                            .Include(c => c.Images)
                                            .Include(c => c.Categories)
                                            .ThenInclude(c => c.Category)
                                            .Include(c => c.Tags)
                                            .SingleOrDefaultAsync();

            if (changedArticle == null) return View();


            if (ModelState.IsValid)
            {
                try
                {
                    changedArticle.Title = article.Title;
                    changedArticle.Body = article.Body;
                    changedArticle.ShortBody = article.ShortBody;
                    changedArticle.IsPublished = article.IsPublished;
                    changedArticle.IsModified = true;
                    changedArticle.ModifiedDate = DateTime.Now;
                    changedArticle.IsApproved = article.IsApproved;


                    string extension = null;
                    string fileName = null;
                    if (mainImage != null)
                    {
                        _imageHandler.RemoveImage(Constant.ArticleFolder, changedArticle.MainImage);
                        extension = "." + mainImage.FileName.Split('.')[mainImage.FileName.Split('.').Length - 1];
                        fileName = changedArticle.SeoUrl + "-" + extension;
                        await _imageHandler.UploadImage(mainImage, Constant.ArticleFolder, fileName);
                        changedArticle.MainImage = fileName;
                    }
                    if (images != null)
                    {
                        foreach (var pic in changedArticle.Images)
                        {
                            _imageHandler.RemoveImage(Constant.ArticleFolder, pic.ImageUrl);
                        }

                        for (int i = 0; i < images.Count; i++)
                        {
                            extension = "." + images[i].FileName.Split('.')[images[i].FileName.Split('.').Length - 1];
                            fileName = changedArticle.SeoUrl + "-" + (i + 1).ToString() + extension;
                            await _imageHandler.UploadImage(images[i], Constant.ArticleFolder, fileName);
                            ArticleImage articleImage = new ArticleImage
                            {
                                ImageUrl = fileName,
                                Article = changedArticle,
                                ArticleId = changedArticle.Id,
                            };
                            changedArticle.Images.Add(articleImage);
                        }
                    }

                    if (changedArticle.Categories.Any())
                    {
                        _context.ArticleCategories.RemoveRange(changedArticle.Categories);
                    }

                    Category category;
                    foreach (var cate in categoryId)
                    {
                        category = await _context.Categories.FindAsync(cate);
                        ArticleCategory articleCategory = new ArticleCategory
                        {
                            Category = category,
                            CategoryId = category.Id,
                            Article = changedArticle,
                            ArticleId = changedArticle.Id
                        };
                        changedArticle.Categories.Add(articleCategory);
                    }
                    _context.ArticleTags.RemoveRange(changedArticle.Tags);

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
                                ArticleTag articleTag1 = new ArticleTag
                                {
                                    Article = changedArticle,
                                    ArticleId = changedArticle.Id,
                                    Tag = existedTag,
                                    TagId = existedTag.Id
                                };
                                changedArticle.Tags.Add(articleTag1);
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

                    if (listOfTags.Any())
                    {
                        foreach (var tag in listOfTags)
                        {
                            ArticleTag newNewsTag = new ArticleTag
                            {
                                Article = changedArticle,
                                ArticleId = changedArticle.Id,
                                Tag = tag,
                                TagId = tag.Id
                            };
                            changedArticle.Tags.Add(newNewsTag);
                        }
                    }
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.Id))
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
            ViewData["Categories"] = new MultiSelectList(await _context.Categories.ToListAsync(), "Id", "Name", categoryId);
            return View(article);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Articles
                                            .Where(c => c.Id == id)
                                                .Include(c => c.Images)
                                                    .SingleOrDefaultAsync();
            foreach (var pic in article.Images)
            {
                _imageHandler.RemoveImage(Constant.ArticleFolder, pic.ImageUrl);
            }
            _imageHandler.RemoveImage(Constant.ArticleFolder, article.MainImage);
            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListIndex));
        }


        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.Id == id);
        }



        public async Task<IActionResult> IsSeoUrlTaken(string seoUrl)
        {
            var newSeoUrl = seoUrl.Replace(" ", "-").Replace("?", "-");
            bool IsSeoUrlTaken = await _context.Articles.Where(c => c.SeoUrl == newSeoUrl).AnyAsync();

            if (IsSeoUrlTaken)
            {
                return Json("This SeoUrl is Taken");
            }
            else
            {
                return Json(true);
            }
        }
    }
}
