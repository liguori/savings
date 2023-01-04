using Microsoft.EntityFrameworkCore;
using Savings.Model;
using System;

namespace Savings.API.Infrastructure
{
    public class SavingsContext : DbContext
    {
        public SavingsContext(DbContextOptions<SavingsContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Configuration>().HasData(
                new Configuration { ID = 1, EndPeriodRecurrencyInterval = 1, EndPeriodRecurrencyType = RecurrencyType.Month }
            );

            modelBuilder.Entity<MoneyCategory>().HasData(
               new MoneyCategory { ID = 1, Description = "Food" },
               new MoneyCategory { ID = 2, Description = "Restaurant" },
               new MoneyCategory { ID = 3, Description = "Home" },
               new MoneyCategory { ID = 4, Description = "Car" },
               new MoneyCategory { ID = 5, Description = "Fun" },
               new MoneyCategory { ID = 6, Description = "Bills" },
               new MoneyCategory { ID = 7, Description = "Loan" },
               new MoneyCategory { ID = 8, Description = "Subscriptions" },
               new MoneyCategory { ID = 9, Description = "Beauty" },
               new MoneyCategory { ID = 10, Description = "Clothes" },
               new MoneyCategory { ID = 11, Description = "Vacation" },
               new MoneyCategory { ID = 12, Description = "Public Transport" },
               new MoneyCategory { ID = 13, Description = "Health" },
               new MoneyCategory { ID = 14, Description = "Gift" },
               new MoneyCategory { ID = 15, Description = "Culture" },
               new MoneyCategory { ID = 16, Description = "Other" }
           );

            modelBuilder.Entity<MaterializedMoneyItem>().HasData(
               new MaterializedMoneyItem { ID = 1, Date = DateTime.Now.Date.AddDays(-DateTime.Now.Date.Day), Amount = 0, Type = MoneyType.InstallmentPayment, Projection = 0, EndPeriod = true, Cash = false }
            );
        }

        public DbSet<MaterializedMoneyItem> MaterializedMoneyItems { get; set; }
        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<MoneyCategory> MoneyCategories { get; set; }
        public DbSet<RecurrentMoneyItem> RecurrentMoneyItems { get; set; }
        public DbSet<FixedMoneyItem> FixedMoneyItems { get; set; }
        public DbSet<Configuration> Configuration { get; set; }
        public DbSet<RecurrencyAdjustement> RecurrencyAdjustements { get; set; }

    }
}
