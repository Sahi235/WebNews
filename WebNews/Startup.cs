using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReflectionIT.Mvc.Paging;
using WebNews.Data;
using WebNews.Models;
using WebNews.Utilities;

namespace WebNews
{
    public class Startup
    {
        private readonly IConfiguration config;

        public Startup(IConfiguration config)
        {
            this.config = config;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddControllersWithViews();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddMvc(option => option.EnableEndpointRouting = false);


            services.AddDbContext<DatabaseContext>(option => 
            {
                option.UseSqlServer(config.GetConnectionString("DefaultConnection"), opt => 
                {
                    opt.EnableRetryOnFailure(2);
                    opt.MaxBatchSize(500);
                });
            });



            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<DatabaseContext>()
                .AddDefaultTokenProviders();


            services.AddTransient<IImageHandler, ImageHandler>();
            services.AddTransient<IImageWriter, ImageWriter>();

            services.AddPaging(options =>
            {
                options.ViewName = "Bootstrap4";
                options.HtmlIndicatorDown = "<span>&darr;</span>";
                options.HtmlIndicatorUp = " <span>&uarr;</span>";
            });
            services.AddMiniProfiler(c =>
            {
                c.PopupRenderPosition = StackExchange.Profiling.RenderPosition.BottomRight;
                c.PopupShowTimeWithChildren = true;
                c.UserIdProvider = (request) => request.HttpContext.User.Identity.Name;
            }).AddEntityFramework();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseMiniProfiler();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                        name: "Panel",
                            template: "{area:exists}/{controller=Panel}/{action=Index}/{id?}");
                routes.MapRoute(
                        name: "default",
                            template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
