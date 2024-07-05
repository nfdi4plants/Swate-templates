using Microsoft.AspNetCore.Http.HttpResults;
using STRService.Models;
using STRService.Pages.Components;

namespace STRService.Pages.Handlers
{
    public static class IndexHandlers
    {
        public static async Task<ContentHttpResult> Render()
        {

            var content =
                Layout.Render(
                    activeNavbarItem: "Home",
                    title: "STR: ARC Swate template registry",
                    content: Components.Index.Render()
                );

            return TypedResults.Text(content: content, contentType: "text/html");

        }
    }

}