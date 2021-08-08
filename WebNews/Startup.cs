using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebNews.Installers;

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

            var installers = typeof(Startup).Assembly.ExportedTypes.Where(c => typeof(IInstaller).IsAssignableFrom(c) && !c.IsInterface && !c.IsAbstract).Select(Activator.CreateInstance).Cast<IInstaller>().ToList();
            installers.ForEach(installer => installer.InstallService(services, config));


            services.AddMvc(option =>
            {
                option.EnableEndpointRouting = false;
            });
            services.AddControllersWithViews();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseResponseCompression();
            app.UseRequestLocalization();
            app.UseStaticFiles();
            //app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();
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
