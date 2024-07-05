﻿using Microsoft.AspNetCore.Http.HttpResults;
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

            group.MapGet("/{name}", TemplateHandlers.GetLatestTemplateByName)
                .WithOpenApi()
                .WithName("GetLatestTemplateByName");

            group.MapGet("/{name}/{version}", TemplateHandlers.GetTemplateByNameAndVersion)
                .WithOpenApi()
                .WithName("GetTemplateByNameAndVersion");

            group.MapPost("/", TemplateHandlers.CreateTemplate)
                .WithOpenApi()
                .WithName("CreateTemplate")
                .AddEndpointFilter<APIKeyEndpointFilter>(); // creating templates via post requests requires an API key

            return group.WithTags("Swate Templates (Full json with table content)");
        }
    }
}
