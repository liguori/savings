using Microsoft.AspNetCore.Components;
using SavingsProjection.Model;
using SavingsProjection.SPA.Services;
using System;
using System.Threading.Tasks;
using Radzen;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace SavingsProjection.SPA.Pages
{
    public partial class Projection : ComponentBase
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
            FilterDateTo = DateTime.Now.Date.AddYears(1);
            await InitializeList();
        }

        async void Change(DateTime? value, string name)
        {
            await InitializeList();
            StateHasChanged();
        }


        async Task InitializeList()
        {
            materializedMoneyItems = await savingProjectionAPI.GetSavingsProjection(FilterDateFrom, FilterDateTo);
        }

        async Task AdjustRecurrency(MaterializedMoneyItem item)
        {
            var res = await dialogService.OpenAsync<RecurrencyAdjustment>($"Recurrency Adjustment",
                            new Dictionary<string, object>() { { "materializedItem", item } },
                            new DialogOptions() { Width = "600px", Height = "300px" });
            await InitializeList();
            StateHasChanged();
        }

        async Task AdjustFixedItem(MaterializedMoneyItem item)
        {
            if (!item.FixedMoneyItemID.HasValue) return;
            var itemToEdit = await savingProjectionAPI.GetixedMoneyItem(item.FixedMoneyItemID.Value);
            bool? res = await dialogService.OpenAsync<FixedItemEdit>($"Edit item",
                             new Dictionary<string, object>() { { "fixedItemToEdit", itemToEdit }, { "isNew", false } },
                             new DialogOptions() { Width = "600px" });
            if (res.HasValue && res.Value)
            {
                await InitializeList();
                StateHasChanged();
            }
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

        async Task AddNew()
        {
            bool? res = await dialogService.OpenAsync<FixedItemEdit>($"Add new",
                         new Dictionary<string, object>() { { "fixedItemToEdit", new SavingsProjection.Model.FixedMoneyItem() }, { "isNew", true } },
                         new DialogOptions() { Width = "600px" });
            if (res.HasValue && res.Value)
            {
                await InitializeList();
                StateHasChanged();
            }
        }
    }
}
