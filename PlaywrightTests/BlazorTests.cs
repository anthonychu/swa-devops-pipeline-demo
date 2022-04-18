using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightTests
{
    [Parallelizable(ParallelScope.Self)]
    public class BlazorTests : PageTest
    {
        private string siteBaseUrl = "http://localhost:4200";
        public BlazorTests()
        {
            var siteBaseUrlVar = Environment.GetEnvironmentVariable("AZURESTATICWEBAPP_STATIC_WEB_APP_URL");
            if (!string.IsNullOrEmpty(siteBaseUrlVar))
            {
                siteBaseUrl = siteBaseUrlVar;
            }
            System.Console.WriteLine($"Using siteBaseUrl: {siteBaseUrl}");
        }

        [Test]
        public async Task ShouldLoadHomepage()
        {
            await using var browser = await Playwright.Chromium.LaunchAsync();
            var page = await browser.NewPageAsync();
            await page.GotoAsync($"{siteBaseUrl}/");
            await page.Locator("text=Hello, world!").WaitForAsync();
            var title = await page.TitleAsync();
            Assert.AreEqual("Index", title);
        }

        [Test]
        public async Task ShouldLoadWeather()
        {
            await using var browser = await Playwright.Chromium.LaunchAsync();
            var page = await browser.NewPageAsync();
            await page.GotoAsync($"{siteBaseUrl}/");
            await page.ClickAsync("a[href='fetchdata']");

            var h1 = await page.QuerySelectorAsync("div#app main h1");
            var h1Text = h1 == null ? "" : await h1.TextContentAsync();
            Assert.AreEqual("Weather forecast", h1Text);

            var rowsSelector = "div#app main table tbody tr";
            // wait for table to have rows
            await page.WaitForFunctionAsync($"document.querySelectorAll('{rowsSelector}').length");
            var rows = await page.QuerySelectorAllAsync(rowsSelector);
            Assert.AreEqual(5, rows.Count);
        }
    }
}