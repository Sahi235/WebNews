using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebNews.Areas.Panel.ViewModels.Roles;
using WebNews.Models;

namespace WebNews.Areas.Panel.Controllers
{
    [Area("Panel")]
    public class RolesController : Controller
    {
        private readonly RoleManager<ApplicationRole> roleManager;

        public RolesController(RoleManager<ApplicationRole> roleManager)
        {
            this.roleManager = roleManager;
        }


        [HttpGet]
        public async Task<IActionResult> RolesList()
        {
            var roles = await roleManager.Roles.Include(c => c.Users).AsNoTracking().ToListAsync();
            return View(roles);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            CreateRoleVM createRoleVM = new CreateRoleVM();
            return View(createRoleVM);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                if (model.RoleName == null || model.RoleName == " ")
                {
                    model.RoleName = " ";
                    model.Message = $"Role Name cannot be null";
                    return View(model);
                }
                var existingRole = await roleManager.FindByNameAsync(model.RoleName);
                if (existingRole != null)
                {
                    ModelState.AddModelError(string.Empty, "This Role Already Exists");
                    return View(model);
                }
                ApplicationRole role = new ApplicationRole
                {
                    Name = model.RoleName
                };
                var result = await roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
                else
                {
                    model.Message = $"{model.RoleName} was created";
                    model.RoleName = " ";
                    return View(model);
                }
            }
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsRoleTaken(string roleName)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null) return Json(true);
            else return Json($"{roleName} is already exists");
        }



        [HttpGet]
        public async Task<IActionResult> Edit(string name, int page = 1)
        {
            var role = await roleManager.FindByNameAsync(name);
            var Users = role.Users
                                 .AsQueryable()
                                    .Include(c => c.Role)
                                        .Include(c => c.User)
                                            .Where(c => c.RoleId == role.Id)
                                                   .Select(c => c.User)
                                                        .OrderByDescending(c => c.SignUpDate);
            var reflrectionIt = PagingList.Create(Users, 2, page);
            EditRoleVm model = new EditRoleVm()
            {
                RoleName = role.Name,
                Previous = role.Name,
                Users = reflrectionIt,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditRoleVm model)
        {
            var role = await roleManager.FindByNameAsync(model.Previous);
            if (ModelState.IsValid || role != null)
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(RolesList));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(new { model.Previous });
                }
            }
            return View(new { model.Previous });   
        }
    }
}
