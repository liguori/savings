using Microsoft.AspNetCore.Components;
using Savings.Model;
using Savings.SPA.Services;

namespace Savings.SPA.Pages
{
    public partial class ConfigurationEdit : ComponentBase
    {

        [Inject]
        private ISavingsApi savingsAPI { get; set; }

        private Configuration Configuration { get; set; }

        public MoneyCategory[] Categories { get; set; }

        public DateTime LastMaterializedDate { get; set; }

        public decimal LastMaterializedAmount { get; set; }

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

    }

}
