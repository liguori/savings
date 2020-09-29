using Microsoft.AspNetCore.Components;
using Radzen;
using SavingsProjection.Model;
using SavingsProjection.SPA.Services;
using System;
using System.Threading.Tasks;

namespace SavingsProjection.SPA.Pages
{
    public partial class History : ComponentBase
    {

        [Inject]
        public ISavingProjectionApi savingProjectionAPI { get; set; }

        private MaterializedMoneyItem[] materializedMoneyItems;

        [Inject]
        public DialogService dialogService { get; set; }
        public DateTime? FilterDateFrom { get; set; }

        public DateTime? FilterDateTo { get; set; }

        protected override async Task OnInitializedAsync()
        {
            FilterDateFrom = DateTime.Now.Date.AddYears(-1);
            await InitializeList();
        }

        async void Change(DateTime? value, string name)
        {
            await InitializeList();
            StateHasChanged();
        }


        async Task InitializeList()
        {
            materializedMoneyItems = await savingProjectionAPI.GetMaterializedMoneyItems(FilterDateFrom, FilterDateTo);
        }

       

        async Task SaveMaterializedHistory()
        {
            var res = await dialogService.Confirm("Do you want to save the projection to the history?", "Save the history", new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });
            if (res.HasValue && res.Value)
            {
                await savingProjectionAPI.PostSavingsProjectionToHistory();
                await InitializeList();
            }
        }
    }
}
