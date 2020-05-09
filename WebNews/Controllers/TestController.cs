using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Controllers
{
    public class TestController : Controller
    {
        private readonly IActionContextAccessor accessor;

        public TestController(IActionContextAccessor accessor)
        {
            this.accessor = accessor;
        }


        public IActionResult Test()
        {
            ViewData["ip"] = accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString();
            return View();
        }
    }
}
