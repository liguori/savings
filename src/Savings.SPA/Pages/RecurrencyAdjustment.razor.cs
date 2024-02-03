using Microsoft.AspNetCore.Components;
using Radzen;
using Savings.Model;
using Savings.SPA.Services;

namespace Savings.SPA.Pages
{
    public partial class RecurrencyAdjustment : ComponentBase
    {
        [Inject]
        public ISavingsApi savingsAPI { get; set; }

        [Inject]
        DialogService dialogService { get; set; }

        [Parameter]
        public MaterializedMoneyItem materializedItem { get; set; }

        [Parameter]
        public RecurrencyAdjustement adjustement { get; set; } = new RecurrencyAdjustement();

        public bool isNew { get; set; }

        public bool OperationRunning { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            var existentAdjustment = await this.savingsAPI.GetRecurrencyAdjustementByIDRecurrencyAndDate(materializedItem.RecurrentMoneyItemID.Value, materializedItem.Date.Date);
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
            try
            {
                OperationRunning = true;
                if (!adjustement.RecurrencyNewDate.HasValue && !adjustement.RecurrencyNewAmount.HasValue && !isNew)
                {
                    await this.savingsAPI.DeleteRecurrencyAdjustment(adjustement.ID);
                }
                else
                {
                    if (isNew)
                    {
                        await this.savingsAPI.InsertRecurrencyAdjustment(adjustement);
                    }
                    else
                    {
                        await this.savingsAPI.EditRecurrencyAdjustment(adjustement.ID, adjustement);
                    }
                }
                this.dialogService.Close(true);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                OperationRunning = false;
            }
        }

    }
}
