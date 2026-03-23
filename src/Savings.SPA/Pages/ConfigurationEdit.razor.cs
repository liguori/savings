using Microsoft.AspNetCore.Components;
using Savings.Model;
using Savings.SPA.Services;
using System.Security.Cryptography;

namespace Savings.SPA.Pages
{
    public partial class ConfigurationEdit : ComponentBase
    {

        [Inject]
        private ISavingsApi savingsAPI { get; set; } = default!;

        private Configuration Configuration { get; set; } = default!;

        public MoneyCategory[] Categories { get; set; } = default!;

        public DateTime LastMaterializedDate { get; set; }

        public decimal LastMaterializedAmount { get; set; }

        public FederationEndpoint[]? FederationEndpoints { get; set; }

        public string NewEndpointName { get; set; } = string.Empty;

        public string NewEndpointUrl { get; set; } = string.Empty;

        public string NewEndpointApiKey { get; set; } = string.Empty;

        public FederationApiKey[]? FederationApiKeys { get; set; }

        public string NewApiKeyDescription { get; set; } = string.Empty;

        public string NewApiKeyValue { get; set; } = string.Empty;

        bool ValidateData()
        {

            return true;
        }

        protected override async Task OnInitializedAsync()
        {
            var lastMaterializedItem = await savingsAPI.GetLastMaterializedMoneyItemPeriod();
            LastMaterializedDate = lastMaterializedItem.Date;
            LastMaterializedAmount = lastMaterializedItem.Projection;
            Categories = await savingsAPI.GetMoneyCategories();
            Configuration = (await savingsAPI.GetConfigurations()).First();
            FederationEndpoints = await savingsAPI.GetFederationEndpoints();
            FederationApiKeys = await savingsAPI.GetFederationApiKeys();
            GenerateRandomKey();
        }

        private async void OnValidSubmit()
        {
            try
            {
                if (!ValidateData()) return;
                await savingsAPI.EditLastMaterializedMoneyItemPeriod(LastMaterializedDate,LastMaterializedAmount);
                await savingsAPI.PutConfiguration(Configuration.ID, Configuration);
            }
            catch
            {
                throw;
            }

        }

        private async Task AddEndpoint()
        {
            if (string.IsNullOrWhiteSpace(NewEndpointName) || string.IsNullOrWhiteSpace(NewEndpointUrl)) return;

            var endpoint = new FederationEndpoint
            {
                Name = NewEndpointName.Trim(),
                Url = NewEndpointUrl.Trim(),
                ApiKey = string.IsNullOrWhiteSpace(NewEndpointApiKey) ? null : NewEndpointApiKey.Trim()
            };

            await savingsAPI.InsertFederationEndpoint(endpoint);
            FederationEndpoints = await savingsAPI.GetFederationEndpoints();
            NewEndpointName = string.Empty;
            NewEndpointUrl = string.Empty;
            NewEndpointApiKey = string.Empty;
            StateHasChanged();
        }

        private async Task DeleteEndpoint(FederationEndpoint endpoint)
        {
            await savingsAPI.DeleteFederationEndpoint(endpoint.ID);
            FederationEndpoints = await savingsAPI.GetFederationEndpoints();
            StateHasChanged();
        }

        private void GenerateRandomKey()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            NewApiKeyValue = Convert.ToHexString(bytes).ToLowerInvariant();
        }

        private async Task AddApiKey()
        {
            if (string.IsNullOrWhiteSpace(NewApiKeyDescription) || string.IsNullOrWhiteSpace(NewApiKeyValue)) return;

            var apiKey = new FederationApiKey
            {
                Key = NewApiKeyValue.Trim(),
                Description = NewApiKeyDescription.Trim()
            };

            await savingsAPI.InsertFederationApiKey(apiKey);
            FederationApiKeys = await savingsAPI.GetFederationApiKeys();
            NewApiKeyDescription = string.Empty;
            GenerateRandomKey();
            StateHasChanged();
        }

        private async Task DeleteApiKey(FederationApiKey apiKey)
        {
            await savingsAPI.DeleteFederationApiKey(apiKey.ID);
            FederationApiKeys = await savingsAPI.GetFederationApiKeys();
            StateHasChanged();
        }

    }

}
