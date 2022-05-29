using System;

namespace Savings.Model
{
    public class RecurrencyAdjustement
    {
        public long ID { get; set; }
        public DateTime RecurrencyDate { get; set; }
        public DateTime? RecurrencyNewDate { get; set; }
        public decimal? RecurrencyNewAmount { get; set; }
        public string Note { get; set; }
        public long RecurrentMoneyItemID { get; set; }
    }
}
