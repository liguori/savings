using Microsoft.AspNetCore.Components;
using Radzen;
using SavingsProjection.Model;
using SavingsProjection.SPA.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SavingsProjection.SPA.Pages
{
    public partial class FixedItems : ComponentBase
    {

        [Inject]
        public ISavingProjectionApi savingProjectionAPI { get; set; }

        [Inject]
        public DialogService dialogService { get; set; }

        private FixedMoneyItem[] fixedMoneyItems;

        public DateTime? FilterDateFrom { get; set; }

        public DateTime? FilterDateTo { get; set; }

        protected override async Task OnInitializedAsync()
        {
            FilterDateFrom = DateTime.Now.Date.AddMonths(-2);
            FilterDateTo = DateTime.Now.Date.AddDays(15);
            await InitializeList();
        }

        async void Change(DateTime? value, string name)
        {
            await InitializeList();
            StateHasChanged();
        }


        async Task InitializeList()
        {
            fixedMoneyItems = await savingProjectionAPI.GetFixedMoneyItems(FilterDateFrom, FilterDateTo);
        }

        async Task Delete(long itemID)
        {
            var res = await dialogService.Confirm("Are you sure you want delete?", "Delete fixed item", new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });
            if (res.HasValue && res.Value)
            {
                var deletedItem = await savingProjectionAPI.DeleteFixedMoneyItem(itemID);
                await InitializeList();
            }
        }

        async Task AddNew()
        {
            bool res = await dialogService.OpenAsync<FixedItemEdit>($"Add new",
                         new Dictionary<string, object>() { { "fixedItemToEdit", new SavingsProjection.Model.FixedMoneyItem() }, { "isNew", true } },
                         new DialogOptions() { Width = "600px", Height = "530px" });
            if (res)
            {
                await InitializeList();
                StateHasChanged();
            }
        }


        async Task Edit(FixedMoneyItem item)
        {
            bool res = await dialogService.OpenAsync<FixedItemEdit>($"Edit item",
                             new Dictionary<string, object>() { { "fixedItemToEdit", item }, { "isNew", false } },
                             new DialogOptions() { Width = "600px", Height = "530px" });
            if (res)
            {
                await InitializeList();
                StateHasChanged();
            }
        }
    }
}
