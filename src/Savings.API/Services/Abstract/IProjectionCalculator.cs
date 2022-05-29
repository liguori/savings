using Savings.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Savings.API.Services.Abstract
{
    public interface IProjectionCalculator
    {

        Task<IEnumerable<MaterializedMoneyItem>> CalculateAsync(DateTime? from, DateTime? to, bool breakFirstEndPeriod = false, bool onlyInstallment = false, bool includeLastEndPeriod = true);

        Task SaveProjectionToHistory();
    }
}
