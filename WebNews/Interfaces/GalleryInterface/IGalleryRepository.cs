using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebNews.Interfaces.BaseInterfaces;
using WebNews.Models;

namespace WebNews.Interfaces.GalleryInterface
{
    public interface IGalleryRepository<Gallery> : IListRepository<Gallery>, IIQueryableRepository<Gallery>, ISingleRepository<Gallery>
    {
    }
}
