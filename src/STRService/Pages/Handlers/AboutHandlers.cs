using Microsoft.AspNetCore.Http.HttpResults;
using STRService.Models;
using STRService.Pages.Components;

namespace STRService.Pages.Handlers
{
    public static class AboutHandlers
    {
        public static async Task<ContentHttpResult> Render()
        {

            var content =
                Layout.Render(
                    activeNavbarItem: "About",
                    title: "ARC Swate template registry API",
                    content: About.Render()
                );

            return TypedResults.Text(content: content, contentType: "text/html");

        }
    }

}