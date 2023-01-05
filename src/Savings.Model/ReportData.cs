using System;

namespace Savings.Model
{
    public class ReportCategory
    {
        public long? CategoryID { get; set; }
        public string Category { get; set; }
        public string CategoryIcon { get; set; }
        public ReportPeriodAmount[] Data { get; set; }
    }

    public class ReportPeriodAmount
    {
        public string Period { get; set; }
        public double Amount { get; set; }
    }

    public class ReportDetail
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }

    public class ReportFullDetail
    {
        public string Type { get; set; }

        public long ID { get; set; }

        public string Period { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }

        public long? CategoryID { get; set; }

        public decimal Amount { get; set; }
    }


}
