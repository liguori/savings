namespace SavingsProjection.API.Models
{
    public class Configuration
    {
        public long ID { get; set; }
        public RecurrencyType EndPeriodRecurrencyType { get; set; }
        public short EndPeriodRecurrencyInterval { get; set; }
    }
}
