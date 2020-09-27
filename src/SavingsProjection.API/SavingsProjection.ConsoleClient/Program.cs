using Microsoft.EntityFrameworkCore;
using SavingsProjection.API.Infrastructure;
using SavingsProjection.API.Models;
using SavingsProjection.API.Services;
using System;
using System.Collections.Generic;

namespace SavingsProjection.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {

            var options = new DbContextOptionsBuilder<SavingProjectionContext>().UseInMemoryDatabase(databaseName: "SavingProjectionDatabase").Options;

            using (var context = new SavingProjectionContext(options))
            {
                PrepareDataContext(context);
                context.SaveChanges();
                var calculator = new ProjectionCalculator(context);
                var res = calculator.Calculate(null, DateTime.Now.AddMonths(13));


                foreach (var item in res)
                {
                    Console.WriteLine(item.Date.ToShortDateString() + "|" + item.Amount.ToString().PadLeft(20) + "|" + item.Projection.ToString().PadLeft(20) + "|" + item.Note);
                }
            }
            Console.ReadLine();
        }

        static void PrepareDataContext(SavingProjectionContext context)
        {
            //Starting point
            context.MaterializedMoneyItems.Add(new MaterializedMoneyItem
            {
                Date = new DateTime(2020, 09, 27),
                Amount = 0M,
                Projection = 3035.04M,
                EndPeriod = true
            });


            context.Configuration.Add(new Configuration
            {
                EndPeriodRecurrencyInterval = 1,
                EndPeriodRecurrencyType = RecurrencyType.Month
            });


            //Fixed expense
            context.FixedMoneyItems.Add(new FixedMoneyItem
            {
                Date = new DateTime(2020, 09, 27),
                Amount = -14.07M,
                Note = "Spesa PAM",
                AccumulateForBudget = true
            });

        }
    }
}
