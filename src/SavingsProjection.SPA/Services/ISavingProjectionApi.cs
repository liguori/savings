using Refit;
using SavingsProjection.Model;
using System;
using System.Threading.Tasks;

namespace SavingsProjection.SPA.Services
{
    public interface ISavingProjectionApi
    {

        [Get("/api/SavingsProjection")]
        Task<MaterializedMoneyItem[]> GetSavingsProjection(DateTime? from, DateTime to);
    }
}
