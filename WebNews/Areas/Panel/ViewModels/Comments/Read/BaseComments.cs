using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebNews.Models;

namespace WebNews.Areas.Panel.ViewModels.Comments.Read
{
    public class BaseComments
    {
        public ReflectionIT.Mvc.Paging.PagingList<Comment> UnApprovedComments { get; set; }
        public ReflectionIT.Mvc.Paging.PagingList<Comment> ApprovedComments { get; set; }
    }
}
