using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace STRService.OpenAPI
{
    // we do not need view endpoints in our OpenAPI documentation
    public class OperationFilterProcessor : IOperationProcessor
    {
        public bool Process(OperationProcessorContext operationProcessorContext)
        {
            return operationProcessorContext.OperationDescription.Path.StartsWith("/api/");
        }
    }
}
