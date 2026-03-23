using Microsoft.AspNetCore.Components;
using Savings.Model;
using Savings.SPA.Services;

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
                Url = NewEndpointUrl.Trim()
            };

            await savingsAPI.InsertFederationEndpoint(endpoint);
            FederationEndpoints = await savingsAPI.GetFederationEndpoints();
            NewEndpointName = string.Empty;
            NewEndpointUrl = string.Empty;
            StateHasChanged();
        }

        private async Task DeleteEndpoint(FederationEndpoint endpoint)
        {
            await savingsAPI.DeleteFederationEndpoint(endpoint.ID);
            FederationEndpoints = await savingsAPI.GetFederationEndpoints();
            StateHasChanged();
        }

    }

}
