using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using WebNews.Data;
using WebNews.Models;
using WebNews.Utilities;
using WebNews.ViewModels.Accounts;

namespace WebNews.Controllers
{
    [AllowAnonymous]
    public class AccountsController : Controller
    {
        private readonly DatabaseContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IOptions<EmailOptionDTO> emailOption;
        private readonly IEmail email;

        public AccountsController(DatabaseContext context,
                                  UserManager<ApplicationUser> userManager,
                                  SignInManager<ApplicationUser> signInManager,
                                  RoleManager<ApplicationRole> roleManager,
                                  IOptions<EmailOptionDTO> emailOption,
                                  IEmail email)
        {
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.emailOption = emailOption;
            this.email = email;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                if (model.Email != model.ConfirmEmail) return View(model);

                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                };
                var result =  await userManager.CreateAsync(user, model.Password);


                await email.Send(model.Email, "You signed up at YalanNews, Welcome boy", emailOption.Value);

                var userRole = await roleManager.FindByNameAsync("User");
                if (userRole == null)
                {
                    ApplicationRole applicationRole = new ApplicationRole() { Name = "User"};
                    var roleCreateResult = await roleManager.CreateAsync(applicationRole);
                    if (!roleCreateResult.Succeeded)
                    {
                        foreach (var error in roleCreateResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    userRole = applicationRole;
                }
                await userManager.AddToRoleAsync(user, userRole.Name);
                if (result.Succeeded)
                {
                    await signInManager.PasswordSignInAsync(user, model.Password, false, false);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Username,Password,RememberMe")] LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else
            {
                var user = await userManager.FindByNameAsync(model.Username);
                if(user == null) return RedirectToAction(nameof(HomeController.Index), "Home");
                else
                {
                    //await signInManager.RefreshSignInAsync(user);
                    //await signInManager.PasswordSignInAsync(user.UserName, model.Password, isPersistent: model.RememberMe, lockoutOnFailure: false);
                    await signInManager.SignInAsync(user, isPersistent: model.RememberMe);

                }
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SignOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> IsUsernameInUse(string userName)
        {
            ApplicationUser userInUse = await userManager.FindByNameAsync(userName);
            if (userInUse != null) return Json($"{userName} is already taken");
            else return Json(true);
        }

        [HttpGet]
        [HttpPost]
        public  async Task<IActionResult> IsEmailInUse(string email)
        {
            ApplicationUser userInUse = await userManager.FindByEmailAsync(email);
            if (userInUse != null) return Json($"{email} is already taken");
            else return Json(true);
        }
    }
}