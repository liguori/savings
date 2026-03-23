using Microsoft.AspNetCore.Components;
using Savings.Model;
using Savings.SPA.Services;

namespace Savings.SPA.Pages
{
    public partial class FederatedProjection : ComponentBase
    {
        [Inject]
        public IFederationService FederationService { get; set; } = default!;

        private bool loading = true;

        private List<FederatedItemViewModel> mergedItems = new();
        private List<FederatedProjectionResult> errors = new();

        public bool ShowPastItems { get; set; } = false;
        public bool ShowZero { get; set; } = false;
        public DateTime? FilterDateTo { get; set; }
        public bool ShowSummaryCards { get; set; } = false;

        // Dashboard summary properties
        public decimal CurrentBalance { get; set; }
        public decimal PeriodIncome { get; set; }
        public decimal PeriodExpenses { get; set; }
        public decimal PeriodGain { get; set; }

        // Balance trend data for mini-chart
        public List<BalanceTrendDataItem> BalanceTrendData { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            FilterDateTo = DateTime.Now.Date.AddYears(1);
            await LoadData();
        }

        async Task LoadData()
        {
            loading = true;
            StateHasChanged();

            var results = await FederationService.GetFederatedProjectionsAsync(null, FilterDateTo);

            errors = results.Where(r => r.Error != null).ToList();

            var allItems = new List<FederatedItemViewModel>();
            foreach (var result in results.Where(r => r.Error == null))
            {
                foreach (var item in result.Items)
                {
                    allItems.Add(new FederatedItemViewModel
                    {
                        SourceName = result.EndpointName,
                        Item = item
                    });
                }
            }

            // Sort by date, then by source name
            allItems = allItems.OrderBy(x => x.Item.Date).ThenBy(x => x.SourceName).ToList();

            // Apply filters
            if (!ShowZero)
            {
                allItems = allItems.Where(x => x.Item.Amount != 0 || x.Item.EndPeriod).ToList();
            }

            if (!ShowPastItems)
            {
                var lastBeforeToday = allItems.LastOrDefault(x => x.Item.Date <= DateTime.Now.Date);
                if (lastBeforeToday != null)
                {
                    var index = allItems.IndexOf(lastBeforeToday);
                    allItems = allItems.Skip(index).ToList();
                }
            }

            mergedItems = allItems;

            ComputeDashboardSummary();

            loading = false;
            StateHasChanged();
        }

        void ComputeDashboardSummary()
        {
            if (mergedItems == null || mergedItems.Count == 0)
            {
                CurrentBalance = 0;
                PeriodIncome = 0;
                PeriodExpenses = 0;
                PeriodGain = 0;
                BalanceTrendData = new();
                return;
            }

            // Current balance: sum of projections from all endpoints at or before today
            var todayItems = mergedItems.Where(x => x.Item.Date <= DateTime.Now.Date).ToList();
            // Get the last item per source that is on or before today
            var lastPerSource = todayItems
                .GroupBy(x => x.SourceName)
                .Select(g => g.Last())
                .ToList();
            CurrentBalance = lastPerSource.Sum(x => x.Item.Projection);

            // Income/Expenses for non end-period items
            var nonEndPeriodItems = mergedItems.Where(x => !x.Item.EndPeriod).ToList();
            PeriodIncome = nonEndPeriodItems.Where(x => x.Item.Amount > 0).Sum(x => x.Item.Amount);
            PeriodExpenses = nonEndPeriodItems.Where(x => x.Item.Amount < 0).Sum(x => x.Item.Amount);
            PeriodGain = PeriodIncome + PeriodExpenses;

            // Build aggregated balance trend from end-period items
            var endPeriodItems = mergedItems.Where(x => x.Item.EndPeriod).ToList();
            var groupedByDate = endPeriodItems
                .GroupBy(x => x.Item.Date.ToString("MMM yy"))
                .Select(g => new BalanceTrendDataItem
                {
                    Label = g.Key,
                    Value = (double)g.Sum(x => x.Item.Projection)
                }).ToList();
            BalanceTrendData = groupedByDate;
        }

        void ToggleSummaryCards()
        {
            ShowSummaryCards = !ShowSummaryCards;
        }

        string GetAmountRowClass(MaterializedMoneyItem item)
        {
            if (item.EndPeriod) return "";
            if (item.Amount > 0) return "income-row";
            if (item.Amount < 0) return "expense-row";
            return "";
        }

        async void OnFilterChanged()
        {
            await LoadData();
        }

        async void OnDateChanged(DateTime? value)
        {
            await LoadData();
        }
    }

    public class FederatedItemViewModel
    {
        public string SourceName { get; set; } = string.Empty;
        public MaterializedMoneyItem Item { get; set; } = default!;
    }
}
