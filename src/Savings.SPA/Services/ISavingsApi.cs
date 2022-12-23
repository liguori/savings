using Refit;
using Savings.Model;
using System;
using System.Threading.Tasks;

namespace Savings.SPA.Services
{
    public interface ISavingsApi
    {
        [Get("/api/Savings")]
        Task<MaterializedMoneyItem[]> GetSavings(DateTime? from, DateTime? to, bool onlyInstallment = false);

        [Get("/api/FixedMoneyItems")]
        Task<FixedMoneyItem[]> GetFixedMoneyItems(DateTime? from, DateTime? to, bool excludeWithdrawal, long? filterCategory);

        [Delete("/api/FixedMoneyItems/{id}")]
        Task<FixedMoneyItem> DeleteFixedMoneyItem(long id);

        [Get("/api/FixedMoneyItems/{id}")]
        Task<FixedMoneyItem> GetixedMoneyItem(long id);

        [Post("/api/FixedMoneyItems")]
        Task<FixedMoneyItem> InsertFixedMoneyItem(FixedMoneyItem fixedMoneyItem);

        [Put("/api/FixedMoneyItems/{id}")]
        Task EditFixedMoneyItem(long id, FixedMoneyItem fixedMoneyItem);

        [Get("/api/RecurrentMoneyItems")]
        Task<RecurrentMoneyItem[]> GetRecurrentMoneyItems(long? parentItemID, bool onlyActive, DateTime? endDateFrom, DateTime? endDateTo);

        [Delete("/api/RecurrentMoneyItems/{id}")]
        Task<RecurrentMoneyItem> DeleteRecurrentMoneyItem(long id);

        [Post("/api/RecurrentMoneyItems")]
        Task<RecurrentMoneyItem> InsertRecurrentMoneyItem(RecurrentMoneyItem fixedMoneyItem);

        [Put("/api/RecurrentMoneyItems/{id}")]
        Task EditRecurrentMoneyItem(long id, RecurrentMoneyItem fixedMoneyItem);

        [Get("/api/RecurrencyAdjustements/ByIDRecurrencyAndDate")]
        Task<RecurrencyAdjustement> GetRecurrencyAdjustementByIDRecurrencyAndDate(long idRecurrency, DateTime date);

        [Post("/api/RecurrencyAdjustements")]
        Task<RecurrencyAdjustement> InsertRecurrencyAdjustment(RecurrencyAdjustement recurrencyAdjustement);

        [Put("/api/RecurrencyAdjustements/{id}")]
        Task EditRecurrencyAdjustment(long id, RecurrencyAdjustement recurrencyAdjustement);

        [Delete("/api/RecurrencyAdjustements/{id}")]
        Task<RecurrencyAdjustement> DeleteRecurrencyAdjustment(long id);

        [Get("/api/Savings/Backup")]
        Task<Stream> GetBackup();

        [Post("/api/Savings/ToHistory")]
        Task PostSavingsToHistory();

        [Patch("/api/MaterializedMoneyItems/LastMaterializedMoneyItemPeriod")]
        Task EditLastMaterializedMoneyItemPeriod(DateTime date);

        [Get("/api/MaterializedMoneyItems/LastMaterializedMoneyItemPeriod")]
        Task<MaterializedMoneyItem> GetLastMaterializedMoneyItemPeriod();

        [Get("/api/MaterializedMoneyItems")]
        Task<MaterializedMoneyItem[]> GetMaterializedMoneyItems(DateTime? from, DateTime? to, bool onlyRecurrent);

        [Delete("/api/MaterializedMoneyItems/ToHistory/{id}")]
        Task DeleteMaterializedMoneyItemToHistory(long id);

        [Get("/api/MoneyCategories")]
        Task<MoneyCategory[]> GetMoneyCategories();

        [Get("/api/Configurations")]
        Task<Configuration[]> GetConfigurations();

        [Put("/api/Configurations/{id}")]
        Task PutConfiguration(long id, Configuration configuration);

        [Get("/api/Report/GetCategoryResume")]
        Task<ReportCategoryData[]> GetCategoryResume(string periodPattern, int lastMonths);
    }
}
