using Microsoft.AspNetCore.Components;
using SavingsProjection.Model;
using SavingsProjection.SPA.Services;
using System;
using System.Threading.Tasks;

namespace SavingsProjection.SPA.Pages
{
    public partial class Projection : ComponentBase
    {

        [Inject]
        public ISavingProjectionApi savingProjectionAPI { get; set; }

        private MaterializedMoneyItem[] materializedMoneyItems;

        public DateTime? FilterDateFrom { get; set; }

        public DateTime? FilterDateTo { get; set; }

        protected override async Task OnInitializedAsync()
        {
            FilterDateTo = DateTime.Now.AddYears(1);
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
    }
}
