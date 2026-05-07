using AADBDT.Controllers;
using BusinessLogic.Enums;
using BusinessLogic.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace APPTesting
{
    public class AccountControllerTests
    {

        [Fact]
        public async Task RequestPackageChange_ShouldFail_IfAlreadyChangedToday()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            using var context = new ApplicationDbContext(options);
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
        new Claim(ClaimTypes.NameIdentifier, "user123")
    }));
            var mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
        mockUserManager.Object,
        contextAccessor.Object,
        claimsFactory.Object,
        null, null, null, null);

            var mockUser = new ApplicationUser
            {
                Id = "user123",
                LastPackageChangeDate = DateTime.Now
            };

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                           .ReturnsAsync(mockUser);
            var mockPhotoService = new Mock<IPhotoService>();
            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userPrincipal }
            };
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var result = await controller.RequestPackageChange(PackageType.PRO);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Plan already changed today. Try again tomorrow.", controller.TempData["Error"]);
        }
    }
}
