using Microsoft.AspNetCore.Http.HttpResults;
using STRService.Authentication;
using Microsoft.AspNetCore.Mvc;
using STRService.API.Handlers;

namespace STRService.API.Endpoints
{
    public static class TemplateEndpointsV1
    {
        public static RouteGroupBuilder MapTemplateApiV1(this RouteGroupBuilder group)
        {

            // templates endpoints
            group.MapGet("/", TemplateHandlers.GetAllTemplates)
                .WithOpenApi()
                .WithName("GetAllTemplates");

            group.MapGet("/{id}", TemplateHandlers.GetLatestTemplateById)
                .WithOpenApi()
                .WithName("GetLatestTemplateById");

            group.MapGet("/{id}/{version}", TemplateHandlers.GetTemplateByIdAndVersion)
                .WithOpenApi()
                .WithName("GetTemplateByIdAndVersion");

            group.MapPost("/", TemplateHandlers.CreateTemplate)
                .WithOpenApi()
                .WithName("CreateTemplate")
                .AddEndpointFilter<APIKeyEndpointFilter>(); // creating templates via post requests requires an API key

            return group.WithTags("Swate Templates (Full json with table content)");
        }
    }
}
