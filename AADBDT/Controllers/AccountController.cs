using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Models;
using BusinessLogic.Enums;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AADBDT.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        // Guest Login Logic
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GuestLogin()
        {
            var guestEmail = "guest@temp.com";
            var user = await _userManager.FindByEmailAsync(guestEmail);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = guestEmail,
                    Email = guestEmail,
                    Package = PackageType.FREE,
                    FirstName = "Guest",
                    LastName = "User"
                };
                await _userManager.CreateAsync(user, "Guest123!");
            }

            if (!await _userManager.IsInRoleAsync(user, "Guest"))
            {
                await _userManager.AddToRoleAsync(user, "Guest");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestPackageChange(PackageType newType)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user.LastPackageChangeDate?.Date == DateTime.Today)
            {
                TempData["Error"] = "Plan already changed today. Try again tomorrow.";
                return RedirectToAction("Index", "Home");
            }

            user.PendingPackage = newType;
            user.LastPackageChangeDate = DateTime.Now;

            await _userManager.UpdateAsync(user);
            TempData["Success"] = $"Plan set to {newType}. Update will trigger at midnight.";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(string email, string password, string firstName, string lastName, PackageType package)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Package = package // Sets the package chosen in the dropdown
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"Error: {error.Code} - {error.Description}");
            }
            return RedirectToPage("/Account/Register", new { area = "Identity" });
        }

        // External Login (Google/GitHub)
        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null) return RedirectToAction(nameof(Login));
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                Package = PackageType.FREE,
                FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? "External",
                LastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "User"
            };
            var createResult = await _userManager.CreateAsync(user);
            if (createResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _userManager.AddLoginAsync(user, info);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }   
            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}