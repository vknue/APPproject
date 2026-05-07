using AADBDT.Models;
using BusinessLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AADBDT.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPhotoService _photoService; // Added service field

        public HomeController(ILogger<HomeController> logger, IPhotoService photoService)
        {
            _logger = logger;
            _photoService = photoService;
        }

        public async Task<IActionResult> Index(int page = 1, string hashtag = null, string author = null, DateTime? start = null, DateTime? end = null)
        {
            var (photos, totalPages) = await _photoService.GetPagedPhotos(page, 10, hashtag, author, start, end);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Hashtag = hashtag;
            ViewBag.Author = author;

            

            return View(photos);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}