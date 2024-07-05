using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors;

namespace STRService.OpenAPI
{
    public class DocGen
    {
        public static void GeneratorSetup (AspNetCoreOpenApiDocumentGeneratorSettings settings)
        {
            settings.Title = "Swate template registry API";
            settings.Version = "v1";
            settings.Description = "A simple API for retrieving Swate templates";

            // add a security definition for API key authentication
            settings.AddSecurity(
                name: "ApiKey",
                new NSwag.OpenApiSecurityScheme
                {
                    Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
                    Name = "X-API-KEY",
                    In = NSwag.OpenApiSecurityApiKeyLocation.Header,
                    Description = "API Key required to access POST endpoints"
                }
            );

            // manually change security for selected endpoints, was not able to find a more automated way for minimal APIs with NSwag and this exact setup. 
            // For e.g. JWT, this would work because we could rely on full blown authorization service/config, but this is a simple API key checked via endpoint filters
            settings.OperationProcessors.Add(
                new OperationSecurityProcessor(
                    secureEndpointIds: ["CreateSwateTemplate"]
                )
            );
            // fix for WithDescription and WithSummary methods not working with nswag and minimal API endpoints
            settings.OperationProcessors.Add(new OperationMetadataProcessor());

            // only inclide "api" paths in docs
            settings.OperationProcessors.Add(new OperationFilterProcessor());
        }
    }
}
