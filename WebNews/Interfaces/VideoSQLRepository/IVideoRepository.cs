using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebNews.Interfaces.BaseInterfaces;
using WebNews.Models;

namespace WebNews.Interfaces.VideoSQLRepository
{
    public interface IVideoRepository : IRepositoryBase<Video>, IListRepository<Video>, IIQueryableRepository<Video>, ISingleRepository<Video>
    {
    }
}
