using Microsoft.EntityFrameworkCore;
using SavingsProjection.API.Models;

namespace SavingsProjection.API.Infrastructure
{
    public class SavingProjectionContext : DbContext
    {
        public SavingProjectionContext(DbContextOptions<SavingProjectionContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
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
