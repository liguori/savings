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

        protected override async Task OnInitializedAsync()
        {
            if (isNew)
            {
                this.fixedItemToEdit.Date = DateTime.Now;
            }
        }

        private void OnValidSubmit()
        {
            if (isNew)
            {
                savingProjectionAPI.InsertFixedMoneyItem(fixedItemToEdit);
            }
            else
            {
                savingProjectionAPI.EditFixedMoneyItem(fixedItemToEdit.ID, fixedItemToEdit);
            }

            this.dialogService.Close(true);
        }

    }
}
