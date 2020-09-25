using System;

namespace SavingsProjection.API.Models
{
    public class MoneyItem
    {
        public long ID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Amount { get; set; }
        public short RecurrencyInterval { get; set; }
        public RecurrencyType RecurrencyType { get; set; }
        public string Note { get; set; }
        public MoneyType Type { get; set; }
        public bool Accumulate { get; set; }
        public MoneyCategory Category { get; set; }
    }


    public enum MoneyType
    {
        InstallmentPayment,
        PeriodicFixedAmount,
        Ordinary
    }


    public enum RecurrencyType
    {
        Fixed,
        Day,
        Week,
        Month
    }
}
