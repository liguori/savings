using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Savings.API.Authentication;
using Savings.Model;

namespace Savings.API.OpenApi
{
    internal sealed class OperationSecurityTransformer(IConfiguration conf) : IOpenApiOperationTransformer
    {
        public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            var authenticationToUse = conf["AuthenticationToUse"];

            if (authenticationToUse == AuthenticationToUse.AzureAD)
            {
                operation.Security = [
                     new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecuritySchemeReference("Bearer",context.Document),
                                [ ]
                            }
                        }
                 ];
            }
            else if (authenticationToUse == AuthenticationToUse.ApiKey)
            {
                operation.Security = [
                     new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecuritySchemeReference(ApiKeyAuthOptions.ApiKeySchemaName,context.Document),
                                [ ]
                            }
                        }
                 ];
            }
            return Task.CompletedTask;
        }
    }
}
