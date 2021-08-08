using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    public class GalleriesController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly IImageHandler _imageHandler;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ISeoUrlEditor seoUrlEditor;

        public GalleriesController(DatabaseContext context,
                                   IImageHandler imageHandler,
                                   UserManager<ApplicationUser> userManager,
                                   ISeoUrlEditor seoUrlEditor)
        {
            _context = context;
            _imageHandler = imageHandler;
            this.userManager = userManager;
            this.seoUrlEditor = seoUrlEditor;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("panel/galleries/cardindex")]
        [Route("panel/galleries/cardindex/{pageNumber}")]
        public async Task<IActionResult> CardIndex(int pageNumber = 1)
        {
            var galleries =  _context.Galleries
                                            .AsNoTracking()
                                                .Select(c => new Gallery
                                                {
                                                    Id = c.Id,
                                                    Title = c.Title,
                                                    MainImage = c.MainImage,
                                                    IsPublished = c.IsPublished,
                                                    PublishedDate = c.PublishedDate,
                                                })
                                                    .OrderByDescending(c => c.PublishedDate)
                                                        .ThenByDescending(c => c.IsPublished);
            var model = await PagingList.CreateAsync(galleries, 20, pageNumber);
            model.PageParameterName = "pageNumber";
            return View(model);
        }


        [Route("panel/galleries/listindex")]
        [Route("panel/galleries/listindex/{pageNumber}")]
        public async Task<IActionResult> ListIndex(int pageNumber = 1)
        {
            var galleries =  _context.Galleries
                                            .AsNoTracking()
                                                .Select(c => new Gallery
                                                {
                                                    Id = c.Id,
                                                    Title = c.Title,
                                                    MainImage = c.MainImage,
                                                    IsPublished = c.IsPublished,
                                                    PublishedDate = c.PublishedDate
                                                })
                                                .OrderByDescending(c => c.PublishedDate)
                                                .ThenByDescending(c => c.IsPublished);
            var model = await PagingList.CreateAsync(galleries, 50, pageNumber);
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
        public async Task<IActionResult> Create([Bind("Id,SeoUrl,Title,Description,PublishedDate,IsPublished")] Gallery gallery, IFormFile mainImage, List<IFormFile> images, int[] categoryId, string tags)
        {
            ViewData["Categories"] = new MultiSelectList(await _context.Categories.ToListAsync(), "Id", "Name", categoryId);
            if (ModelState.IsValid)
            {
                gallery.SeoUrl = seoUrlEditor.FixSeoUrl(gallery.SeoUrl);
                if (mainImage == null) return View();
                string extension = null;
                string fileName = null;

                extension = "." + mainImage.FileName.Split('.')[mainImage.FileName.Split('.').Length - 1];
                fileName = gallery.SeoUrl + extension;
                await _imageHandler.UploadImage(mainImage, Constant.GalleryFolder, fileName);
                gallery.MainImage = fileName;

                for (int i = 0; i < images.Count(); i++)
                {
                    extension = "." + images[i].FileName.Split('.')[images[i].FileName.Split('.').Length - 1];
                    fileName = gallery.SeoUrl + (i + 2).ToString() + extension;
                    await _imageHandler.UploadImage(images[i], Constant.GalleryFolder, fileName);
                    GalleryImage galleryImage = new GalleryImage
                    {
                        Gallery = gallery,
                        GalleryId = gallery.Id,
                        ImageUrl = fileName,
                    };
                    gallery.Images.Add(galleryImage);
                }

                Category category;
                foreach (var cate in categoryId)
                {
                    category = await _context.Categories.FindAsync(cate);
                    GalleryCategory galleryCategory = new GalleryCategory
                    {
                        Category = category,
                        CategoryId = category.Id,
                        Gallery = gallery,
                        GalleryId = gallery.Id
                    };
                    gallery.Categories.Add(galleryCategory);
                }



                //ICollection<Tag> listOfTags = new List<Tag>();
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
                            GalleryTag newsTag1 = new GalleryTag
                            {
                                Gallery = gallery,
                                GalleryId = gallery.Id,
                                Tag = existedTag,
                                TagId = existedTag.Id
                            };
                            gallery.Tags.Add(newsTag1);
                        }
                        else
                        {
                            Tag newTag = new Tag
                            {
                                Name = tag
                            };
                            GalleryTag newNewsTag = new GalleryTag
                            {
                                Gallery = gallery,
                                GalleryId = gallery.Id,
                                Tag = newTag,
                                TagId = newTag.Id
                            };
                            gallery.Tags.Add(newNewsTag);
                        }
                    }
                }
                var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                gallery.User = user;
                gallery.UserId = user.Id;
                await _context.AddAsync(gallery);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ListIndex));
            }
            return View(gallery);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gallery = await _context.Galleries
                                        .Include(g => g.User)
                                        .FirstOrDefaultAsync(m => m.Id == id);



            var categoryId = from cate in await _context.GalleryCategories
                             .Include(c => c.Category)
                             .ToListAsync()
                             where cate.GalleryId == id
                             select cate.CategoryId;


            var tags = from tag in await _context.GalleryTags
                       .Include(c => c.Tag)
                       .ToListAsync()
                       where tag.GalleryId == id
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

            ViewData["Categories"] = new MultiSelectList(await _context.Categories.ToListAsync(), "Id", "Name", categoryId);

            if (gallery == null)
            {
                return NotFound();
            }
            return View(gallery);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IsPublished,Title,Description")] Gallery gallery, int[] categoryId, IFormFile mainImage, List<IFormFile> images, string tags)
        {
            if (id != gallery.Id)
            {
                return NotFound();
            }


            ViewData["Categories"] = new MultiSelectList(await _context.Categories.ToListAsync(), "Id", "Name", categoryId);

            var existingGallery = await _context.Galleries
                                                    .Where(c => c.Id == id)
                                                        .Include(c => c.Tags)
                                                            .Include(c => c.Categories)
                                                                .SingleOrDefaultAsync();
            existingGallery.Title = gallery.Title;
            existingGallery.Description = gallery.Description;
            existingGallery.IsModified = true;
            existingGallery.IsPublished = gallery.IsPublished;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.GalleryTags.RemoveRange(existingGallery.Tags);
                    if (tags.Any())
                    {
                        Tag tag1;
                        bool tagExists;
                        string[] listOfTasg = tags.Split(",");
                        foreach (var tag in listOfTasg)
                        {
                            tagExists = await _context.Tags.Where(c => c.Name == tag).AnyAsync();
                            if (tagExists)
                            {
                                tag1 = await _context.Tags
                                        .Where(c => c.Name == tag)
                                            .SingleOrDefaultAsync();
                                GalleryTag galleryTag = new GalleryTag
                                {
                                    Gallery = existingGallery,
                                    GalleryId = existingGallery.Id,
                                    Tag = tag1,
                                    TagId = tag1.Id
                                };
                                existingGallery.Tags.Add(galleryTag);
                            }
                            else
                            {
                                Tag newTag = new Tag
                                {
                                    Name = tag
                                };
                                GalleryTag galleryTag = new GalleryTag
                                {
                                    Gallery = existingGallery,
                                    GalleryId = existingGallery.Id,
                                    Tag = newTag,
                                    TagId = newTag.Id
                                };
                                existingGallery.Tags.Add(galleryTag);
                            }
                        }
                    }
                    string extension = null;
                    string fileName = null;
                    if (mainImage != null)
                    {
                        _imageHandler.RemoveImage(Constant.GalleryFolder, existingGallery.MainImage);
                        extension = "." + mainImage.FileName.Split('.')[mainImage.FileName.Split('.').Length - 1];
                        fileName = existingGallery.SeoUrl + extension;
                        await _imageHandler.UploadImage(mainImage, Constant.GalleryFolder, fileName);
                        existingGallery.MainImage = fileName;
                    }

                    if (images.Any())
                    {
                        for (int i = 0; i < images.Count(); i++)
                        {
                            extension = "." + images[i].FileName.Split('.')[images[i].FileName.Split('.').Length - 1];
                            fileName = existingGallery.SeoUrl + "-" + (i + 2).ToString() + extension;
                            GalleryImage galleryImage = new GalleryImage
                            {
                                Gallery = existingGallery,
                                GalleryId = existingGallery.Id,
                                ImageUrl = fileName,
                            };
                            existingGallery.Images.Add(galleryImage);
                        }
                    }


                    _context.GalleryCategories.RemoveRange(existingGallery.Categories);
                    if (categoryId.Any())
                    {
                        Category category;
                        foreach (var cate in categoryId)
                        {
                            category = await _context.Categories.FindAsync(cate);
                            if (category == null) return View();
                            GalleryCategory galleryCategory = new GalleryCategory
                            {
                                Category = category,
                                CategoryId = category.Id,
                                Gallery = existingGallery,
                                GalleryId = existingGallery.Id,
                            };
                            existingGallery.Categories.Add(galleryCategory);
                        }
                    }
                    var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                    existingGallery.IsModified = true;
                    existingGallery.Modifier = user;
                    existingGallery.ModifierId = user.Id;
                    _context.Update(existingGallery);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GalleryExists(existingGallery.Id))
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
            return View(gallery);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gallery = await _context.Galleries
                                            .Where(c => c.Id == id)
                                                .Include(c => c.Images)
                                                    .SingleOrDefaultAsync();
            foreach (var pic in gallery.Images)
            {
                _imageHandler.RemoveImage(Constant.GalleryFolder, pic.ImageUrl);
            }
            _imageHandler.RemoveImage(Constant.GalleryFolder, gallery.MainImage);

            _context.Galleries.Remove(gallery);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListIndex));
        }

        private bool GalleryExists(int id)
        {
            return _context.Galleries.Any(e => e.Id == id);
        }

        public async Task<IActionResult> IsSeoUrlTaken(string seoUrl)
        {
            bool seo = await _context.Galleries.Where(c => c.SeoUrl == seoUrl.Replace(" ", "-").Replace("?", "-")).AnyAsync();
            if (seo)
            {
                return Json("This SeoUrl is taken");
            }
            else
            {
                return Json(true);
            }
        }
    }
}
