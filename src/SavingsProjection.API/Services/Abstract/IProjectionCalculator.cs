using SavingsProjection.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SavingsProjection.API.Services.Abstract
{
    public interface IProjectionCalculator
    {

        Task<IEnumerable<MaterializedMoneyItem>> CalculateAsync(DateTime? from, DateTime? to, bool breakFirstEndPeriod = false, bool onlyInstallment = false);

        Task SaveProjectionToHistory();
    }
}
