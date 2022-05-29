namespace Savings.Model
{
    public class Configuration
    {
        public long ID { get; set; }
        public RecurrencyType EndPeriodRecurrencyType { get; set; }
        public short EndPeriodRecurrencyInterval { get; set; }
        public long CashWithdrawalCategoryID { get; set; }
    }
}
