using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using ReflectionIT.Mvc.Paging;
using WebNews.Areas.Panel.ViewModels.Emails;
using WebNews.Data;
using WebNews.Models;
using WebNews.Utilities;

namespace WebNews.Areas.Panel.Controllers
{
    [Area("Panel")]
    public class EmailsController : Controller
    {
        private readonly DatabaseContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IImageHandler imageHandler;

        public EmailsController(DatabaseContext context,
                                UserManager<ApplicationUser> userManager,
                                IImageHandler imageHandler)
        {
            this.context = context;
            this.userManager = userManager;
            this.imageHandler = imageHandler;
        }


        [Route("[Area]/[Controller]/[Action]")]
        [Route("[Area]/[Controller]/[Action]/{page}")]

        public async Task<IActionResult> Inbox(int page = 1)
        {
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var emails = context.Emails.AsNoTracking()
                .Select(c => new Email
                {
                    Id = c.Id,
                    Subject = c.Subject,
                    IsRead = c.IsRead,
                    SentDate = c.SentDate,
                    ReceiverId = c.ReceiverId,
                    Sender = userManager.Users.Select(e => new ApplicationUser
                    {
                        Id = e.Id,
                        UserName = e.UserName
                    }).SingleOrDefault(e => e.Id == c.SenderId)
                }).Where(c => c.ReceiverId == user.Id).OrderByDescending(c => c.SentDate);
            var model = await PagingList.CreateAsync(emails, 12, page);
            return View(model);
        }

        [HttpGet]
        public IActionResult Compose()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Compose([Bind("Id,Subject,Body,Receiver,Attachments")] ComposeEmailsVM model)
        {
            var sender = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var receiver = await userManager.FindByNameAsync(model.Receiver);
            if (receiver == null)
            {
                ModelState.AddModelError(string.Empty, "Receiver was not found");
                return View(model);
            }
            if (sender == null) return NotFound();

            Email email = new Email()
            {
                Sender = sender,
                Receiver = receiver,
                SenderId = sender.Id,
                ReceiverId = receiver.Id,
                SentDate = DateTime.Now,
                Subject = model.Subject,
                Body = model.Body,
                IsRead = false,
            };
            if (model.Attachments.Any())
            {
                ICollection<Attachments> attachments = new List<Attachments>();
                string fileName;
                foreach (var file in model.Attachments)
                {
                    string extension = "." + file.FileName.Split('.')[^1];
                    fileName = Guid.NewGuid().ToString() + extension;
                    await imageHandler.UploadImage(file, Constant.AttachmentFolder, fileName);
                    Attachments attachment = new Attachments
                    {
                        AttachmentUrl = fileName,
                        Email = email,
                        EmailId = email.Id,
                    };
                    attachments.Add(attachment);
                }
                await context.Attachments.AddRangeAsync(attachments);
                email.Attachments = attachments;
            }
            await context.AddAsync(email);
            sender.SentMessages.Add(email);
            receiver.ReceivedEmail.Add(email);
            await userManager.UpdateAsync(receiver);
            await userManager.UpdateAsync(sender);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Inbox));
        }


        [Route("[Area]/[Controller]/[Action]/{id?}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Inbox));
            }
            var email = await context.Emails.AsNoTracking()
                .Select(c => new Email
                {
                    Id = c.Id,
                    Body = c.Body,
                    Sender = userManager.Users
                    .Select(e => new ApplicationUser
                    {
                        Id = e.Id,
                        UserName = e.UserName,
                    }).SingleOrDefault(r => r.Id == c.SenderId),
                    IsRead = c.IsRead,
                    SentDate = c.SentDate,
                    Subject = c.Subject,
                    Attachments = context.Attachments.Where(t => t.EmailId == c.Id).ToList(),
                    Answers = context.EmailAnswers.Where(t => t.EmailId == c.Id).ToList(),
                }).SingleOrDefaultAsync(c => c.Id == id);

            var isReadChaneEmail = await context.Emails.FindAsync(email.Id);
            isReadChaneEmail.IsRead = true;
            context.Emails.Update(isReadChaneEmail);
            await context.SaveChangesAsync();

            if (email == null)
            {
                return RedirectToAction(nameof(Inbox));
            }
            return View(email);
        }


        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsReceiverExists(string Receiver)
        {
            var user = await userManager.FindByNameAsync(Receiver);
            if (user == null)
            {
                return Json("User doesnt exists");
            }
            else
            {
                return Json(true);
            }
        }
    }
}