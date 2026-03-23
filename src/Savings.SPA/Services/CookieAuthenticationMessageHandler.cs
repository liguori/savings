using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace Savings.SPA.Services
{
    public class CookieAuthenticationMessageHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
