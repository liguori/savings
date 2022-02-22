using System;
using System.Collections.Generic;

namespace SavingsProjection.Model
{
    public class RecurrentMoneyItem
    {
        public long ID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal Amount { get; set; }
        public int RecurrencyInterval { get; set; }
        public RecurrencyType RecurrencyType { get; set; }
        public string Note { get; set; }
        public MoneyType Type { get; set; }
        public MoneyCategory Category { get; set; }
        public long? CategoryID { get; set; }
        public virtual IEnumerable<RecurrentMoneyItem> AssociatedItems { get; set; }
        public int TimelineWeight { get; set; }
        public virtual IEnumerable<RecurrencyAdjustement> Adjustements { get; set; }
        public long? RecurrentMoneyItemID { get; set; }
    }


    public enum MoneyType
    {
        InstallmentPayment,
        PeriodicBudget,
        Others
    }

    public enum RecurrencyType
    {
        Day,
        Week,
        Month
    }
}
