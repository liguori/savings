using Microsoft.AspNetCore.Components;
using SavingsProjection.Model;
using Radzen;
using SavingsProjection.SPA.Services;
using System.Threading.Tasks;

namespace SavingsProjection.SPA.Pages
{
    public partial class RecurrencyAdjustment : ComponentBase
    {
        [Inject]
        public ISavingProjectionApi savingProjectionAPI { get; set; }

        [Inject]
        DialogService dialogService { get; set; }

        [Parameter]
        public MaterializedMoneyItem materializedItem { get; set; }

        [Parameter]
        public RecurrencyAdjustement adjustement { get; set; } = new RecurrencyAdjustement();

        protected override async Task OnInitializedAsync()
        {
            var existentAdjustment = await this.savingProjectionAPI.GetRecurrencyAdjustementByIDRecurrency(materializedItem.RecurrentMoneyItemID.Value);
            this.adjustement = existentAdjustment ?? new RecurrencyAdjustement() { RecurrentMoneyItemID = materializedItem.RecurrentMoneyItemID.Value, RecurrencyDate = materializedItem.Date };
        }

        private async void OnValidSubmit()
        {

        }

    }
}
