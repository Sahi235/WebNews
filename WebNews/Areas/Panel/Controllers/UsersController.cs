using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration;
using ReflectionIT.Mvc.Paging;
using WebNews.Areas.Panel.ViewModels.Users;
using WebNews.Models;
using WebNews.Utilities;

namespace WebNews.Areas.Panel.Controllers
{
    [Area("Panel")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IImageHandler imageHandler;

        public UsersController(UserManager<ApplicationUser> userManager,
                               RoleManager<ApplicationRole> roleManager,
                               IImageHandler imageHandler)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.imageHandler = imageHandler;
        }


        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            var users = userManager.Users
                                    .AsNoTracking()
                                        .Include(c => c.Roles)
                                                .ThenInclude(c => c.Role)
                                                    .Include(c => c.News)
                                                        .Include(c => c.Galleries)
                                                            .OrderByDescending(c => c.SignUpDate);
            var model = await PagingList.CreateAsync(users, 20, page);
            return View(model);
        }






        [HttpGet]
        [Route("[Area]/[Controller]/[Action]/{userName}")]
        [Route("[Area]/[Controller]/[Action]/{userName}/{messsage}")]
        public async Task<IActionResult> Profile(string userName, string message)
        {
            var user = await userManager.Users
                .Include(c => c.Roles)
                .ThenInclude(c => c.Role)
                .Include(c => c.News)
                .Take(9)
                .Include(c => c.News)
                .ThenInclude(c => c.Categories)
                .ThenInclude(c => c.Category)
                .Include(c => c.Galleries)
                .Take(9)
                .SingleOrDefaultAsync(c => c.UserName == userName);

            UserProfileVM model = new UserProfileVM()
            {
                User = user,
            };
            if (message != null)
            {
                model.Message = message;
            }
            return View(model);
        }

        [HttpGet]
        [Route("[Area]/[Controller]/[Action]/{userName}")]
        public async Task<IActionResult> EditUserRole(string userName)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user == null) return NotFound();
            EditUserRoleVM model = new EditUserRoleVM
            {
                User = user
            };
            var allRoles = await roleManager.Roles.ToListAsync();
            foreach (var role in allRoles)
            {
                var vm = new UserRoleVM();
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    vm.RoleName = role.Name;
                    vm.RoleId = role.Id;
                    vm.InRole = true;
                    vm.WasInRole = true;
                }
                else
                {
                    vm.RoleId = role.Id;
                    vm.RoleName = role.Name;
                    vm.InRole = false;
                    vm.WasInRole = false;
                }
                model.UserRoles.Add(vm);
            }
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> EditUserRole(EditUserRoleVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                if (model.User.UserName == null) return NotFound();
                
                var user = await userManager.FindByNameAsync(model.User.UserName);
                
                if (user == null) return NotFound();

                foreach (var r in model.UserRoles)
                {
                    var role = await roleManager.FindByIdAsync(r.RoleId);
                    if (role == null) return NotFound();
                    if (!r.InRole || r.WasInRole)
                    {
                        await userManager.RemoveFromRoleAsync(user, role.Name);
                    }
                    else if (r.InRole || !r.WasInRole)
                    {
                        await userManager.AddToRoleAsync(user, role.Name);
                    }
                    else
                    {
                        continue;
                    }
                }
                string message = "User was edited seccessfully";
                return RedirectToAction(nameof(Profile), new { model.User.UserName, message });
            }
        }


        [HttpGet]
        [Route("[Area]/[Controller]/[Action]/{userName}")]
        public async Task<IActionResult> EditProfile(string userName)
        {
            var user = await userManager.Users
                                    .Include(c => c.Roles)
                                        .ThenInclude(c => c.Role)
                                            .SingleOrDefaultAsync(c => c.UserName == userName);
            EditProfileVM model = new EditProfileVM
            {
                User = user,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await userManager.FindByIdAsync(model.User.Id);
            if (user == null) return NotFound();
            StringBuilder sb = new StringBuilder();
            
            user.FullAdress = sb.ToString();
            if (model.NewImage != null)
            {
                if (user.ImageUrl != null)
                {
                    imageHandler.RemoveImage(Constant.UserFolder, user.ImageUrl);
                }
                var extension = "." + model.NewImage.FileName.Split('.')[^1];
                var fileName = Guid.NewGuid().ToString() + extension;
                await imageHandler.UploadImage(model.NewImage, Constant.UserFolder, fileName);
                user.ImageUrl = fileName;
            }
            user.PhoneNumber = model.User.PhoneNumber;
            user.AdressOne = model.User.AdressOne;
            user.AdressTwo = model.User.AdressTwo;
            user.City = model.User.City;
            user.Country = model.User.Country;
            user.ZipCode = model.User.ZipCode;
            user.Province = model.User.Province;
            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                model.Message = "User updated successfully";
                return RedirectToAction(nameof(EditProfile), new { user.Id });
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View();
        }
    }
}