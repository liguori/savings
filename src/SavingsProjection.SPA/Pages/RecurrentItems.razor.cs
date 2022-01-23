using Microsoft.AspNetCore.Components;
using Radzen;
using SavingsProjection.Model;
using SavingsProjection.SPA.Services;

namespace SavingsProjection.SPA.Pages
{
    public partial class RecurrentItems : ComponentBase
    {

        [Inject]
        public ISavingProjectionApi savingProjectionAPI { get; set; }

        [Inject]
        public DialogService dialogService { get; set; }

        [Inject]
        public NotificationService notificationService { get; set; }


        private RecurrentMoneyItem[] recurrentMoneyItems;

        [Parameter]
        public long? parentItemID { get; set; } = null;

        public bool ShowOnlyActive { get; set; } = true;
        public DateTime? FilterOnlyActiveDateFrom { get; set; }
        public DateTime? FilterOnlyActiveDateTo { get; set; }

        async Task ShowOnlyActiveOnChange()
        {
            await InitializeList();
            StateHasChanged();
        }

        async void FilterDateOnlyActiveChange(DateTime? value, string name)
        {
            await InitializeList();
            StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {
            await InitializeList();
        }

        async Task InitializeList()
        {
            recurrentMoneyItems = await savingProjectionAPI.GetRecurrentMoneyItems(parentItemID, ShowOnlyActive, FilterOnlyActiveDateFrom, FilterOnlyActiveDateTo);
        }

        async Task Delete(long itemID)
        {
            try
            {
                var res = await dialogService.Confirm("Are you sure you want delete?", "Delete recurrent item", new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });
                if (res.HasValue && res.Value)
                {
                    var deletedItem = await savingProjectionAPI.DeleteRecurrentMoneyItem(itemID);
                    await InitializeList();
                }
            }
            catch (Exception ex)
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", ex.Message);
            }
        }

        async Task AddNew()
        {
            bool? res = await dialogService.OpenAsync<RecurrentItemEdit>($"Add new",
                        new Dictionary<string, object>() { { "recurrentItemToEdit", new SavingsProjection.Model.RecurrentMoneyItem() }, { "isNew", true }, { "parentItemID", parentItemID } },
                        new DialogOptions() { Width = "600px" });
            if (res.HasValue && res.Value)
            {
                await InitializeList();
                StateHasChanged();
            }
        }

        async Task Edit(RecurrentMoneyItem item)
        {
            bool? res = await dialogService.OpenAsync<RecurrentItemEdit>($"Edit item",
                             new Dictionary<string, object>() { { "recurrentItemToEdit", item }, { "isNew", false }, { "parentItemID", parentItemID } },
                             new DialogOptions() { Width = "600px" });
            if (res.HasValue && res.Value)
            {
                await InitializeList();
                StateHasChanged();
            }
        }


        async Task ViewChild(RecurrentMoneyItem item)
        {
            var res = await dialogService.OpenAsync<RecurrentItems>($"Associated Items",
                            new Dictionary<string, object>() { { "parentItemID", item.ID } },
                             new DialogOptions() { Width = "800px", Height = "600px" });
            await InitializeList();
            StateHasChanged();
        }

    }
}
