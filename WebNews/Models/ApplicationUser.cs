using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<ApplicationUserRole> Roles { get; set; }
        public string ImageUrl { get; set; }
        public string AboutMe { get; set; }
        public virtual ICollection<News> News { get; set; } = new HashSet<News>();
        public virtual ICollection<News> ModifiedNews { get; set; } = new HashSet<News>();
        public virtual ICollection<Video> Videos { get; set; } = new HashSet<Video>();
        public virtual ICollection<Gallery> Galleries { get; set; } = new HashSet<Gallery>();

    }
}
