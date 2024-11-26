using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using Savings.API.Authentication;
using Savings.Model;

namespace Savings.API.OpenApi
{
    internal sealed class DocumentSecuritySchemeTransformer(IConfiguration conf) : IOpenApiDocumentTransformer
    {
        
        public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            var authenticationToUse = conf["AuthenticationToUse"];

            document.Info = new OpenApiInfo { Title = "Savings", Version = "v1" };
            if (authenticationToUse == AuthenticationToUse.AzureAD)
            {
                document.Components = new OpenApiComponents();
                document.Components.SecuritySchemes.Add(ApiKeyAuthOptions.ApiKeySchemaName, new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.OAuth2
                });
            }
            else if (authenticationToUse == AuthenticationToUse.ApiKey)
            {
                document.Components = new OpenApiComponents();
                document.Components.SecuritySchemes.Add(ApiKeyAuthOptions.ApiKeySchemaName, new OpenApiSecurityScheme
                {
                    Description = "Api key needed to access the endpoints. " + ApiKeyAuthOptions.HeaderName + ": My_API_Key",
                    In = ParameterLocation.Header,
                    Name = ApiKeyAuthOptions.HeaderName,
                    Type = SecuritySchemeType.ApiKey
                });
            }

            return Task.CompletedTask;
        }
    }
}
