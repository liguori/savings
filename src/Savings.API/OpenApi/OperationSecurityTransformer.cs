using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
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
                operation.Security = [new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                         {
                           Reference = new OpenApiReference
                           {
                             Type = ReferenceType.SecurityScheme,
                             Id = "Bearer"
                           }
                          },
                         new string[] {}
                     }
                }];
            }
            else if (authenticationToUse == AuthenticationToUse.ApiKey)
            {
                operation.Security = [new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Name = ApiKeyAuthOptions.HeaderName,
                            Type = SecuritySchemeType.ApiKey,
                            In = ParameterLocation.Header,
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = ApiKeyAuthOptions.ApiKeySchemaName
                            },
                         },
                         new string[] {}
                     }
                }];
            }
            return Task.CompletedTask;
        }
    }
}
