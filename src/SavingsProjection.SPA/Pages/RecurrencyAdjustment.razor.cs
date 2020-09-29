using Microsoft.AspNetCore.Components;
using SavingsProjection.Model;
using Radzen;
using SavingsProjection.SPA.Services;
using System.Threading.Tasks;
using System.Reflection;

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

        public bool isNew { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var existentAdjustment = await this.savingProjectionAPI.GetRecurrencyAdjustementByIDRecurrency(materializedItem.RecurrentMoneyItemID.Value);
            if (existentAdjustment != null)
            {
                this.adjustement = existentAdjustment;
                isNew = false;
            }
            else
            {
                this.adjustement = new RecurrencyAdjustement() { RecurrentMoneyItemID = materializedItem.RecurrentMoneyItemID.Value, RecurrencyDate = materializedItem.Date };
                isNew = true;
            }
        }

        private async void OnValidSubmit()
        {
            if (!adjustement.RecurrencyNewDate.HasValue && !adjustement.RecurrencyNewAmount.HasValue && !isNew)
            {
                await this.savingProjectionAPI.DeleteRecurrencyAdjustment(adjustement.ID);
            }
            else
            {
                if (isNew)
                {
                    await this.savingProjectionAPI.InsertRecurrencyAdjustment(adjustement);
                }
                else
                {
                    await this.savingProjectionAPI.EditRecurrencyAdjustment(adjustement.ID, adjustement);
                }
            }
            this.dialogService.Close(true);
        }

    }
}
