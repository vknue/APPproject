using AADBDT.ViewModels;
using BusinessLogic.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

namespace APPTesting
{
    public class PhotoControllerTests
    {

        /*
         * Test uploading limit for FREE Package Type
         */
        [Fact]
        public async Task Upload_RedirectsToHome_WhenDailyLimitIsExceeded()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            using var context = new ApplicationDbContext(options);

            var mockUser = new ApplicationUser { Id = "user123" };
            var mockPhotoService = new Mock<IPhotoService>();

            mockPhotoService.Setup(s => s.GetTodayUploadCount(mockUser.Id)).ReturnsAsync(5);

            var store = new Mock<IUserStore<ApplicationUser>>();
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(mockUser);

            var controller = new PhotosController(context, mockUserManager.Object, mockPhotoService.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };

            var result = await controller.Upload(new PhotoUploadViewModel());

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
        }


        /*
         * Test that the function returns 'NotFound' for invalid ID
         */
        [Fact]
        public async Task Download_ReturnsNotFound_WhenPhotoIdDoesNotExist()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            using var context = new ApplicationDbContext(options);
            var controller = new PhotosController(context, null, null);

            var result = await controller.Download(999, new List<string>());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Download_ReturnsFileResult_ForValidPhoto()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            using var context = new ApplicationDbContext(options);
            var controller = new PhotosController(context, null, null);

            var photo = new Photo
            {
                Id = 1,
                ImageUrl = "/uploads/test.jpg",
                Description = "Test",
                Hashtags = "#t",
                UserId = "1"
            };
            context.Photos.Add(photo);
            await context.SaveChangesAsync();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "test.jpg");
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            using (var image = new Image<Rgba32>(1, 1)) 
            {
                await image.SaveAsJpegAsync(path);
            }

            var result = await controller.Download(1, new List<string>());

            Assert.IsType<FileContentResult>(result);
        }
    }
}
