using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebNews.Data;
using WebNews.Models;
using WebNews.Utilities;

namespace WebNews.Areas.Panel.Controllers
{
    [Area("Panel")]
    public class VideosController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly IImageHandler _imageHandler;

        public VideosController(DatabaseContext context,
                                IImageHandler imageHandler)
        {
            _context = context;
            _imageHandler = imageHandler;
        }

        public async Task<IActionResult> Index()
        {
            var databaseContext = _context.Videos.Include(v => v.User);
            return View(await databaseContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var video = await _context.Videos
                .Include(v => v.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (video == null)
            {
                return NotFound();
            }

            return View(video);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["Categories"] = new MultiSelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SeoUrl,Title,MainPage,Description,PublishedDate,IsPublished")] Video video, IFormFile mainVideo, List<IFormFile> videos, IFormFile mainImage, List<int> categoryId, string tags)
        {
            ViewData["Categories"] = new MultiSelectList(await _context.Categories.ToListAsync(), "Id", "Name", categoryId);

            if (ModelState.IsValid)
            {
                if (mainImage == null || mainVideo == null)
                {
                    return View();
                }




                video.SeoUrl = video.SeoUrl.Replace(" ", "-").Replace("?", "-");
                string extension = "." + mainImage.FileName.Split('.')[mainImage.FileName.Split('.').Length - 1];
                string fileName = video.SeoUrl + extension;
                await _imageHandler.UploadImage(mainImage, Constant.VideoImagesFolder, fileName);
                video.ImageUrl = fileName;





                extension = "." + mainVideo.FileName.Split('.')[mainVideo.FileName.Split('.').Length - 1];
                fileName = video.SeoUrl + extension;
                await _imageHandler.UploadImage(mainVideo, Constant.VideoFolder, fileName);
                video.VideoUrl = fileName;





                if (videos.Any())
                {
                    for (int i = 0; i < videos.Count(); i++)
                    {
                        extension = "." + videos[i].FileName.Split('.')[videos[i].FileName.Split('.').Length - 1];
                        fileName = video.SeoUrl + (i + 2).ToString() + extension;
                        await _imageHandler.UploadImage(videos[i], Constant.VideoFolder, fileName);
                        VideoUrl videoUrl = new VideoUrl
                        {
                            Url = fileName,
                            Video = video,
                            VideoId = video.Id
                        };
                        video.Videos.Add(videoUrl);
                    }
                }




                if (categoryId.Any())
                {
                    Category category;
                    foreach (var cate in categoryId)
                    {
                        category = await _context.Categories.FindAsync(cate);
                        if (category == null)
                        {
                            return View();
                        }
                        VideoCategory videoCategory = new VideoCategory
                        {
                            Category = category,
                            CategoryId = category.Id,
                            Video = video,
                            VideoId = video.Id
                        };
                        video.Categories.Add(videoCategory);
                    }
                }




                if (tags != null)
                {
                    string[] tagNames = tags.Split(",");
                    Tag tag1;
                    foreach (var tag in tagNames)
                    {
                        bool tagExists = await _context.Tags.Where(c => c.Name == tag).AnyAsync();
                        if (tagExists)
                        {
                            tag1 = await _context.Tags.Where(c => c.Name == tag).SingleOrDefaultAsync();
                            VideoTag videoTag = new VideoTag
                            {
                                Tag = tag1,
                                TagId = tag1.Id,
                                Video = video,
                                VideoId = video.Id
                            };
                            video.Tags.Add(videoTag);
                        }
                        else
                        {
                            Tag newTag = new Tag
                            {
                                Name = tag,
                            };
                            VideoTag videoTag = new VideoTag
                            {
                                Tag = newTag,
                                TagId = newTag.Id,
                                Video = video,
                                VideoId = video.Id
                            };
                            video.Tags.Add(videoTag);
                        }
                    }
                }




                _context.Add(video);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", video.UserId);
            return View(video);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var video = await _context.Videos.FindAsync(id);
            if (video == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", video.UserId);
            return View(video);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,MainPage,VideoUrl,Description,PublishedDate,IsPublished,UserId")] Video video)
        {
            if (id != video.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(video);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VideoExists(video.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", video.UserId);
            return View(video);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var video = await _context.Videos
                .Include(v => v.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (video == null)
            {
                return NotFound();
            }

            return View(video);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var video = await _context.Videos.FindAsync(id);
            _context.Videos.Remove(video);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VideoExists(int id)
        {
            return _context.Videos.Any(e => e.Id == id);
        }
    }
}
