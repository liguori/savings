using Microsoft.AspNetCore.Components;
using SavingsProjection.Model;
using SavingsProjection.SPA.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SavingsProjection.SPA.Pages
{
    public partial class Reports : ComponentBase
    {

        [Inject]
        private ISavingProjectionApi savingProjectionAPI { get; set; }

        private RecurrentMoneyItem[] RecurrentItems { get; set; }

        private MaterializedMoneyItem[] Projections { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InitializeInstallmentResume();
        }

        async Task InitializeInstallmentResume()
        {
            var recurrentItems = await savingProjectionAPI.GetRecurrentMoneyItems(null, true);
            RecurrentItems = recurrentItems.Where(x => x.EndDate >= DateTime.Now && x.Type == MoneyType.InstallmentPayment).OrderBy(x => x.Note).ToArray();
            var projections = await savingProjectionAPI.GetSavingsProjection(null, RecurrentItems.Max(x => x.EndDate), true);
            Projections = projections.Where(x => x.RecurrentMoneyItemID.HasValue && x.Amount != 0 && x.Date >= DateTime.Now).ToArray();
        }
    }
}
