using SavingsProjection.API.Models;
using System;
using System.Collections.Generic;

namespace SavingsProjection.API.Services.Abstract
{
    interface IProjectionCalculator
    {

        IEnumerable<MaterializedMoneyItem> Calculate(DateTime? from, DateTime to);
    }
}
