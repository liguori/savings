using Microsoft.AspNetCore.Components;
using Radzen;
using Savings.Model;
using Savings.SPA.Services;

namespace Savings.SPA.Pages
{
    public partial class FixedItems : ComponentBase
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        public ISavingsApi savingsAPI { get; set; } = default!;

        [Inject]
        public DialogService dialogService { get; set; } = default!;

        public Configuration CurrentConfiguration { get; set; } = default!;

        private FixedMoneyItem[] fixedMoneyItems = default!;

        public DateTime? FilterDateFrom { get; set; }

        public DateTime? FilterDateTo { get; set; }

        public long? FilterCategory { get; set; }

        public MoneyCategory[] Categories { get; set; } = default!;

        public bool ShowToVerifyOnly { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            ShowToVerifyOnly = (System.Web.HttpUtility.ParseQueryString(NavigationManager.ToAbsoluteUri(NavigationManager.Uri).Query)["toverify"] != null);
            FilterDateFrom = DateTime.Now.Date.AddMonths(-2);
            FilterDateTo = DateTime.Now.Date.AddDays(15);
            Categories = await savingsAPI.GetMoneyCategories();
            CurrentConfiguration = (await savingsAPI.GetConfigurations()).First();
            await InitializeList();
        }

        async void Change(DateTime? value, string name)
        {
            await InitializeList();
            StateHasChanged();
        }

        async Task FilterCategoryChanged(ChangeEventArgs e)
        {
            var selectedString = e.Value?.ToString();
            FilterCategory = string.IsNullOrWhiteSpace(selectedString) ? null : long.Parse(selectedString);

            await InitializeList();
            StateHasChanged();
        }


        async Task InitializeList()
        {
            fixedMoneyItems = await savingsAPI.GetFixedMoneyItems(FilterDateFrom, FilterDateTo, false, FilterCategory, ShowToVerifyOnly);
        }

        async Task AddNew()
        {
            bool? res = await dialogService.OpenAsync<FixedItemEdit>($"Add new",
                         new Dictionary<string, object>() { { "fixedItemToEdit", new Savings.Model.FixedMoneyItem() }, { "isNew", true } },
                         new DialogOptions() { Width = "600px" });
            if (res.HasValue && res.Value)
            {
                await InitializeList();
                StateHasChanged();
            }
        }


        async Task Edit(FixedMoneyItem item)
        {
            bool? res = await dialogService.OpenAsync<FixedItemEdit>($"Edit item",
                             new Dictionary<string, object>() { { "fixedItemToEdit", item }, { "isNew", false } },
                             new DialogOptions() { Width = "600px" });
            if (res.HasValue && res.Value)
            {
                await InitializeList();
                StateHasChanged();
            }
        }

        async void ToVerifyOnly_Changed()
        {
            await InitializeList();
            StateHasChanged();
        }
    }
}
