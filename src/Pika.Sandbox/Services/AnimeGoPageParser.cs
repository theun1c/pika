using System.Runtime.InteropServices.ComTypes;
using System.Xml;
using Microsoft.Playwright;

namespace Pika.Sandbox.Models;

public class AnimeGoPageParser
{
    public async Task<PageParseResult> ParseAsync(string url)
    {
        using var playwright = await Playwright.CreateAsync();

        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions{
            Headless = true
        });

        var page = await browser.NewPageAsync();
        await page.GotoAsync(url);

        var playerBlock = page.Locator(".player-video__online");
        await playerBlock.ScrollIntoViewIfNeededAsync();
        
        var frameLocator = page.FrameLocator("iframe[src*='kodik.info']");
        var playButton = frameLocator.Locator("a.play_button");
        
        await playButton.ClickAsync();
        
        var responseTask = page.WaitForResponseAsync(r => r.Url.Contains(".m3u8") && r.Status == 200);
        
        var response = await responseTask;

        var m3u8 = response.Url;
        
        Console.WriteLine($"URL: {m3u8}");
        
        return new PageParseResult(m3u8);
    }
}