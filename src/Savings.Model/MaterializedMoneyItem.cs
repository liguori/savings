using System;
using System.Collections.Generic;

namespace Savings.Model
{
    public class MaterializedMoneyItem
    {
        public long ID { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public MoneyType Type { get; set; }
        public MoneyCategory Category { get; set; }
        public long? CategoryID { get; set; }
        public string Note { get; set; }
        public decimal Projection { get; set; }
        public bool EndPeriod { get; set; }
        public int TimelineWeight { get; set; }
        public bool IsRecurrent { get; set; }
        public long? RecurrentMoneyItemID { get; set; }
        public long? FixedMoneyItemID { get; set; }
        public bool Cash { get; set; }
        public decimal EndPeriodCashCarry { get; set; }
        public IEnumerable<MaterializedMoneySubitems> Subitems{ get; set; }
    }

    public class MaterializedMoneySubitems
    {
        public long ID { get; set; }
        public decimal Amount { get; set; }
        public long? CategoryID { get; set; }
        public MoneyCategory Category { get; set; }
        public string Note { get; set; }
    }
}
