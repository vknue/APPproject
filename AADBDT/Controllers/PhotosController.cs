using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AADBDT.Models;
using AADBDT.ViewModels;
using BusinessLogic.Models;
using BusinessLogic;
using Infrastructure.Data;
using BusinessLogic.Factories;
using BusinessLogic.Helpers;
using SixLabors.ImageSharp;

[Authorize]
public class PhotosController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPhotoService _photoService;

    public PhotosController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IPhotoService photoService)
    {
        _context = context;
        _userManager = userManager;
        _photoService = photoService;
    }

    [HttpGet]
    public IActionResult Upload()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(PhotoUploadViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var count = await _photoService.GetTodayUploadCount(user.Id);
        var limit = PackageHelper.GetDailyLimit(user.Package);
        if (count >= limit)
        {
            TempData["Error"] = $"You have reached your daily limit of {limit} uploads for the {user.Package} package.";
            return RedirectToAction("Index", "Home"); 
        }


        if (!ModelState.IsValid || model.ImageFile == null)
        {
            return View(model);
        }

        

        var filePath = await _photoService.SaveFileAsync(
        model.ImageFile,
        model.TargetFormat,
        model.ResizeToSquare
    );

        var photo = new Photo
        {
            Title = "User upload",
            Description = model.Description,
            Hashtags = model.Hashtags,
            ImageUrl = filePath,
            UploadDate = DateTime.Now,
            UserId = user.Id
        };

        _context.Photos.Add(photo);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Photo uploaded successfully";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var photo = await _context.Photos.FindAsync(id);

        if (photo == null) return NotFound();
        if (photo.UserId != user.Id && !User.IsInRole("Admin")) return Unauthorized();

        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", photo.ImageUrl.TrimStart('/'));
        if (System.IO.File.Exists(fullPath))
        {
            System.IO.File.Delete(fullPath);
        }

        _context.Photos.Remove(photo);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Photo deleted successfully.";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Download(int id, List<string> filters)
    {
        var photo = await _context.Photos.FindAsync(id);
        if (photo == null) return NotFound();

        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", photo.ImageUrl.TrimStart('/'));

        using var image = Image.Load(path);

        foreach (var filterName in filters)
        {
            var strategy = FilterFactory.GetStrategy(filterName);
            strategy?.Apply(image); 
        }

        using var ms = new MemoryStream();
        image.SaveAsJpeg(ms);
        return File(ms.ToArray(), "image/jpeg", $"filtered_{id}.jpg");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, string description, string hashtags)
    {
        var photo = await _context.Photos.FindAsync(id);
        var userId = _userManager.GetUserId(User);

        if ((photo == null || photo.UserId != userId) &&  !(User.IsInRole("Admin"))) return Unauthorized();

        photo.Description = description;
        photo.Hashtags = hashtags;

        _context.Photos.Update(photo);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }
}