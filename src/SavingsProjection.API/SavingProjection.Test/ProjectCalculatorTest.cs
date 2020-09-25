using Microsoft.EntityFrameworkCore;
using SavingsProjection.API.Infrastructure;
using SavingsProjection.API.Models;
using SavingsProjection.API.Services;
using System;
using Xunit;

namespace SavingProjection.Test
{
    public class ProjectCalculatorTest
    {
        [Fact]
        public void TestCalculation()
        {

            var options = new DbContextOptionsBuilder<SavingProjectionContext>().UseInMemoryDatabase(databaseName: "SavingProjectionDatabase").Options;

            using (var context = new SavingProjectionContext(options))
            {
                PrepareDataContext(context);
                context.SaveChanges();
                var calculator = new ProjectionCalculator(context);
                var res = calculator.Calculate(null, DateTime.Now.AddMonths(3));
            }
        }



        void PrepareDataContext(SavingProjectionContext context)
        {
            context.MoneyItems.Add(new MoneyItem
            {
                Accumulate = false,
                Amount = 118.4M,
                Category = null,
                StartDate = new DateTime(2020, 10, 15),
                EndDate = new DateTime(2021, 08, 15),
                Type = MoneyType.InstallmentPayment,
                RecurrencyInterval = 1,
                RecurrencyType = RecurrencyType.Month
            });
        }
    }
}
