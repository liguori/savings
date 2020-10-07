using Microsoft.AspNetCore.Components;
using Radzen;
using SavingsProjection.Model;
using SavingsProjection.SPA.Services;
using System;
using System.Threading.Tasks;

namespace SavingsProjection.SPA.Pages
{
    public partial class RecurrentItemEdit : ComponentBase
    {
        [Inject]
        public ISavingProjectionApi savingProjectionAPI { get; set; }

        [Inject]
        DialogService dialogService { get; set; }

        [Inject]
        public NotificationService notificationService { get; set; }

        [Parameter]
        public RecurrentMoneyItem recurrentItemToEdit { get; set; }

        public MoneyCategory[] Categories { get; set; }

        [Parameter]
        public bool isNew { get; set; }

        [ParameterAttribute]
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
            Categories = await savingProjectionAPI.GetMoneyCategories();
        }

        bool ValidateData()
        {
            if(recurrentItemToEdit.Amount>0 && recurrentItemToEdit.Type== MoneyType.PeriodicBudget)
            {
                notificationService.Notify(NotificationSeverity.Error, "Attention", "The amount for the periodic budget must be negative");
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
                    await savingProjectionAPI.InsertRecurrentMoneyItem(recurrentItemToEdit);
                }
                else
                {
                    await savingProjectionAPI.EditRecurrentMoneyItem(recurrentItemToEdit.ID, recurrentItemToEdit);
                }
                this.dialogService.Close(true);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
