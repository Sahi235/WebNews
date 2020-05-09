using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebNews.Data;

namespace WebNews.ViewComponents
{
    public class CategoriesViewComponent : ViewComponent
    {
        private readonly DatabaseContext context;

        public CategoriesViewComponent(DatabaseContext context)
        {
            this.context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await context.Categories
                            .AsNoTracking()
                                .Include(c => c.News)
                                    .OrderByDescending(c => c.News.Count())
                                        .ToListAsync();
            return View(categories);
        }
    }
}
