using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using Savings.Model;
using Savings.SPA.Services;

namespace Savings.SPA.Pages
{
    public partial class FixedItemEdit : ComponentBase
    {

        [Inject]
        public ISavingsApi savingsAPI { get; set; }

        [Inject]
        DialogService dialogService { get; set; }

        [Inject]
        public NotificationService notificationService { get; set; }

        [Parameter]
        public FixedMoneyItem fixedItemToEdit { get; set; }

        public bool Incoming { get; set; }

        [Parameter]
        public bool isNew { get; set; }

        bool Credit { get; set; } = false;

        public MoneyCategory[] Categories { get; set; }

        InputNumber<decimal?> amountInputNumber;

        protected override void OnInitialized()
        {
            if (isNew)
            {
                this.fixedItemToEdit.Date = DateTime.UtcNow.Date;
                this.fixedItemToEdit.Amount = null;
                this.fixedItemToEdit.AccumulateForBudget = true;
            }
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                await Task.Delay(500);
                await amountInputNumber.Element.Value.FocusAsync();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            Categories = await savingsAPI.GetMoneyCategories();
            Incoming = fixedItemToEdit.Amount > 0;
        }

        bool ValidateData()
        {
            if (fixedItemToEdit.Amount == null || fixedItemToEdit.Amount == 0)
            {
                notificationService.Notify(NotificationSeverity.Error, "Attention", "The amount must contain a value and be different than 0");
                return false;
            }
            if (fixedItemToEdit.CategoryID == null)
            {
                notificationService.Notify(NotificationSeverity.Error, "Attention", "Category is mandatory field");
                return false;
            }
            return true;
        }

        async Task Delete()
        {
            var res = await dialogService.Confirm("Are you sure you want delete?", "Delete fixed item", new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });
            if (res.HasValue && res.Value)
            {
                var deletedItem = await savingsAPI.DeleteFixedMoneyItem(fixedItemToEdit.ID);
                this.dialogService.Close(true);
            }
        }


        private async void OnValidSubmit()
        {
            try
            {
                if (!ValidateData()) return;
                if (Incoming)
                {
                    fixedItemToEdit.Amount = Math.Abs(fixedItemToEdit.Amount.Value);
                }
                else
                {
                    fixedItemToEdit.Amount = -Math.Abs(fixedItemToEdit.Amount.Value);
                }
                if (isNew)
                {
                    if (Credit)
                    {
                        await savingsAPI.InsertCreditFixedMoneyItem(fixedItemToEdit);
                    }
                    else
                    {
                        await savingsAPI.InsertFixedMoneyItem(fixedItemToEdit);
                    }
                }
                else
                {
                    await savingsAPI.EditFixedMoneyItem(fixedItemToEdit.ID, fixedItemToEdit);
                }
                this.dialogService.Close(true);
            }
            catch(Exception ex)
            {
                notificationService.Notify(NotificationSeverity.Error, "Attention", ex.Message);
            }

        }

    }
}
