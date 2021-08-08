using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebNews.Models;

namespace WebNews.Areas.Panel.ViewModels.Roles
{
    public class EditRoleVm
    {
        public string RoleName { get; set; }
        public string Previous { get; set; }
        public ReflectionIT.Mvc.Paging.PagingList<ApplicationUser> Users { get; set; } 
        public List<ApplicationUser> Modirators { get; set; } = new List<ApplicationUser>();
    }
}
