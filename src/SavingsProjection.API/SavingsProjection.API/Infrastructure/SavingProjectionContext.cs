using Microsoft.EntityFrameworkCore;
using SavingsProjection.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public DbSet<MoneyCategory> MoneyCategories { get; set; }
        public DbSet<MoneyItem> MoneyItems { get; set; }

    }
}
