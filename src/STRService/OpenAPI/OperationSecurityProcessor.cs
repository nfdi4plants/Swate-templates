using Microsoft.AspNetCore.Authorization;
using NSwag.Generation.Processors.Contexts;
using NSwag.Generation.Processors;
using NSwag;

namespace STRService.OpenAPI
{
    public class OperationSecurityProcessor(string[] secureEndpointIds) : IOperationProcessor
    {

        public bool Process(OperationProcessorContext operationProcessorContext)
        {
            foreach (OpenApiOperationDescription operationDescription in operationProcessorContext.AllOperationDescriptions)
            {
                if (secureEndpointIds.Contains(operationDescription.Operation.OperationId))
                {
                    operationDescription.Operation.Security = new OpenApiSecurityRequirement[]
                    {
                        new OpenApiSecurityRequirement
                        {
                            {
                                "ApiKey", new string[] { }
                            }
                        }

                    };
                }

            }

            return true;
        }

    }
}
