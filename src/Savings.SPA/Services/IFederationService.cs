using Savings.Model;

namespace Savings.SPA.Services
{
    public interface IFederationService
    {
        Task<FederationEndpoint[]> GetEndpointsAsync();
        Task<FederatedProjectionResult[]> GetFederatedProjectionsAsync(DateTime? from, DateTime? to);
    }

    public class FederatedProjectionResult
    {
        public string EndpointName { get; set; } = string.Empty;
        public MaterializedMoneyItem[] Items { get; set; } = Array.Empty<MaterializedMoneyItem>();
        public string? Error { get; set; }
    }
}
