namespace DealFinder.Services
{
    public interface IPricingService
    {
        decimal ApplyMargin(PricingInformation info);
    }

    public class PricingInformation
    {
        public string MarginType { get; set; }
        public decimal MarginValue { get; set; }
        public decimal Cost { get; set; }
        public int Duration { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public decimal MinMargin { get; set; }
        public decimal MaxMargin { get; set; }
    }
}
