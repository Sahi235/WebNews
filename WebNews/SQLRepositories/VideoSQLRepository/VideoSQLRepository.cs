using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebNews.Data;
using WebNews.Interfaces.VideoSQLRepository;
using WebNews.Models;
using WebNews.SQLRepositories.BaseSQLRepositories;

namespace WebNews.SQLRepositories.VideoSQLRepository
{
    public class VideoSQLRepository : SQLBaseAllImplementations<Video>, IVideoRepository
    {
        public VideoSQLRepository(DatabaseContext context) : base(context)
        {

        }
    }
}
