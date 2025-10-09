using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Savings.Model;
using Savings.SPA.Services;

namespace Savings.SPA.Pages
{
    public partial class Projection : ComponentBase
    {
        [Inject]
        public ISavingsApi savingsAPI { get; set; } = default!;

        [Inject]
        public IJSRuntime JSRuntime { get; set; } = default!;

        private MaterializedMoneyItem[] materializedMoneyItems = default!;

        private FixedMoneyItem[] fixedMoneyItemsToVerify = default!;

        [Inject]
        public DialogService dialogService { get; set; } = default!;

        public bool ShowPastItems { get; set; } = false;

        public bool ShowZero { get; set; } = false;

        public DateTime? FilterDateTo { get; set; }

        public Configuration CurrentConfiguration { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            FilterDateTo = DateTime.Now.Date.AddYears(1);

            await Task.WhenAll(
                InitializeList(), 
                InitializeFixedMoneyItemsToVerify()
                );

            CurrentConfiguration = (await savingsAPI.GetConfigurations()).First();
        }


        async void PastItems_Changed()
        {
            await InitializeList();
            StateHasChanged();
            await InitializeRowSelection();
        }

        async void Zero_Changed()
        {
            await InitializeList();
            StateHasChanged();
            await InitializeRowSelection();
        }

        async void Change(DateTime? value, string name)
        {
            await InitializeList();
            StateHasChanged();
            await InitializeRowSelection();
        }

        async Task InitializeFixedMoneyItemsToVerify()
        {
            fixedMoneyItemsToVerify = await savingsAPI.GetFixedMoneyItemsToVerify();
        }

        async Task InitializeList()
        {
            materializedMoneyItems = await savingsAPI.GetSavings(null, FilterDateTo);

            //Remove first zero
            if (!ShowZero)
            {
                materializedMoneyItems = materializedMoneyItems.Where(x => x.Amount != 0 || x.EndPeriod).ToArray();
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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            await InitializeRowSelection();
        }

        private async Task InitializeRowSelection()
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("projectionsRowSelection.initialize");
            }
            catch (Exception)
            {
                // Ignore errors if JavaScript is not ready yet
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
            await InitializeRowSelection();
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
                await InitializeRowSelection();
            }
        }

        async Task SaveMaterializedHistory(DateTime date)
        {
            var res = await dialogService.Confirm($"Do you want to save the projection to the history until {date:dd/MM/yyyy}?", "Save the history", new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });
            if (res.HasValue && res.Value)
            {
                await savingsAPI.PostSavingsToHistory(date);
                await InitializeList();
                await InitializeRowSelection();
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
                await InitializeRowSelection();
            }
        }
    }
}
