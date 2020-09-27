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
        public ISavingProjectionApi savingProjectionAPI{ get; set; }

        private MaterializedMoneyItem[] materializedMoneyItems;

        public DateTime FilterDateFrom { get; set; }

        protected override async Task OnInitializedAsync()
        {
            materializedMoneyItems = await savingProjectionAPI.GetSavingsProjection(null, new DateTime(2020, 12, 29));
        }

       
    }
}
