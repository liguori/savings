using Microsoft.AspNetCore.Components;
using Radzen;
using SavingsProjection.Model;
using SavingsProjection.SPA.Services;
using System;
using System.Threading.Tasks;

namespace SavingsProjection.SPA.Pages
{
    public partial class FixedItemEdit : ComponentBase
    {

        [Inject]
        public ISavingProjectionApi savingProjectionAPI { get; set; }

        [Inject]
        DialogService dialogService { get; set; }

        [Parameter]
        public FixedMoneyItem fixedItemToEdit { get; set; }

        [Parameter]
        public bool isNew { get; set; }

        public MoneyCategory[] Categories { get; set; }

        protected override void OnInitialized()
        {
            if (isNew)
            {
                this.fixedItemToEdit.Date = DateTime.Now.Date;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            Categories = await savingProjectionAPI.GetMoneyCategories();
        }

        private async void OnValidSubmit()
        {
            try
            {
                if (isNew)
                {
                    await savingProjectionAPI.InsertFixedMoneyItem(fixedItemToEdit);
                }
                else
                {
                    await savingProjectionAPI.EditFixedMoneyItem(fixedItemToEdit.ID, fixedItemToEdit);
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
