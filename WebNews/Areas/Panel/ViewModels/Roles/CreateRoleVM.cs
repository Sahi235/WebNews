using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

using WebNews.Areas.Panel.Controllers;

namespace WebNews.Areas.Panel.ViewModels.Roles
{
    public class CreateRoleVM
    {
        [Remote(action: nameof(RolesController.IsRoleTaken), controller: "Roles", areaName: "Panel")]
        public string RoleName { get; set; }
        [TempData]
        public string Message { get; set; }
    }
}
