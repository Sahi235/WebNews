using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WebNews.Models;

namespace WebNews.Areas.Panel.ViewModels.Users
{
    public class EditUserRoleVM
    {
        public ApplicationUser User { get; set; }
        public List<UserRoleVM> UserRoles { get; set; } = new List<UserRoleVM>();
    }
}
