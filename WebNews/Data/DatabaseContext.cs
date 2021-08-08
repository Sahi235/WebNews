using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebNews.Models;

namespace WebNews.Data
{
    public class DatabaseContext : IdentityDbContext
                                        <
                                         ApplicationUser,
                                         ApplicationRole,
                                         string,
                                         IdentityUserClaim<string>,
                                         ApplicationUserRole,
                                         IdentityUserLogin<string>,
                                         IdentityRoleClaim<string>,
                                         IdentityUserToken<string>
                                         >
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }
        #region Tables
        public DbSet<News> News { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<NewsCategory> NewsCategories { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<NewsImage> NewsImages { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleImage> ArticleImages { get; set; }
        public DbSet<ArticleCategory> ArticleCategories { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<GalleryCategory> GalleryCategories { get; set; }
        public DbSet<GalleryImage> GalleryImages { get; set; }
        public DbSet<GalleryTag> GalleryTags { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<NewsTag> NewsTags { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<VideoUrl> VideoUrls { get; set; }
        public DbSet<VideoTag> VideoTags { get; set; }
        public DbSet<VideoCategory> VideoCategories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ArticleTag> ArticleTags { get; set; }
        public DbSet<PaginationSetting> PaginationSettings { get; set; }
        public DbSet<CommentAnswer> CommentAnswers { get; set; }
        public DbSet<ContactUs> ContactUs { get; set; }
        public DbSet<Culture> Cultures { get; set; }
        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<VisitorHistory> VisitorHistories { get; set; }
        public DbSet<FavouriteCateUser> UserFavorites { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<EmailAnswer> EmailAnswers { get; set; }
        public DbSet<Attachments> Attachments { get; set; }

        //public DbSet<UserFriend> UserFriends { get; set; }
        #endregion
        #region Tables Logic
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<FavouriteCateUser>()
                            .HasKey(c => new { c.UserId, c.CategoryId });
            builder.Entity<FavouriteCateUser>()
                            .HasOne(c => c.User)
                                .WithMany(c => c.FavouriteCategories)
                                    .HasForeignKey(c => c.UserId);
            builder.Entity<FavouriteCateUser>()
                            .HasOne(c => c.Category)
                                .WithMany(c => c.Users)
                                    .HasForeignKey(c => c.CategoryId);



            builder.Entity<ApplicationUserRole>()
                        .HasKey(c => new { c.UserId, c.RoleId });
            builder.Entity<ApplicationUserRole>()
                        .HasOne(c => c.User)
                            .WithMany(c => c.Roles)
                                .HasForeignKey(c => c.UserId);
            builder.Entity<ApplicationUserRole>()
                        .HasOne(c => c.Role)
                            .WithMany(c => c.Users)
                                .HasForeignKey(c => c.RoleId);


            builder.Entity<NewsCategory>()
                        .HasKey(c => new { c.NewsId, c.CategoryId });
            builder.Entity<NewsCategory>()
                        .HasOne(c => c.News)
                            .WithMany(c => c.Categories)
                                .HasForeignKey(c => c.NewsId);
            builder.Entity<NewsCategory>()
                        .HasOne(c => c.Category)
                            .WithMany(c => c.News)
                                .HasForeignKey(c => c.CategoryId);




            builder.Entity<GalleryCategory>()
                        .HasKey(c => new { c.GalleryId, c.CategoryId });
            builder.Entity<GalleryCategory>()
                        .HasOne(c => c.Gallery)
                            .WithMany(c => c.Categories)
                                .HasForeignKey(c => c.GalleryId);
            builder.Entity<GalleryCategory>()
                        .HasOne(c => c.Category)
                            .WithMany(c => c.Galleries)
                                .HasForeignKey(c => c.CategoryId);

            builder.Entity<GalleryTag>()
            .HasKey(c => new { c.GalleryId, c.TagId });

            builder.Entity<GalleryTag>()
                        .HasOne(c => c.Gallery)
                            .WithMany(c => c.Tags)
                                .HasForeignKey(c => c.GalleryId);
            builder.Entity<GalleryTag>()
                        .HasOne(c => c.Tag)
                            .WithMany(c => c.Galleries)
                                .HasForeignKey(c => c.TagId);



            builder.Entity<NewsTag>()
                            .HasKey(c => new { c.NewsId, c.TagId });
            builder.Entity<NewsTag>()
                            .HasOne(c => c.News)
                                .WithMany(c => c.Tags)
                                    .HasForeignKey(c => c.NewsId);
            builder.Entity<NewsTag>()
                            .HasOne(c => c.Tag)
                                .WithMany(c => c.News)
                                    .HasForeignKey(c => c.TagId);



            builder.Entity<VideoCategory>()
                            .HasKey(c => new { c.VideoId, c.CategoryId });
            builder.Entity<VideoCategory>()
                            .HasOne(c => c.Category)
                                .WithMany(c => c.Videos)
                                    .HasForeignKey(c => c.CategoryId);
            builder.Entity<VideoCategory>()
                            .HasOne(c => c.Video)
                                .WithMany(c => c.Categories)
                                    .HasForeignKey(c => c.VideoId);





            builder.Entity<ArticleCategory>()
                            .HasKey(c => new { c.ArticleId, c.CategoryId });
            builder.Entity<ArticleCategory>()
                            .HasOne(c => c.Article)
                                .WithMany(c => c.Categories)
                                    .HasForeignKey(c => c.ArticleId);
            builder.Entity<ArticleCategory>()
                            .HasOne(c => c.Category)
                                .WithMany(c => c.Articles)
                                    .HasForeignKey(c => c.CategoryId);


            builder.Entity<ArticleTag>()
                            .HasKey(c => new { c.ArticleId, c.TagId });
            builder.Entity<ArticleTag>()
                            .HasOne(c => c.Article)
                                .WithMany(c => c.Tags)
                                    .HasForeignKey(c => c.ArticleId);
            builder.Entity<ArticleTag>()
                            .HasOne(c => c.Tag)
                                .WithMany(c => c.Articles)
                                    .HasForeignKey(c => c.TagId);

            builder.Entity<VideoTag>()
                            .HasKey(c => new { c.VideoId, c.TagId });
            builder.Entity<VideoTag>()
                            .HasOne(c => c.Video)
                                .WithMany(c => c.Tags)
                                    .HasForeignKey(c => c.VideoId);
            builder.Entity<VideoTag>()
                            .HasOne(c => c.Tag)
                                .WithMany(c => c.Videos)
                                    .HasForeignKey(c => c.TagId);

            builder.Entity<News>()
                        .HasMany(c => c.Image)
                            .WithOne(c => c.News)
                                .HasForeignKey(c => c.NewsId);
            builder.Entity<News>()
                        .HasMany(c => c.Comments)
                            .WithOne(c => c.News)
                                .HasForeignKey(c => c.NewsId);
            builder.Entity<News>()
                        .HasOne(c => c.User)
                            .WithMany(c => c.News)
                                .HasForeignKey(c => c.UserId)
                                        .IsRequired(false)
                                            .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<News>()
                        .HasOne(c => c.Modifier)
                            .WithMany(c => c.ModifiedNews)
                                .HasForeignKey(c => c.ModfierId)
                                    .IsRequired(false)
                                            .OnDelete(DeleteBehavior.Restrict);



            builder.Entity<Comment>()
                        .HasOne(c => c.News)
                            .WithMany(c => c.Comments)
                                .HasForeignKey(c => c.NewsId)
                                        .IsRequired(false);
            builder.Entity<Comment>()
                        .HasOne(c => c.Video)
                            .WithMany(c => c.Comments)
                                .HasForeignKey(c => c.VideoId)
                                        .IsRequired(false);
            builder.Entity<Comment>()
                        .HasOne(c => c.Gallery)
                            .WithMany(c => c.Comments)
                                .HasForeignKey(c => c.GalleryId)
                                    .IsRequired(false);

            builder.Entity<Gallery>()
                        .HasMany(c => c.Images)
                            .WithOne(c => c.Gallery)
                                .HasForeignKey(c => c.GalleryId);

            builder.Entity<Tag>()
                        .HasKey(c => c.Id);


            builder.Entity<Gallery>()
                        .HasOne(c => c.Modifier)
                            .WithMany(c => c.Galleries)
                                .HasForeignKey(c => c.UserId)
                                    .IsRequired(false);

            builder.Entity<PaginationSetting>()
                        .HasIndex(c => c.Name)
                            .IsUnique(true);
            builder.Entity<Video>()
                        .HasOne(c => c.User)
                            .WithMany(c => c.Videos)
                                .HasForeignKey(c => c.UserId)
                                    .IsRequired(false);

            builder.Entity<Email>()
                        .HasOne(c => c.Sender)
                            .WithMany(c => c.SentMessages)
                                .HasForeignKey(c => c.SenderId);
            builder.Entity<Email>()
                        .HasOne(c => c.Receiver)
                            .WithMany(c => c.ReceivedEmail)
                                .HasForeignKey(c => c.ReceiverId);
        }
        #endregion

    }

}