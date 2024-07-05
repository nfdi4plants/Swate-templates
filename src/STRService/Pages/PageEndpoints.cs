using STRService.Pages.Handlers;

namespace STRService.Pages
{
    public static class PageEndpoints
    {
        public static RouteGroupBuilder MapPageEndpoints(this RouteGroupBuilder group)
        {
            group.MapGet("", IndexHandlers.Render);

            group.MapGet("about", AboutHandlers.Render);

            group.MapGet("templates", TemplatesHandlers.Render);

            group.MapGet("template/{templateName}", TemplateHandlers.RenderLatest);

            group.MapGet("template/{templateName}/{version}", TemplateHandlers.Render);

            return group;
        }
    }
}
