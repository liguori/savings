using SavingsProjection.API.Infrastructure;
using SavingsProjection.API.Models;
using SavingsProjection.API.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SavingsProjection.API.Services
{
    public class ProjectionCalculator : IProjectionCalculator
    {
        private readonly SavingProjectionContext context;

        public ProjectionCalculator(SavingProjectionContext context)
        {
            this.context = context;
        }

        public IEnumerable<MaterializedMoneyItem> Calculate(DateTime? from, DateTime to)
        {
            var res = new List<MaterializedMoneyItem>();
            var ele = context.MoneyItems.ToList();

            return res;
        }
    }
}
