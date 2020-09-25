using System;

namespace SavingsProjection.API.Models
{
    public class MaterializedMoneyItem
    {
        public long ID { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public MoneyType Type { get; set; }
        public bool Accumulate { get; set; }
        public MoneyCategory Category { get; set; }
        public string Note { get; set; }
        public decimal Projection { get; set; }
        public bool EndPeriod { get; set; }
    }
}
