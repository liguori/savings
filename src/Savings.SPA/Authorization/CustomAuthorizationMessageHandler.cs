using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Savings.SPA.Authorization
{
    public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler
    {
        private readonly ILogger<CustomAuthorizationMessageHandler> _logger;
        public CustomAuthorizationMessageHandler(IConfiguration configuration, IAccessTokenProvider provider,
            NavigationManager navigationManager, ILogger<CustomAuthorizationMessageHandler> logger)
            : base(provider, navigationManager)
        {
            _logger = logger;
            ConfigureHandler(
                authorizedUrls: new[] { configuration["SavingsApiServiceUrl"] ?? throw new ArgumentNullException("SavingsApiServiceUrl") });
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Invoking the request...");
                return base.SendAsync(request, cancellationToken);
            }
            catch (AccessTokenNotAvailableException ex)
            {
                _logger.LogInformation("Access token not available. Redirecting to login...");
                ex.Redirect();
                throw new Exception("Authentication has expired. Redirecting to login...");
            }
        }

    }
}