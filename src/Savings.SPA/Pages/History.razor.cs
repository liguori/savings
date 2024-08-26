using Microsoft.AspNetCore.Components;
using Radzen;
using Savings.Model;
using Savings.SPA.Services;

namespace Savings.SPA.Pages
{
    public partial class History : ComponentBase
    {

        [Inject]
        public ISavingsApi savingsAPI { get; set; } = default!;

        private MaterializedMoneyItem[]? materializedMoneyItems;

        [Inject]
        public DialogService dialogService { get; set; } = default!;
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
            var items = await savingsAPI.GetMaterializedMoneyItems(FilterDateFrom, FilterDateTo, false);
            materializedMoneyItems = items;
        }

        async Task DeleteMaterializedHistory(MaterializedMoneyItem item)
        {
            var res = await dialogService.Confirm($"Do you want to delete the projection to the history until {item.Date:dd/MM/yyyy}?", "Delete the history", new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });
            if (res.HasValue && res.Value)
            {
                await savingsAPI.DeleteMaterializedMoneyItemToHistory(item.ID);
                await InitializeList();
            }
        }
    }
}
