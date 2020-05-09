using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using WebNews.Areas.Panel.ViewModels.Comments.Read;
using WebNews.Data;
using WebNews.Models;

namespace WebNews.Areas.Panel.Controllers
{
    [Area("Panel")]
    public class CommentsController : Controller
    {
        private readonly DatabaseContext _context;

        public CommentsController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var databaseContext = _context.Comments.Include(c => c.News).Include(c => c.Video);
            return View(await databaseContext.ToListAsync());
        }


        public async Task<IActionResult> NewsComments(int id, int pageNumberUnapproved = 1, int PageNumberApproved = 1)
        {


            var upApprovedComments = _context.Comments
                                                    .Where(c => c.NewsId == id && c.IsApproved == false)
                                                        .Select(c => new Comment
                                                        {
                                                            Id = c.Id,
                                                            Name = c.Name,
                                                            NewsId = c.NewsId,
                                                            IsApproved = c.IsApproved,
                                                            Description = c.Description,
                                                            PublishedDate = c.PublishedDate,
                                                        })
                                                        .OrderByDescending(c => c.PublishedDate);
            var unApproved = await PagingList.CreateAsync(upApprovedComments, 5, pageNumberUnapproved);




            var approvedComments = _context.Comments
                                                .Where(c => c.NewsId == id && c.IsApproved == true)
                                                    .Select(c => new Comment
                                                    {
                                                        Id = c.Id,
                                                        Name = c.Name,
                                                        NewsId = c.NewsId,
                                                        IsApproved = c.IsApproved,
                                                        Description = c.Description,
                                                        PublishedDate = c.PublishedDate,
                                                    })
                                                    .OrderByDescending(c => c.PublishedDate);
            var approved = await PagingList.CreateAsync(approvedComments, 5, PageNumberApproved);



            NewsCommentsVM model = new NewsCommentsVM
            {
                UnApprovedComments =  unApproved,
                ApprovedComments = approved
            };
            ViewData["CommentsCount"] = await upApprovedComments.CountAsync() + approvedComments.Count();
            return View(model);
        }


        public async Task<IActionResult> GalleryComments(int id,
                                                         int pageNumberUnapproved = 1,
                                                         int PageNumberApproved = 1)
        {
            var upApprovedComments = _context.Comments
                                                    .Where(c => c.GalleryId == id && c.IsApproved == false)
                                                        .Select(c => new Comment
                                                        {
                                                            Id = c.Id,
                                                            Name = c.Name,
                                                            NewsId = c.NewsId,
                                                            IsApproved = c.IsApproved,
                                                            Description = c.Description,
                                                            PublishedDate = c.PublishedDate,
                                                        })
                                                        .OrderByDescending(c => c.PublishedDate);
            var unApproved = await PagingList.CreateAsync(upApprovedComments, 5, pageNumberUnapproved);




            var approvedComments = _context.Comments
                                                .Where(c => c.GalleryId == id && c.IsApproved == true)
                                                    .Select(c => new Comment
                                                    {
                                                        Id = c.Id,
                                                        Name = c.Name,
                                                        NewsId = c.NewsId,
                                                        IsApproved = c.IsApproved,
                                                        Description = c.Description,
                                                        PublishedDate = c.PublishedDate,
                                                    })
                                                    .OrderByDescending(c => c.PublishedDate);
            var approved = await PagingList.CreateAsync(approvedComments, 5, PageNumberApproved);



            GalleryCommentsVM model = new GalleryCommentsVM
            {
                UnApprovedComments = unApproved,
                ApprovedComments = approved
            };
            ViewData["CommentsCount"] = await upApprovedComments.CountAsync() + approvedComments.Count();
            return View(model);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.News)
                .Include(c => c.Video)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["NewsId"] = new SelectList(_context.News, "Id", "SeoUrl", comment.NewsId);
            ViewData["VideoId"] = new SelectList(_context.Videos, "Id", "Id", comment.VideoId);
            return View(comment);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,PublishedDate,IsApproved,NewsId,VideoId")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
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
            ViewData["NewsId"] = new SelectList(_context.News, "Id", "SeoUrl", comment.NewsId);
            ViewData["VideoId"] = new SelectList(_context.Videos, "Id", "Id", comment.VideoId);
            return View(comment);
        }


        // POST: Panel/Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
