using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
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
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                document.Components.SecuritySchemes.Add("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Complete the authorization flow wit Entra ID",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(conf["IdentityProvider:Authority"] + "/oauth2/v2.0/authorize"),
                            TokenUrl = new Uri(conf["IdentityProvider:Authority"] + "oauth2/v2.0/token"),
                            Scopes = new Dictionary<string, string> { { conf["IdentityProvider:Audience"], "Read access" } }
                        }
                    }

                });
            }
            else if (authenticationToUse == AuthenticationToUse.ApiKey)
            {
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
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
