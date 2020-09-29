using Refit;
using SavingsProjection.Model;
using System;
using System.Threading.Tasks;

namespace SavingsProjection.SPA.Services
{
    public interface ISavingProjectionApi
    {
        [Get("/api/SavingsProjection")]
        Task<MaterializedMoneyItem[]> GetSavingsProjection(DateTime? from, DateTime? to);

        [Get("/api/FixedMoneyItems")]
        Task<FixedMoneyItem[]> GetFixedMoneyItems(DateTime? from, DateTime? to);

        [Delete("/api/FixedMoneyItems/{id}")]
        Task<FixedMoneyItem> DeleteFixedMoneyItem(long id);

        [Post("/api/FixedMoneyItems")]
        Task<FixedMoneyItem> InsertFixedMoneyItem(FixedMoneyItem fixedMoneyItem);

        [Put("/api/FixedMoneyItems/{id}")]
        Task<FixedMoneyItem> EditFixedMoneyItem(long id, FixedMoneyItem fixedMoneyItem);

        [Get("/api/RecurrentMoneyItems")]
        Task<RecurrentMoneyItem[]> GetRecurrentMoneyItems(long? parentItemID);

        [Delete("/api/RecurrentMoneyItems/{id}")]
        Task<RecurrentMoneyItem> DeleteRecurrentMoneyItem(long id);

        [Post("/api/RecurrentMoneyItems")]
        Task<RecurrentMoneyItem> InsertRecurrentMoneyItem(RecurrentMoneyItem fixedMoneyItem);

        [Put("/api/RecurrentMoneyItems/{id}")]
        Task<RecurrentMoneyItem> EditRecurrentMoneyItem(long id, RecurrentMoneyItem fixedMoneyItem);

        [Get("/api/RecurrencyAdjustements/ByIDRecurrency/{idRecurrency}")]
        Task<RecurrencyAdjustement> GetRecurrencyAdjustementByIDRecurrency(long idRecurrency);

        [Post("/api/RecurrencyAdjustements")]
        Task<RecurrencyAdjustement> InsertRecurrencyAdjustment(RecurrencyAdjustement recurrencyAdjustement);

        [Put("/api/RecurrencyAdjustements/{id}")]
        Task<RecurrencyAdjustement> EditRecurrencyAdjustment(long id, RecurrencyAdjustement recurrencyAdjustement);

        [Delete("/api/RecurrencyAdjustements/{id}")]
        Task<RecurrencyAdjustement> DeleteRecurrencyAdjustment(long id);

        [Post("/api/SavingsProjection/ToHistory")]
        Task PostSavingsProjectionToHistory();

        [Get("/api/MaterializedMoneyItems")]
        Task<MaterializedMoneyItem[]> GetMaterializedMoneyItems(DateTime? from, DateTime? to);

        [Delete("/api/MaterializedMoneyItems/ToHistory/{id}")]
        Task DeleteMaterializedMoneyItemToHistory(long id);
    }
}
