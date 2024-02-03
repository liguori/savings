using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using Radzen.Blazor.Rendering;
using Savings.Model;
using Savings.SPA.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        [Parameter]
        public RecurrentMoneyItem parentItem { get; set; } = null;

        InputNumber<decimal> amountInputNumber;

        public bool OperationRunning { get; set; } = false;


        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                await Task.Delay(500);
                await amountInputNumber.Element.Value.FocusAsync();
            }
        }

        protected override void OnInitialized()
        {
            if (isNew)
            {
                this.recurrentItemToEdit.StartDate = DateTime.Now.Date;
                if (parentItemID.HasValue)
                {
                    DateTime targetDate = DateTime.Now.AddMonths(1);
                    targetDate = new DateTime(targetDate.Year, targetDate.Month, parentItem.StartDate.Day);

                    recurrentItemToEdit.StartDate = targetDate;
                    recurrentItemToEdit.EndDate = targetDate;
                }
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
                OperationRunning = true;
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
            finally
            {
                OperationRunning = false;
            }
        }


        bool ValidateData()
        {
            if (recurrentItemToEdit.Amount > 0 && recurrentItemToEdit.Type == MoneyType.PeriodicBudget)
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
                OperationRunning = true;
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
                throw;
            }
            finally
            {
                OperationRunning = false;
            }

        }
    }
}
