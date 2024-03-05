using Microsoft.AspNetCore.Components;
using Savings.Model;
using Savings.SPA.Services;
using System;
using System.Threading.Tasks;
using Radzen;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Savings.SPA.Pages
{
    public partial class Projection : ComponentBase
    {
        [Inject]
        public ISavingsApi savingsAPI { get; set; }

        private MaterializedMoneyItem[] materializedMoneyItems;

        [Inject]
        public DialogService dialogService { get; set; }

        public bool ShowPastItems { get; set; } = false;

        public bool ShowZero { get; set; } = false;

        public DateTime? FilterDateTo { get; set; }

        public Configuration CurrentConfiguration { get; set; }

        protected override async Task OnInitializedAsync()
        {
            FilterDateTo = DateTime.Now.Date.AddYears(1);
            await InitializeList();
            CurrentConfiguration = (await savingsAPI.GetConfigurations()).FirstOrDefault();
        }


        async void PastItems_Changed()
        {
            await InitializeList();
            StateHasChanged();
        }

        async void Zero_Changed()
        {
            await InitializeList();
            StateHasChanged();
        }

        async void Change(DateTime? value, string name)
        {
            await InitializeList();
            StateHasChanged();
        }


        async Task InitializeList()
        {
            materializedMoneyItems = await savingsAPI.GetSavings(null, FilterDateTo);

            //Remove first zero
            if (!ShowZero)
            {
                materializedMoneyItems = materializedMoneyItems.Where(x => x.Amount != 0).ToArray();
            }

            //The decide to remove past items (when zero is already excluded)
            if (!ShowPastItems)
            {
                var lastBeforeToday = materializedMoneyItems.LastOrDefault(x => x.Date <= DateTime.Now.Date);
                if (lastBeforeToday != null)
                {
                    materializedMoneyItems = materializedMoneyItems[Array.IndexOf(materializedMoneyItems, lastBeforeToday)..];
                }
            }
           
        }

        async Task AdjustRecurrency(MaterializedMoneyItem item)
        {
            if (item.EndPeriod) return;
            var res = await dialogService.OpenAsync<RecurrencyAdjustment>($"Recurrency Adjustment",
                            new Dictionary<string, object>() { { "materializedItem", item } },
                            new DialogOptions() { Width = "600px", Height = "300px" });
            await InitializeList();
            StateHasChanged();
        }

        async Task AdjustFixedItem(MaterializedMoneyItem item)
        {
            if (item.EndPeriod) return;
            if (!item.FixedMoneyItemID.HasValue) return;
            var itemToEdit = await savingsAPI.GetixedMoneyItem(item.FixedMoneyItemID.Value);
            bool? res = await dialogService.OpenAsync<FixedItemEdit>($"Edit item",
                             new Dictionary<string, object>() { { "fixedItemToEdit", itemToEdit }, { "isNew", false } },
                             new DialogOptions() { Width = "600px" });
            if (res.HasValue && res.Value)
            {
                await InitializeList();
                StateHasChanged();
            }
        }

        async Task SaveMaterializedHistory(DateTime date)
        {
            var res = await dialogService.Confirm($"Do you want to save the projection to the history until {date:dd/MM/yyyy}?", "Save the history", new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });
            if (res.HasValue && res.Value)
            {
                await savingsAPI.PostSavingsToHistory(date);
                await InitializeList();
            }
        }

        async Task AddNew()
        {
            bool? res = await dialogService.OpenAsync<FixedItemEdit>($"Add new",
                         new Dictionary<string, object>() { { "fixedItemToEdit", new Savings.Model.FixedMoneyItem() }, { "isNew", true } },
                         new DialogOptions() { Width = "700px" });
            if (res.HasValue && res.Value)
            {
                await InitializeList();
                StateHasChanged();
            }
        }
    }
}
