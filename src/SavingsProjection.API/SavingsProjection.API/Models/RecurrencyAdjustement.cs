using System;

namespace SavingsProjection.API.Models
{
    public class RecurrencyAdjustement
    {
        public long ID { get; set; }
        public long RecurrencyMoneyItemID { get; set; }
        public DateTime RecurrencyDate { get; set; }
        public DateTime? RecurrencyNewDate { get; set; }
        public decimal? RecurrencyNewAmount { get; set; }
        public string Note { get; set; }
    }
}
