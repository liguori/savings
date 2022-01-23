using Microsoft.AspNetCore.Components;
using SavingsProjection.Model;
using SavingsProjection.SPA.Services;

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
            await InitializeCategoryResume();
        }

        async Task InitializeInstallmentResume()
        {
            var recurrentItems = await savingProjectionAPI.GetRecurrentMoneyItems(null, true, null, null);
            RecurrentItems = recurrentItems.Where(x => x.EndDate >= DateTime.Now && x.Type == MoneyType.InstallmentPayment).OrderBy(x => x.Note).ToArray();
            DateTime endDate = DateTime.Now.AddMonths(1);
            if (RecurrentItems.Any())
            {
                endDate = RecurrentItems.Max(x => x.EndDate);
            }
            var projections = await savingProjectionAPI.GetSavingsProjection(null, endDate, true);
            Projections = projections.Where(x => x.RecurrentMoneyItemID.HasValue && x.Amount != 0 && x.Date >= DateTime.Now).ToArray();
        }


        async Task InitializeCategoryResume()
        {
            statistics = await savingProjectionAPI.GetCategoryResume();
        }



        string FormatAmount(object value)
        {
            return ((double)value).ToString("N2");
        }

        ReportCategoryData[] statistics;
    }

}
