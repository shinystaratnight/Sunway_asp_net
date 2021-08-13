namespace DealFinder.Records
{
    using System;
    using DealFinder.Services;

    public class PropertyRecord
    {
        public int ArrivalAirportID { get; set; }
        public int DepartureAirportID { get; set; }
        public DateTime DepartureDate { get; set; }
        public int Duration { get; set; }
        public int MealBasisID { get; set; }
        public decimal FlightCost { get; set; }
        public int Interestingness { get; set; }
        public string PackageReference { get; set; }
        public decimal PropertyCost { get; set; }
        public int PropertyReferenceID { get; set; }
        public string PropertyMarginType { get; set; }
        public decimal PropertyMarginValue { get; set; }
        public decimal PropertyMinMargin { get; set; }
        public decimal PropertyMaxMargin { get; set; }
        public string FlightMarginType { get; set; }
        public decimal FlightMarginValue { get; set; }
        public decimal FlightMinMargin { get; set; }
        public decimal FlightMaxMargin { get; set; }

        public PricingInformation GetPropertyPriceInfo(int adults, int children)
        {
            return new PricingInformation
            {
                MarginType = PropertyMarginType,
                MarginValue = PropertyMarginValue,
                MinMargin = PropertyMinMargin,
                MaxMargin = PropertyMaxMargin,
                Duration = Duration,
                Adults = adults,
                Children = children,
                Cost = PropertyCost
            };
        }

        public PricingInformation GetFlightPriceInfo(int adults, int children)
        {
            return new PricingInformation
            {
                MarginType = FlightMarginType,
                MarginValue = FlightMarginValue,
                MinMargin = FlightMinMargin,
                MaxMargin = FlightMaxMargin,
                Duration = Duration,
                Adults = adults,
                Children = children,
                Cost = FlightCost
            };
        }
    }


}
