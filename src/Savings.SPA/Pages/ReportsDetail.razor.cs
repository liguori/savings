using Microsoft.AspNetCore.Components;
using Savings.Model;
using Savings.SPA.Services;

namespace Savings.SPA.Pages
{
    public partial class ReportsDetail : ComponentBase
    {

        [Inject]
        public ISavingsApi savingsAPI { get; set; }

        [Parameter]
        public DateTime FilterDateFrom { get; set; }

        [Parameter]
        public DateTime FilterDateTo { get; set; }

        [Parameter]
        public long? category { get; set; } = null;

        [Parameter]
        public string periodPattern { get; set; } = null;

        [Parameter]
        public string period { get; set; } = null;

        public ReportDetail[]  ReportCategoryDetails { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InitializeList();
        }

        async Task InitializeList()
        {
            ReportCategoryDetails = await savingsAPI.GetCategoryResumeDetail(periodPattern, FilterDateFrom, FilterDateTo, category, period);
        }

    }
}
