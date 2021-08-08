using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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

        public async Task<IActionResult> Index(int page = 1)
        {
            var comments = _context.Comments.Include(c => c.News).Select(c => new Comment
            {
                Id = c.Id,
                Name = c.Name,
                IsApproved = c.IsApproved,
                Description = c.Description,
                PublishedDate = c.PublishedDate,
                News = c.News
            }).OrderByDescending(c => c.PublishedDate);
            var model = await PagingList.CreateAsync(comments, 100, page);
            return View(model);
        }


        [HttpGet]
        [Route("[Area]/[Controller]/[Action]/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var comment = await _context.Comments.Include(c => c.News).Include(c => c.Gallery).SingleOrDefaultAsync(c => c.Id == id);
            if (comment == null) return NotFound();
            return View(comment);
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
                UnApprovedComments = unApproved,
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



        [HttpPost]
        public async Task<IActionResult> ApproveComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            comment.IsApproved = true;
            _context.Comments.Attach(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(NewsComments), new { id = comment.NewsId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(NewsComments), new { id = comment.NewsId });
        }

        [HttpPost]
        public async Task<IActionResult> AnswerComment(string answer, int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            comment.IsApproved = true;
            _context.Comments.Attach(comment);
            CommentAnswer commentAnswer = new CommentAnswer
            {
                Body = answer,
                PublishedDate = DateTime.Now,
                Comment = comment,
                CommentId = comment.Id,
                Name = "Admin",
            };
            await _context.CommentAnswers.AddAsync(commentAnswer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(NewsComments), new { id = comment.NewsId });
        }

        [HttpPost]
        public async Task<IActionResult> UnapproveComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            comment.IsApproved = false;
            _context.Comments.Attach(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(NewsComments), new { id = comment.NewsId });
        }


        [Route("[Area]/[Controller]/[Action]/{id}")]
        public async Task<IActionResult> DeleteCommentFromDashboard(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null) return NoContent();
            _context.Remove(comment);
            await _context.SaveChangesAsync();
            return StatusCode(200);
        }


        [Route("[Area]/[Controller]/[Action]/{id}")]
        public async Task<IActionResult> ApproveCommentFromDashboard(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null) return NoContent();
            comment.IsApproved = true;
            _context.Comments.Attach(comment);
            await _context.SaveChangesAsync();
            return StatusCode(200);
        }


        [Route("[Area]/[Controller]/[Action]/{id}")]
        public async Task<IActionResult> UnapproveCommentFromDashboard(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null) return NoContent();
            comment.IsApproved = false;
            _context.Comments.Attach(comment);
            await _context.SaveChangesAsync();
            return StatusCode(200);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCommentDetails(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            ViewData["Info"] = "Comment deleted";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UnapproveCommentDetails(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            comment.IsApproved = false;
            _context.Comments.Attach(comment);
            await _context.SaveChangesAsync();
            ViewData["Info"] = "Comment status changed to Unapproved";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ApproveCommentDetails(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            comment.IsApproved = true;
            _context.Comments.Attach(comment);
            await _context.SaveChangesAsync();
            ViewData["Info"] = "Comment status changed to Approved";
            return RedirectToAction(nameof(Index));
        }
    }
}
