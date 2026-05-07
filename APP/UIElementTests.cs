using Microsoft.Playwright;
namespace APPTesting
{
    public class UIElementTests
    {

        /*
         * Make sure upload photo button is seen when user logs in
         */
        [Fact]
        public async Task HomePage_DisplaysUploadButton_OnUserLogIn()
        {
            using var playwright = await Playwright.CreateAsync();

            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true 
            });

            var context = await browser.NewContextAsync(new BrowserNewContextOptions
            {
                IgnoreHTTPSErrors = true 
            });

            var page = await context.NewPageAsync();

            await page.GotoAsync("https://localhost:7044/Identity/Account/Login");
            await page.FillAsync("input[name='Input.Email']", "teste@testet.com");
            await page.FillAsync("input[name='Input.Password']", "Valon123!");
            await page.ClickAsync("#login-submit");
            await page.WaitForURLAsync(url => !url.Contains("Login"), new() { Timeout = 5000 });
            await page.GotoAsync("https://localhost:7044");

            var uploadButton = page.Locator("#uploadButton");
            bool isVisible = await uploadButton.IsVisibleAsync();
            Assert.True(isVisible, "The upload button with ID 'uploadButton' was not visible on the page.");
        }

        /*
         * Make sure upload photo button is not seen when guest logs in
         */
        [Fact]
        public async Task HomePage_DoesntDisplayUploadButton_OnGuestLogIn()
        {
            using var playwright = await Playwright.CreateAsync();

            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });

            var context = await browser.NewContextAsync(new BrowserNewContextOptions
            {
                IgnoreHTTPSErrors = true
            });

            var page = await context.NewPageAsync();

            await page.GotoAsync("https://localhost:7044/Identity/Account/Login"); 
            var guestButton = page.GetByRole(AriaRole.Button, new() { Name = "Continue as Guest" });
            await guestButton.ClickAsync();
            await page.WaitForURLAsync(url => !url.Contains("Login"), new() { Timeout = 5000 });
            await page.GotoAsync("https://localhost:7044");

            var uploadButton = page.Locator("#uploadButton");
            bool isVisible = await uploadButton.IsVisibleAsync();
            Assert.False(isVisible, "The upload button with ID 'uploadButton' was not visible on the page.");
        }
    }
}
