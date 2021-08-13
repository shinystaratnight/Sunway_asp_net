namespace DealFinder.Response
{
    using System;

    public class Property
    {
        public Property() { }

        public Property(
            int arrivalAirportId,
            int departureAirportId,
            DateTime departureDate, 
            int duration, 
            int interestingness, 
            int mealBasisId,
            string packageReference,
            decimal price,
            int propertyReferenceId)
        {
            PackageReference = packageReference;
            PropertyReferenceID = propertyReferenceId;
            DepartureAirportID = departureAirportId;
            ArrivalAirportID = arrivalAirportId;
            DepartureDate = departureDate;
            Duration = duration;
            Price = price;
            Interestingness = interestingness;
            MealBasisID = mealBasisId;
        }

        public int ArrivalAirportID { get; set; }
        public int DepartureAirportID { get; set; }
        public DateTime DepartureDate { get; set; }
        public int Duration { get; set; }
        public int Interestingness { get; set; }
        public int MealBasisID { get; set; }
        public string PackageReference { get; set; }
        public decimal Price { get; set; }
        public int PropertyReferenceID { get; set; }
    }
}
