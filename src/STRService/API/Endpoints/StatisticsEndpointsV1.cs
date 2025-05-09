using STRService.API.Handlers;
using STRService.Authentication;

namespace STRService.API.Endpoints
{
    public static class StatisticsEndpointsV1
    {
        public static RouteGroupBuilder MapStatisticsApiV1(this RouteGroupBuilder group)
        {

            // templates endpoints
            group.MapGet("/downloads", DownloadsHandlers.GetAllDownloads)
                .WithOpenApi()
                .WithName("GetAllDownloads");

            group.MapGet("/downloads/{id}", DownloadsHandlers.GetAllDownloadsById)
                .WithOpenApi()
                .WithName("GetAllDownloadsById");

            group.MapGet("/downloads/{id}/{version}", DownloadsHandlers.GetDownloadsByIdAndVersion)
                .WithOpenApi()
                .WithName("GetDownloadsByIdAndVersion");

            return group.WithTags("Statistics"); ;
        }
    }
}
