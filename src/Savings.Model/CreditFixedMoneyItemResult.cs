#nullable enable

namespace Savings.Model
{
    public class CreditFixedMoneyItemResult
    {
        public bool ToVerify { get; set; }
        public FixedMoneyItem? FixedMoneyItem { get; set; }
        public RecurrentMoneyItem? RecurrentMoneyItem { get; set; }
    }
}
