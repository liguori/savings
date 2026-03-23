using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Refit;
using Savings.Model;

namespace Savings.SPA.Services
{
    public class FederationService : IFederationService
    {
        private readonly ISavingsApi _savingsApi;
        private readonly HttpClient _httpClient;

        public FederationService(ISavingsApi savingsApi, HttpClient httpClient)
        {
            _savingsApi = savingsApi;
            _httpClient = httpClient;
        }

        public async Task<FederationEndpoint[]> GetEndpointsAsync()
        {
            return await _savingsApi.GetFederationEndpoints();
        }

        public async Task<FederatedProjectionResult[]> GetFederatedProjectionsAsync(DateTime? from, DateTime? to)
        {
            var endpoints = await _savingsApi.GetFederationEndpoints();

            var tasks = endpoints.Select(async endpoint =>
            {
                try
                {
                    var client = new HttpClient(new CookieIncludeHandler { InnerHandler = new HttpClientHandler() });
                    client.BaseAddress = new Uri(endpoint.Url);
                    var api = RestService.For<IFederationApi>(client);
                    var items = await api.GetSavings(from, to);
                    return new FederatedProjectionResult
                    {
                        EndpointName = endpoint.Name,
                        Items = items
                    };
                }
                catch (Exception)
                {
                    return new FederatedProjectionResult
                    {
                        EndpointName = endpoint.Name,
                        Error = $"Unable to reach endpoint '{endpoint.Name}'"
                    };
                }
            });

            return await Task.WhenAll(tasks);
        }
    }

    internal class CookieIncludeHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            return base.SendAsync(request, cancellationToken);
        }
    }
}
