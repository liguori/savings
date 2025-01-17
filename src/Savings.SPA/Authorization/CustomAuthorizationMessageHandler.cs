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

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                return await base.SendAsync(request, cancellationToken);
            }
            catch (AccessTokenNotAvailableException ex)
            {
                ex.Redirect();
                await Task.Delay(3000);//Wait the redirect to complete before to throw the exception
                throw;
            }
        }

    }
}