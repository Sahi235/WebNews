using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string ImageUrl { get; set; }
        public string AboutMe { get; set; }
        public DateTime SignUpDate { get; set; } = DateTime.Now;
        public string City { get; set; }
        public string FullAdress { get; set; }
        public string AdressOne { get; set; }
        public string AdressTwo { get; set; }
        public string Province { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string Name { get; set; }
        public virtual ICollection<News> News { get; set; } = new HashSet<News>();
        public virtual ICollection<News> ModifiedNews { get; set; } = new HashSet<News>();
        public virtual ICollection<Video> Videos { get; set; } = new HashSet<Video>();
        public virtual ICollection<Gallery> Galleries { get; set; } = new HashSet<Gallery>();
        public virtual ICollection<FavouriteCateUser> FavouriteCategories { get; set; } = new HashSet<FavouriteCateUser>();
        public virtual ICollection<ApplicationUserRole> Roles { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public virtual ICollection<Activity> Activities { get; set; } = new HashSet<Activity>();
        public virtual ICollection<Email> SentMessages { get; set; } = new HashSet<Email>();
        public virtual ICollection<Email> ReceivedEmail { get; set; } = new HashSet<Email>();
    }
}
