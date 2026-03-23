using Refit;
using Savings.Model;

namespace Savings.SPA.Services
{
    public interface IFederationApi
    {
        [Get("/api/Savings")]
        Task<MaterializedMoneyItem[]> GetSavings(DateTime? from, DateTime? to);
    }
}
