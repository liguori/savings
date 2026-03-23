using Microsoft.Extensions.Configuration;
using Refit;
using Savings.Model;

namespace Savings.SPA.Services
{
    public class FederationService : IFederationService
    {
        private readonly IReadOnlyList<FederationEndpoint> _endpoints;
        private readonly IHttpClientFactory _httpClientFactory;

        public FederationService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            var endpoints = new List<FederationEndpoint>();
            configuration.GetSection("FederationEndpoints").Bind(endpoints);
            _endpoints = endpoints.AsReadOnly();
        }

        public IReadOnlyList<FederationEndpoint> Endpoints => _endpoints;

        public bool IsEnabled => _endpoints.Count > 0;

        public async Task<FederatedProjectionResult[]> GetFederatedProjectionsAsync(DateTime? from, DateTime? to)
        {
            var tasks = _endpoints.Select(async endpoint =>
            {
                try
                {
                    var client = _httpClientFactory.CreateClient($"Federation_{endpoint.Name}");
                    var api = RestService.For<IFederationApi>(client);
                    var items = await api.GetSavings(from, to);
                    return new FederatedProjectionResult
                    {
                        EndpointName = endpoint.Name,
                        Items = items
                    };
                }
                catch (Exception ex)
                {
                    return new FederatedProjectionResult
                    {
                        EndpointName = endpoint.Name,
                        Error = ex.Message
                    };
                }
            });

            return await Task.WhenAll(tasks);
        }
    }
}
