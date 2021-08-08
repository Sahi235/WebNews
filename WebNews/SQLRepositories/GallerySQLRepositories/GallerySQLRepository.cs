using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebNews.Data;
using WebNews.Interfaces.GalleryInterface;
using WebNews.Models;
using WebNews.SQLRepositories.BaseSQLRepositories;

namespace WebNews.SQLRepositories.GallerySQLRepositories
{
    public class GallerySQLRepository : SQLBaseAllImplementations<Gallery>, IGalleryRepository<Gallery>
    {
        private readonly DatabaseContext context;

        public GallerySQLRepository(DatabaseContext context) : base(context)
        {
            this.context = context;
        }
    }
}
