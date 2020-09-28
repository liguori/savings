using Microsoft.AspNetCore.Components;
using Radzen;
using SavingsProjection.Model;
using SavingsProjection.SPA.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SavingsProjection.SPA.Pages
{
    public partial class RecurrentItems : ComponentBase
    {

        [Inject]
        public ISavingProjectionApi savingProjectionAPI { get; set; }

        [Inject]
        public DialogService dialogService { get; set; }

        private RecurrentMoneyItem[] recurrentMoneyItems;

        protected override async Task OnInitializedAsync()
        {
            await InitializeList();
        }

        async Task InitializeList()
        {
            recurrentMoneyItems = await savingProjectionAPI.GetRecurrentMoneyItems(true);
        }

        async Task Delete(long itemID)
        {
            var res = await dialogService.Confirm("Are you sure you want delete?", "Delete recurrent item", new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });
            if (res.HasValue && res.Value)
            {
                var deletedItem = await savingProjectionAPI.DeleteRecurrentMoneyItem(itemID);
                await InitializeList();
            }
        }

        async Task AddNew()
        {
            var res = await dialogService.OpenAsync<RecurrentItemEdit>($"Add new",
                        new Dictionary<string, object>() { { "recurrentItemToEdit", new SavingsProjection.Model.RecurrentMoneyItem() }, { "isNew", true } },
                        new DialogOptions() { Width = "600px", Height = "530px" });
            if (Convert.ToBoolean(res))
            {
                await InitializeList();
                StateHasChanged();
            }
        }



        async Task Edit(RecurrentMoneyItem item)
        {
            var res = await dialogService.OpenAsync<RecurrentItemEdit>($"Edit item",
                             new Dictionary<string, object>() { { "recurrentItemToEdit", item }, { "isNew", false } },
                             new DialogOptions() { Width = "600px", Height = "530px" });
            if (Convert.ToBoolean(res))
            {
                await InitializeList();
                StateHasChanged();
            }
        }

    }
}
