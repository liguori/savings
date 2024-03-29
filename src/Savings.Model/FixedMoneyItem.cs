﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Savings.Model
{
    public class FixedMoneyItem
    {
        public long ID { get; set; }
        public DateTime Date { get; set; }
        public decimal? Amount { get; set; }
        public string Note { get; set; }
        public MoneyCategory Category { get; set; }
        public bool AccumulateForBudget { get; set; }
        public bool Cash { get; set; }
        public int TimelineWeight { get; set; }
        public long? CategoryID { get; set; }
    }
}
