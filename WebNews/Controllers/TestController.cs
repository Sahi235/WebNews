using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebNews.Interfaces.NewsInterfaces;
using WebNews.Models;

namespace WebNews.Controllers
{
    public class TestController : Controller
    {
        private readonly INewsRepositories<News> newsRepositories;

        public TestController(INewsRepositories<News> newsRepositories)
        {
            this.newsRepositories = newsRepositories;
        }
        public async Task<IActionResult> Index()
        {
            var news = await newsRepositories.GetTByOrderByInList(c => c.Id);
            return View(news);
        }
    }
}