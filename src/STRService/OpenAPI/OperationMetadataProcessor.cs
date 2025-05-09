using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace STRService.OpenAPI
{
    public class OperationMetadataProcessor : IOperationProcessor
    {
        public Dictionary<string, Dictionary<string,string>> EndpointMetadata = new Dictionary<string, Dictionary<string,string>>
        {
            {
                "CreateTemplate", new Dictionary<string, string>
                {
                    { "Summary", "Submit a new Swate template" },
                    { "Description", "Submit a new Swate template to the template registry. This Endpoint requires API Key authentication." }
                }
            },
            {
                "GetAllTemplates", new Dictionary<string, string>
                {
                    { "Summary", "Get all Swate templates" },
                    { "Description", "Get all Swate templates from the template registry. Note that this endpoint returns all versions of each template. Template content is a base64 encoded byte array containing the template executable." }
                }
            },
            {
                "GetLatestTemplateById", new Dictionary<string, string>
                {
                    { "Summary", "Get the latest version of a Swate template" },
                    { "Description", "Get the latest version of a Swate template from the template registry. Template content is a base64 encoded byte array containing the template executable." }
                }
            },
            {
                "GetTemplateByIdAndVersion", new Dictionary<string, string>
                {
                    { "Summary", "Get a specific version of a Swate template" },
                    { "Description", "Get a specific version of a Swate template from the template registry. Template content is a base64 encoded byte array containing the template executable." }
                }
            },
            {
                "GetAllTemplateMetadata", new Dictionary<string, string>
                {
                    { "Summary", "Get metadata for all Swate templates" },
                    { "Description", "Get all Swate templates from the template registry. Note that this endpoint returns all versions of each template. Template content is a base64 encoded byte array containing the template executable." }
                }
            },
            {
                "GetLatestTemplateMetadataById", new Dictionary<string, string>
                {
                    { "Summary", "Get the metadata for latest version of a Swate template" },
                    { "Description", "Get the latest version of a Swate template from the template registry. Template content is a base64 encoded byte array containing the template executable." }
                }
            },
            {
                "GetTemplateMetadataByIdAndVersion", new Dictionary<string, string>
                {
                    { "Summary", "Get metadata for a specific version of a Swate template" },
                    { "Description", "Get a specific version of a Swate template from the template registry. Template content is a base64 encoded byte array containing the template executable." }
                }
            },
            {
                "GetAllDownloads", new Dictionary<string, string>
                {
                    { "Summary", "Get all download statistics" },
                    { "Description", "Get all download statistics for all Swate templates." }
                }
            },
            {
                "GetAllDownloadsById", new Dictionary<string, string>
                {
                    { "Summary", "Get download statistics for all versions specific template" },
                    { "Description", "Get download statistics for all versions specific template" }
                }
            },
            {
                "GetDownloadsByIdAndVersion", new Dictionary<string, string>
                {
                    { "Summary", "Get download statistics for a specific version of a specific template" },
                    { "Description", "Get download statistics for a specific version of a specific template" }
                }
            }
        };

        public bool Process(OperationProcessorContext operationProcessorContext)
        {
            foreach (OpenApiOperationDescription operationDescription in operationProcessorContext.AllOperationDescriptions)
            {
                var op = operationDescription.Operation;
                if (EndpointMetadata.ContainsKey(op.OperationId)) {
                    op.Summary = EndpointMetadata[op.OperationId]["Summary"];
                    op.Description = EndpointMetadata[op.OperationId]["Description"];
                }
            }

            return true;
        }

    }
}
