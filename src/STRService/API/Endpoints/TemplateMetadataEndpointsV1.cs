using Microsoft.AspNetCore.Http.HttpResults;
using STRService.Authentication;
using Microsoft.AspNetCore.Mvc;
using STRService.API.Handlers;
 
namespace STRService.API.Endpoints
{ 
    public static class TemplateMetadataEndpointsV1
    {
        public static RouteGroupBuilder MapTemplateMetadataApiV1(this RouteGroupBuilder group)
        {

            // templates endpoints
            group.MapGet("/", TemplateMetadataHandlers.GetAllTemplateMetadata)
                .WithOpenApi()
                .WithName("GetAllTemplateMetadata");

            group.MapGet("/{id}", TemplateMetadataHandlers.GetLatestTemplateMetadataById)
                .WithOpenApi()
                .WithName("GetLatestTemplateMetadataByName");

            group.MapGet("/{id}/{version}", TemplateMetadataHandlers.GetTemplateMetadataByIdAndVersion)
                .WithOpenApi()
                .WithName("GetTemplateMetadataByNameAndVersion");

            return group.WithTags("Swate Template Metadata");
        }
    }
}
