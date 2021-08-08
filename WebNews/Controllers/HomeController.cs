using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ReflectionIT.Mvc.Paging;
using StackExchange.Profiling.Internal;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using WebNews.Data;
using WebNews.Interfaces.ArticleInterface;
using WebNews.Interfaces.GalleryInterface;
using WebNews.Interfaces.NewsInterfaces;
using WebNews.Interfaces.VideoSQLRepository;
using WebNews.Models;
using WebNews.ViewModels.Home.Read;

namespace WebNews.Controllers
{
	public class HomeController : Controller
	{
		private readonly DatabaseContext context;
		private readonly IMemoryCache cache;
		private readonly IActionContextAccessor accessor;
		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly UserManager<ApplicationUser> userManager;
		private readonly INewsRepositories<News> newsRepositories;
		private readonly IGalleryRepository<Gallery> galleryRepository;
		private readonly IVideoRepository videoRepository;
		private readonly IArticleRepository articleRepository;

		public HomeController(DatabaseContext context,
							  IMemoryCache cache,
							  IActionContextAccessor accessor,
							  SignInManager<ApplicationUser> signInManager,
							  UserManager<ApplicationUser> userManager,
							  INewsRepositories<News> newsRepositories,
							  IGalleryRepository<Gallery> galleryRepository,
							  IVideoRepository videoRepository,
							  IArticleRepository articleRepository)
		{
			this.context = context;
			this.cache = cache;
			this.accessor = accessor;
			this.signInManager = signInManager;
			this.userManager = userManager;
			this.newsRepositories = newsRepositories;
			this.galleryRepository = galleryRepository;
			this.videoRepository = videoRepository;
			this.articleRepository = articleRepository;
		}


		public async Task<IActionResult> Index()
		{


			var news = await newsRepositories.GetOrderByConditionTakeSelectInList(c => c.PublishedDate, c => c.IsPublished == true, 3, c => new News
			{
				Id = c.Id,
				User = c.User,
				Likes = c.Likes,
				Title = c.Title,
				SeoUrl = c.SeoUrl,
				Comments = c.Comments.Where(e => e.IsApproved == true).ToList(),
				ViewCount = c.ViewCount,
				MainImage = c.MainImage,
				Categories = c.Categories
				.Select(e => new NewsCategory
				{
					Category = e.Category,
					CategoryId = e.CategoryId,
				}).ToList(),
				IsPublished = c.IsPublished,
				PublishedDate = c.PublishedDate,
				ShortDescription = c.ShortDescription,
			});

			var randomNews = await newsRepositories.GetWhereSkipOrderByDescTakeSelectInList((c => c.PublishedDate > DateTime.Now.AddDays(-7) && c.IsPublished == true), 3, c => Guid.NewGuid(), 5, c => new News
			{
				Id = c.Id,
				Title = c.Title,
				SeoUrl = c.SeoUrl,
				MainImage = c.MainImage,
				Categories = c.Categories
				.Select(e => new NewsCategory
				{
					Category = e.Category,
					CategoryId = e.CategoryId,
				}).ToList(),
				PublishedDate = c.PublishedDate,
			});


			var galleries = await galleryRepository.GetWhereSkipOrderByDescTakeSelectInList(c => c.IsPublished == true, 0, c => c.PublishedDate, 6, c => new Gallery
			{
				Title = c.Title,
				Likes = c.Likes,
				Comments = c.Comments,
				MainImage = c.MainImage,
				Categories = c.Categories
				.Select(e => new GalleryCategory
				{
					Category = e.Category,
					CategoryId = e.CategoryId,
				}).ToList(),
			});


			var popularNews = await newsRepositories.GetWhereOrderByDescTakeSelect(c => c.IsPublished == true, c => c.ViewCount, 5, c => new News
			{
				Id = c.Id,
				Title = c.Title,
				SeoUrl = c.SeoUrl,
				PublishedDate = c.PublishedDate,
			});


			var videos = await videoRepository.GetWhereOrderByDescTakeSelect(c => c.IsPublished == true, c => c.PublishedDate, 3, c => new Video
			{
				Id = c.Id,
				Title = c.Title,
				ImageUrl = c.ImageUrl,
				VideoUrl = c.VideoUrl,
			});


			var articles = await articleRepository.GetWhereOrderByDescTakeSelect(c => c.IsPublished == true, c => c.PublishedDate, 6, c => new Article
			{
				Id = c.Id,
				Title = c.Title,
				MainImage = c.MainImage,
				PublishedDate = c.PublishedDate,
			});


			IndexVM model = new IndexVM
			{
				News = news,
				RandomNews = randomNews,
				Videos = videos,
				Articles = articles,
				Galleries = galleries,
				PopularNews = popularNews,
			};


			var visitorIp = accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString();
			bool visitorIpExists = await context.Visitors.Where(c => c.IP == visitorIp).AnyAsync();
			if (!visitorIpExists)
			{
				var requestCulture = Request.HttpContext.Features.Get<IRequestCultureFeature>();
				var localCulture = requestCulture.RequestCulture.UICulture.ToString();
				bool cultureExists = await context.Cultures.Where(c => c.CultureName == localCulture).AnyAsync();
				if (!cultureExists)
				{
					Culture culture = new Culture
					{
						CultureName = localCulture,
						VisitCount = 1,
					};
					Visitor visitor = new Visitor
					{
						Culture = culture,
						CultureId = culture.Id,
						IP = visitorIp,
						VisitCount = 1,
					};
					VisitorHistory visitorHistory = new VisitorHistory
					{
						Visitor = visitor,
						VisitorId = visitor.Id,
						VisitTime = DateTime.Now,
					};
					await context.AddAsync(visitorHistory);
					await context.SaveChangesAsync();
				}
				else
				{
					Culture culture = await context.Cultures.FirstOrDefaultAsync(c => c.CultureName == localCulture);
					Visitor visitor = new Visitor
					{
						Culture = culture,
						CultureId = culture.Id,
						IP = visitorIp,
						VisitCount = 1,
					};
					VisitorHistory visitorHistory = new VisitorHistory
					{
						Visitor = visitor,
						VisitorId = visitor.Id,
						VisitTime = DateTime.Now,
					};
					culture.VisitCount++;
					context.Cultures.Attach(culture);
					await context.AddAsync(visitorHistory);
					await context.SaveChangesAsync();
				}

			}
			else
			{
				Visitor visitor = await context.Visitors.FirstOrDefaultAsync(c => c.IP == visitorIp);
				VisitorHistory visitorHistory = new VisitorHistory
				{
					Visitor = visitor,
					VisitorId = visitor.Id,
					VisitTime = DateTime.Now,
				};
				visitor.VisitCount++;
				context.Visitors.Attach(visitor);
				await context.VisitorHistories.AddAsync(visitorHistory);
				await context.SaveChangesAsync();
			}

			return View(model);
		}



		//BlogDetailsView
		[HttpGet]
		[Route("NewsDetails/{seoUrl}")]
		public async Task<IActionResult> NewsDetails(string seoUrl)
		{

			BlogDetailsVM model = new BlogDetailsVM();


			News newsDetails = await context.News
								.AsNoTracking()
								.Select(c => new News
								{
									Id = c.Id,
									Tags = c.Tags
									.Select(e => new NewsTag
									{
										Tag = e.Tag
									}).ToList(),
									User = context.Users
									.Select(e => new ApplicationUser
									{
										Id = e.Id,
										ImageUrl = e.ImageUrl,
										UserName = e.UserName,
									}).SingleOrDefault(e => e.Id == c.UserId),
									Body = c.Body,
									Title = c.Title,
									Image = c.Image,
									Likes = c.Likes,
									SeoUrl = c.SeoUrl,
									Comments = c.Comments
									.Where(c => c.IsApproved == true)
									.Select(e => new Comment
									{
										Id = e.Id,
										Name = e.Name,
										Description = e.Description,
										PublishedDate = e.PublishedDate,
										Answers = e.Answers
										.Select(t => new CommentAnswer
										{
											Id = t.Id,
											Body = t.Body,
											Name = t.Name,
											PublishedDate = t.PublishedDate,
										}).ToList(),
									}).OrderByDescending(e => e.PublishedDate).ToList(),
									ViewCount = c.ViewCount,
									MainImage = c.MainImage,
									Categories = c.Categories
									.Select(e => new NewsCategory
									{
										Category = e.Category,
										CategoryId = e.CategoryId,
									}).ToList(),
									PublishedDate = c.PublishedDate,
								}).SingleOrDefaultAsync(c => c.SeoUrl == seoUrl);


			//if (!cache.TryGetValue("NewsDetails", out newsDetails))
			//{
			//    if (newsDetails == null)
			//    {
			//        #region NewsDetailsCreater
			//        newsDetails = await context.News
			//                                    .AsNoTracking()
			//                                    .Where(c => c.SeoUrl == seoUrl && c.IsPublished == true)
			//                                    .Include(c => c.User)
			//                                    .Select(c => new News
			//                                    {
			//                                        Id = c.Id,
			//                                        Tags = c.Tags
			//                                        .Select(e => new NewsTag
			//                                        {
			//                                            Tag = e.Tag
			//                                        }).ToList(),
			//                                        User = c.User,
			//                                        Body = c.Body,
			//                                        Title = c.Title,
			//                                        Image = c.Image,
			//                                        Likes = c.Likes,
			//                                        SeoUrl = c.SeoUrl,
			//                                        Comments = c.Comments
			//                                        .Where(c => c.IsApproved == true)
			//                                        .Select(e => new Comment
			//                                        {
			//                                            Id = e.Id,
			//                                            Name = e.Name,
			//                                            Description = e.Description,
			//                                            PublishedDate = e.PublishedDate,
			//                                            Answers = e.Answers
			//                                            .Select(t => new CommentAnswer 
			//                                            {
			//                                                Id = t.Id,
			//                                                Body = t.Body,
			//                                                Name = t.Name,
			//                                                PublishedDate = t.PublishedDate,
			//                                            }).ToList(),
			//                                        }).OrderByDescending(e => e.PublishedDate).ToList(),
			//                                        ViewCount = c.ViewCount,
			//                                        MainImage = c.MainImage,
			//                                        Categories = c.Categories
			//                                        .Select(e => new NewsCategory
			//                                        {
			//                                            Category = e.Category,
			//                                            CategoryId = e.CategoryId,
			//                                        }).ToList(),
			//                                        PublishedDate = c.PublishedDate,
			//                                    }).SingleOrDefaultAsync();
			//        #endregion
			//    }

			//    cache.Set("NewsDetails", newsDetails);
			//}




			var randomNews = await context.News
			.AsNoTracking()
				.Where(c => c.PublishedDate > DateTime.Now.AddDays(-7)
						&& c.IsPublished == true)
							.Skip(3)
								.OrderByDescending(c => Guid.NewGuid())
									.Take(8)
										.Select(c => new News
										{
											Id = c.Id,
											Title = c.Title,
											SeoUrl = c.SeoUrl,
											MainImage = c.MainImage,
											Categories = c.Categories
											.Select(e => new NewsCategory
											{
												Category = e.Category,
												CategoryId = e.CategoryId,
											}).ToList(),
											PublishedDate = c.PublishedDate,
										})
										.ToListAsync();



			var popularNews = await context.News
											.AsNoTracking()
												.Where(c => c.IsPublished == true)
													.OrderByDescending(c => c.ViewCount)
														.Take(6)
															.Select(c => new News
															{
																Id = c.Id,
																Title = c.Title,
																SeoUrl = c.SeoUrl,
																PublishedDate = c.PublishedDate,
															})
															.ToListAsync();




			var latestComments = await context.Comments
												.AsNoTracking()
													.OrderByDescending(c => c.PublishedDate)
														.Where(c => c.IsApproved == true)
															.Take(6)
																.Include(c => c.News)
																	.Select(c => new Comment
																	{
																		Id = c.Id,
																		News = c.News,
																		Name = c.Name,
																		Description = c.Description,
																		PublishedDate = c.PublishedDate,
																	})
																	.ToListAsync();

			if (newsDetails.Categories != null)
			{
				foreach (var cate in newsDetails.Categories)
				{
					var relatedNews = await context.NewsCategories
										.AsNoTracking()
											.Where(c => c.CategoryId == cate.CategoryId)
												.Include(c => c.News)
													.Select(c => c.News)
														.OrderByDescending(c => c.PublishedDate)
																//.SkipWhile(c => c.Id == newsDetails.Id)
																.Take(4)
																	.Select(c => new News
																	{
																		Id = c.Id,
																		Title = c.Title,
																		SeoUrl = c.SeoUrl,
																		MainImage = c.MainImage,
																		PublishedDate = c.PublishedDate,
																	}).ToListAsync();
					model.RelatedNews = relatedNews;

					break;
				}
			}
			else
			{
				foreach (var tag in newsDetails.Tags)
				{
					var relatedNews = await context.NewsTags
											.AsNoTracking()
												.Where(c => c.TagId == tag.TagId)
													.Include(c => c.News)
														.Select(c => c.News)
															.OrderByDescending(c => c.PublishedDate)
																	//.SkipWhile(c => c.Id == newsDetails.Id)
																	.Take(4)
																		.Select(c => new News
																		{
																			Id = c.Id,
																			Title = c.Title,
																			SeoUrl = c.SeoUrl,
																			MainImage = c.MainImage,
																			PublishedDate = c.PublishedDate,
																		}).ToListAsync();
					model.RelatedNews = relatedNews;
					break;
				}
			}


			model.News = newsDetails;
			model.PopularNews = popularNews;
			model.RandomNews = randomNews;
			model.Comments = latestComments;


			var newsCount = await context.News.FindAsync(newsDetails.Id);
			newsCount.ViewCount += 1;
			context.News.Attach(newsCount);
			await context.SaveChangesAsync();

			return View(model);
		}




		[HttpPost]
		[Route("NewsDetails/{seoUrl}")]
		public async Task<IActionResult> NewsDetails([Bind("Name,Description,Email")] Comment comment, string seoUrl)
		{
			var news = await context.News.Where(c => c.SeoUrl == seoUrl).SingleOrDefaultAsync();
			if (news == null)
			{
				return NotFound();
			}
			comment.News = news;
			comment.NewsId = news.Id;
			comment.PublishedDate = DateTime.Now;
			if (signInManager.IsSignedIn(User))
			{
				var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
				comment.User = user;
				comment.UserId = user.Id;
				comment.Name = user.UserName;
				comment.Email = user.Email;
			}

			await context.Comments.AddAsync(comment);
			await context.SaveChangesAsync();
			return RedirectToAction("NewsDetails", new RouteValueDictionary(new { controller = "Home", action = "NewsDetails", seoUrl = news.SeoUrl }));
		}



		[Route("categorynews/{name}")]
		[Route("categorynews/{name}/{page}")]
		public async Task<IActionResult> CategoryNews(string name, int page = 1)
		{

			var category = await context.Categories.Where(c => c.Name == name).SingleOrDefaultAsync();

			var news = context.NewsCategories
			   .AsNoTracking()
			   .Where(c => c.CategoryId == category.Id)
			   .Select(c => c.News)
			   .Select(c => new News
			   {
				   Id = c.Id,
				   User = c.User,
				   Likes = c.Likes,
				   Title = c.Title,
				   SeoUrl = c.SeoUrl,
				   Comments = c.Comments
				   .Where(e => e.IsApproved == true)
				   .ToList(),
				   MainImage = c.MainImage,
				   Categories = c.Categories
				   .Select(e => new NewsCategory
				   {
					   Category = e.Category,
					   CategoryId = e.CategoryId,
				   }).ToList(),
				   IsPublished = c.IsPublished,
				   PublishedDate = c.PublishedDate,
				   ShortDescription = c.ShortDescription,
			   })
			   .Where(c => c.IsPublished == true)
			   .OrderByDescending(c => c.PublishedDate);

			var newsModel = await PagingList.CreateAsync(news, 4, page);
			newsModel.Action = nameof(CategoryNews);
			newsModel.PageParameterName = "page";



			var randomNews = await context.News
											.AsNoTracking()
												.Where(c => c.PublishedDate > DateTime.Now.AddDays(-3)
														&& c.IsPublished == true)
															.Skip(3)
																.OrderByDescending(c => Guid.NewGuid())
																	.Take(8)
																		.Include(c => c.Categories)
																			.ThenInclude(c => c.Category)
																				.Select(c => new News
																				{
																					Id = c.Id,
																					Title = c.Title,
																					SeoUrl = c.SeoUrl,
																					MainImage = c.MainImage,
																					Categories = c.Categories
																					.Select(e => new NewsCategory
																					{
																						Category = e.Category,
																						CategoryId = e.CategoryId,
																					}).ToList(),
																					PublishedDate = c.PublishedDate,
																				})
																				.ToListAsync();



			var popularNews = await context.News
											.AsNoTracking()
												.Where(c => c.IsPublished == true)
													.OrderByDescending(c => c.ViewCount)
														.Take(6)
															.Select(c => new News
															{
																Id = c.Id,
																Title = c.Title,
																SeoUrl = c.SeoUrl,
																PublishedDate = c.PublishedDate,
															})
															.ToListAsync();

			var latestComments = await context.Comments
									.AsNoTracking()
										.OrderByDescending(c => c.PublishedDate)
											.Where(c => c.IsApproved == true)
												.Take(6)
													.Include(c => c.News)
														.Select(c => new Comment
														{
															Id = c.Id,
															News = c.News,
															Name = c.Name,
															Description = c.Description,
															PublishedDate = c.PublishedDate,
														})
														.ToListAsync();


			CategoryNewsVM model = new CategoryNewsVM
			{
				Comments = latestComments,
				News = newsModel,
				PopularNews = popularNews,
				RandomNews = randomNews,
			};
			return View(model);
		}


		public async Task<IActionResult> Search(string searchString, int pagenumber = 1)
		{
			//if (String.IsNullOrEmpty(searchString))
			//{
			//    return RedirectToAction(nameof(Index));
			//}

			var news = context.News
								.AsNoTracking()
									.Where(c => c.Title.Contains(searchString) || c.Body.Contains(searchString))
										.Select(c => new News
										{
											Id = c.Id,
											User = c.User,
											Likes = c.Likes,
											Title = c.Title,
											SeoUrl = c.SeoUrl,
											Comments = c.Comments
										   .Where(e => e.IsApproved == true)
										   .ToList(),
											MainImage = c.MainImage,
											Categories = c.Categories
										   .Select(e => new NewsCategory
										   {
											   Category = e.Category,
											   CategoryId = e.CategoryId,
										   }).ToList(),
											IsPublished = c.IsPublished,
											PublishedDate = c.PublishedDate,
											ShortDescription = c.ShortDescription,
										}).Where(c => c.IsPublished == true)
										  .OrderByDescending(c => c.PublishedDate);

			var newsModel = await PagingList.CreateAsync(news, 6, pagenumber);
			newsModel.PageParameterName = "pagenumber";


			var randomNews = await context.News
											.AsNoTracking()
												.Where(c => c.PublishedDate > DateTime.Now.AddDays(-3)
														&& c.IsPublished == true)
															.Skip(3)
																.OrderByDescending(c => Guid.NewGuid())
																	.Take(8)
																		.Include(c => c.Categories)
																			.ThenInclude(c => c.Category)
																				.Select(c => new News
																				{
																					Id = c.Id,
																					Title = c.Title,
																					SeoUrl = c.SeoUrl,
																					MainImage = c.MainImage,
																					Categories = c.Categories
																					.Select(e => new NewsCategory
																					{
																						Category = e.Category,
																						CategoryId = e.CategoryId,
																					}).ToList(),
																					PublishedDate = c.PublishedDate,
																				})
																				.ToListAsync();



			var popularNews = await context.News
											.AsNoTracking()
												.Where(c => c.IsPublished == true)
													.OrderByDescending(c => c.ViewCount)
														.Take(6)
															.Select(c => new News
															{
																Id = c.Id,
																Title = c.Title,
																SeoUrl = c.SeoUrl,
																PublishedDate = c.PublishedDate,
															})
															.ToListAsync();

			var latestComments = await context.Comments
									.AsNoTracking()
										.OrderByDescending(c => c.PublishedDate)
											.Where(c => c.IsApproved == true)
												.Take(6)
													.Include(c => c.News)
														.Select(c => new Comment
														{
															Id = c.Id,
															News = c.News,
															Name = c.Name,
															Description = c.Description,
															PublishedDate = c.PublishedDate,
														})
														.ToListAsync();


			CategoryNewsVM model = new CategoryNewsVM
			{
				Comments = latestComments,
				News = newsModel,
				PopularNews = popularNews,
				RandomNews = randomNews,
			};

			return View(model);
		}


		public string TestMe()
		{
			var locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
			var BrowserCulture = locale.RequestCulture.UICulture.ToString();
			return BrowserCulture;
		}

		public IActionResult Sahand()
		{
			var news = context.News.Include(c => c.Comments).Select(c => new News 
			{
				Id = c.Id,
				Title = c.Title,
				Comments = c.Comments
			});
			return Json(news);
		}

		[Route("[Action]/{id}")]
		public async Task<IActionResult> Sahand2(int id)
		{
			//var news = context.News.Include(c => c.Comments).Include(c => c.Categories).ThenInclude(c => c.Category).Select(c => new News
			//{
			//    Id = c.Id,
			//    Title = c.Title,
			//    PublishedDate = c.PublishedDate,
			//    Comments = c.Comments,
			//    Categories = c.Categories,
			//}).OrderByDescending(c => c.PublishedDate).ToList();
			//List<News> newsList = new List<News>();
			//var connection = context.Database.GetDbConnection();
			//try
			//{
			//    await connection.OpenAsync();
			//    using (var command = connection.CreateCommand())
			//    {
			//        string query = "select * from News";
			//        command.CommandText = query;
			//        DbDataReader reader = await command.ExecuteReaderAsync();
			//        if (reader.HasRows)
			//        {
			//            if (reader.HasRows)
			//            {
			//                while (await reader.ReadAsync())
			//                {
			//                    var row = new News { Id = reader.GetInt32(1), Title = reader.GetString(1) };
			//                    newsList.Add(row);
			//                }
			//            }
			//            reader.Dispose();
			//        }
			//    }
			//}
			//finally
			//{
			//    connection.Close();
			//}
			//////var sqlParam = new SqlParameter();
			//////sqlParam.ParameterName("");
			//News news = context.News.FromSqlInterpolated($"spTest2NewsWithId {id}").ToList().FirstOrDefault();

			var newsList = await context.News.FromSqlRaw(@"select [N].*, [NC].*, [C].* from [News] AS [N]
														left join
														(
															select [NewsCate].*, [Cate].* 
															from [NewsCategories] AS [NewsCate]
															inner join [Categories] AS [Cate]
															on [NewsCate].CategoryId = [Cate].[Id]
														) as [NC]
														on [N].[Id] = [NC].[NewsId]
														left join [Comments] as [C]
														on [N].[Id] = [C].[NewsId]").ToListAsync();
			return View(newsList);
		}
	}


}
