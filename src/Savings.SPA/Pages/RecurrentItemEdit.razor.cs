using Microsoft.AspNetCore.Components;
using Radzen;
using Savings.Model;
using Savings.SPA.Services;

namespace Savings.SPA.Pages
{
    public partial class RecurrentItemEdit : ComponentBase
    {
        [Inject]
        public ISavingsApi savingsAPI { get; set; }

        [Inject]
        DialogService dialogService { get; set; }

        [Inject]
        public NotificationService notificationService { get; set; }

        [Parameter]
        public RecurrentMoneyItem recurrentItemToEdit { get; set; }

        public MoneyCategory[] Categories { get; set; }

        [Parameter]
        public bool isNew { get; set; }

        [Parameter]
        public long? parentItemID { get; set; } = null;

        protected override void OnInitialized()
        {
            if (isNew)
            {
                this.recurrentItemToEdit.StartDate = DateTime.Now.Date;
            }
            recurrentItemToEdit.RecurrentMoneyItemID = parentItemID;
        }

        protected override async Task OnInitializedAsync()
        {
            Categories = await savingsAPI.GetMoneyCategories();
        }

        async Task Delete()
        {
            try
            {
                var res = await dialogService.Confirm("Are you sure you want delete?", "Delete recurrent item", new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });
                if (res.HasValue && res.Value)
                {
                    var deletedItem = await savingsAPI.DeleteRecurrentMoneyItem(recurrentItemToEdit.ID);
                    this.dialogService.Close(true);
                }
            }
            catch (Exception ex)
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", ex.Message);
            }
        }


        bool ValidateData()
        {
            if(recurrentItemToEdit.Amount>0 && recurrentItemToEdit.Type== MoneyType.PeriodicBudget)
            {
                notificationService.Notify(NotificationSeverity.Error, "Attention", "The amount for the periodic budget must be negative");
                return false;
            }
            if (recurrentItemToEdit.CategoryID == null)
            {
                notificationService.Notify(NotificationSeverity.Error, "Attention", "Category is mandatory field");
                return false;
            }
            return true;
        }

        private async void OnValidSubmit()
        {
            try
            {
                if (!ValidateData()) return;
                if (isNew)
                {
                    await savingsAPI.InsertRecurrentMoneyItem(recurrentItemToEdit);
                }
                else
                {
                    await savingsAPI.EditRecurrentMoneyItem(recurrentItemToEdit.ID, recurrentItemToEdit);
                }
                this.dialogService.Close(true);
            }
            catch
            {
                throw ;
            }

        }
    }
}
