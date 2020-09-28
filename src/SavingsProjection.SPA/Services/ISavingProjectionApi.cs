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

    }
}
