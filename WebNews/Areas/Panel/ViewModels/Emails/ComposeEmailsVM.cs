using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebNews.Areas.Panel.Controllers;

namespace WebNews.Areas.Panel.ViewModels.Emails
{
    public class ComposeEmailsVM
    {
        [MaxLength(255)]
        public string Subject { get; set; }
        public string Body { get; set; }
        [Remote(action:"IsReceiverExists", controller: "Emails", areaName: "Panel", ErrorMessage = "User does not exist")]
        public string Receiver { get; set; }
        public List<IFormFile> Attachments { get; set; } = new List<IFormFile>();
    }
}
