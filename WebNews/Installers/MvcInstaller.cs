using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReflectionIT.Mvc.Paging;
using WebNews.Data;
using WebNews.Interfaces.ArticleInterface;
using WebNews.Interfaces.GalleryInterface;
using WebNews.Interfaces.NewsInterfaces;
using WebNews.Interfaces.VideoSQLRepository;
using WebNews.Models;
using WebNews.SQLRepositories.ArticleSQLRepository;
using WebNews.SQLRepositories.GallerySQLRepositories;
using WebNews.SQLRepositories.NewsSQLRepositories;
using WebNews.SQLRepositories.VideoSQLRepository;
using WebNews.Utilities;

namespace WebNews.Installers
{
    public class MvcInstaller : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {

            services.AddControllersWithViews();


            services.AddDbContext<DatabaseContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), opt =>
                {
                    //opt.EnableRetryOnFailure(2);
                    //opt.MaxBatchSize(500);
                });
            });
            services.AddIdentity<ApplicationUser, ApplicationRole>(c =>
            {
                c.Password.RequireDigit = false;
                c.Password.RequireNonAlphanumeric = false;
                c.Password.RequireUppercase = false;
                c.Password.RequireLowercase = false;
                c.Password.RequiredLength = 6;
            })
                .AddEntityFrameworkStores<DatabaseContext>()
                .AddDefaultTokenProviders();



            services.AddAuthentication();
            services.AddAuthorization(c => {
                c.AddPolicy("Admin", policy => policy.RequireAssertion(context => context.User.IsInRole("Admin")));
                c.AddPolicy("Author", policy => policy.RequireAssertion(context => context.User.IsInRole("Author")));
            });


            services.AddScoped<IArticleRepository, ArticleSQLRepository>();
            services.AddScoped<IVideoRepository, VideoSQLRepository>();
            services.AddScoped<IGalleryRepository<Gallery>, GallerySQLRepository>();
            services.AddScoped<INewsRepositories<News>, NewsSQLRepositoryBase>(); 
            services.AddTransient<IImageHandler, ImageHandler>();
            services.AddTransient<IImageWriter, ImageWriter>();
            services.AddTransient<ISeoUrlEditor, SeoUrlEditor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IEmail, MailJet>();
            services.Configure<EmailOptionDTO>(configuration.GetSection("Mailjet"));


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

            services.AddMemoryCache();
            services.AddResponseCompression(c => c.Providers.Add<GzipCompressionProvider>());

            services.Configure<RouteOptions>(c => 
            {
                c.LowercaseUrls = true;
                c.LowercaseQueryStrings = true;
            });
        }
    }
}
