using BusinessLogic.Enums;
using BusinessLogic.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AADBDT.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalUsers = _userManager.Users.Count();
            ViewBag.TotalPhotos = _context.Photos.Count();
            return View();


        }


        public async Task<IActionResult> UserDetails(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var logs = _context.AuditLogs
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.Timestamp)
                .ToList();

            ViewBag.TotalActions = logs.Count;
            ViewBag.LastActive = logs.FirstOrDefault()?.Timestamp.ToString("g") ?? "Never";
            ViewBag.MostFrequentAction = logs.GroupBy(l => l.Action)
                                             .OrderByDescending(g => g.Count())
                                             .FirstOrDefault()?.Key ?? "None";

            return View(new Tuple<ApplicationUser, List<AuditLog>>(user, logs));
        }

        public IActionResult Users()
        {
            return View(_userManager.Users.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserPackage(string userId, PackageType newPackage)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            user.Package = newPackage;
            await _userManager.UpdateAsync(user);

            TempData["Success"] = $"Updated {user.UserName} to {newPackage}";
            return RedirectToAction(nameof(Users));
        }
    }
}
