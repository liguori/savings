using Refit;
using SavingsProjection.Model;
using System;
using System.Threading.Tasks;

namespace SavingsProjection.SPA.Services
{
    public interface ISavingProjectionApi
    {
        [Get("/api/SavingsProjection")]
        Task<MaterializedMoneyItem[]> GetSavingsProjection(DateTime? from, DateTime? to, bool onlyInstallment = false);

        [Get("/api/FixedMoneyItems")]
        Task<FixedMoneyItem[]> GetFixedMoneyItems(DateTime? from, DateTime? to, bool excludeWithdrawal);

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

        [Post("/api/SavingsProjection/ToHistory")]
        Task PostSavingsProjectionToHistory();

        [Get("/api/MaterializedMoneyItems")]
        Task<MaterializedMoneyItem[]> GetMaterializedMoneyItems(DateTime? from, DateTime? to, bool onlyRecurrent);

        [Delete("/api/MaterializedMoneyItems/ToHistory/{id}")]
        Task DeleteMaterializedMoneyItemToHistory(long id);

        [Get("/api/MoneyCategories")]
        Task<MoneyCategory[]> GetMoneyCategories();

        [Get("/api/Configurations")]
        Task<Configuration[]> GetConfigurations();

        [Get("/api/Report/GetCategoryResume")]
        Task<ReportCategoryData[]> GetCategoryResume();
    }
}
