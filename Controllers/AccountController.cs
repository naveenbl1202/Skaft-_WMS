using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SkaftoBageriA.Models;
using SkaftoBageriA.ViewModels;
using System.Threading.Tasks;

namespace SkaftoBageriA.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // Register GET
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Register POST
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    FullName = model.FullName
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Ensure role exists
                    if (!await _roleManager.RoleExistsAsync(model.Role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(model.Role));
                    }

                    // Assign role to user
                    await _userManager.AddToRoleAsync(user, model.Role);

                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        // Login GET
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Login POST
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid login attempt.");
            }

            return View(model);
        }

        // Logout POST
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        // Profile GET
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var model = new UserProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email
            };

            return View(model);
        }

        // Profile POST
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Profile(UserProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                // Update basic profile information
                user.FullName = model.FullName;
                user.Email = model.Email;

                // Handle password change if provided
                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    if (model.NewPassword == model.ConfirmPassword)
                    {
                        // Verify the current password before changing
                        var passwordValid = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
                        if (passwordValid)
                        {
                            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                            if (!changePasswordResult.Succeeded)
                            {
                                foreach (var error in changePasswordResult.Errors)
                                {
                                    ModelState.AddModelError("", error.Description);
                                }
                                return View(model);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Current password is incorrect.");
                            return View(model);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "New password and confirmation password do not match.");
                        return View(model);
                    }
                }

                // Save changes to user profile
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }

                ViewData["SuccessMessage"] = "Profile updated successfully!";
            }

            return View(model);
        }
    }
}
