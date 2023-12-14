using Microsoft.AspNetCore.Components;
using Radzen;
using Savings.Model;
using Savings.SPA.Services;

namespace Savings.SPA.Pages
{
    public partial class RecurrentItems : ComponentBase
    {

        [Inject]
        public ISavingsApi savingsAPI { get; set; }

        [Inject]
        public DialogService dialogService { get; set; }

        [Inject]
        public NotificationService notificationService { get; set; }


        private RecurrentMoneyItem[] recurrentMoneyItems;

        [Parameter]
        public long? parentItemID { get; set; } = null;

        [Parameter]
        public RecurrentMoneyItem parentItem { get; set; } = null;

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
            recurrentMoneyItems = await savingsAPI.GetRecurrentMoneyItems(parentItemID, ShowOnlyActive, FilterOnlyActiveDateFrom, FilterOnlyActiveDateTo);
        }


        async Task AddNew()
        {
            bool? res = await dialogService.OpenAsync<RecurrentItemEdit>($"Add new",
                        new Dictionary<string, object>() { { "recurrentItemToEdit", new Savings.Model.RecurrentMoneyItem() }, { "isNew", true }, { "parentItemID", parentItemID }, { "parentItem", parentItem } },
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
            var res = await dialogService.OpenAsync<RecurrentItems>($"Associated Items - {item.Note}",
                            new Dictionary<string, object>() { { "parentItemID", item.ID }, { "parentItem", item } },
                             new DialogOptions() { Width = "800px", Height = "600px" });
            await InitializeList();
            StateHasChanged();
        }

    }
}
