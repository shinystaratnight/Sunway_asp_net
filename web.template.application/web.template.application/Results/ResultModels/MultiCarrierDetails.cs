namespace Web.Template.Application.Results.ResultModels
{
    public class MultiCarrierDetails
    {
        public string BookingToken { get; set; }

        public int FlightBookingId { get; set; }

        public decimal Price { get; set; }

        public string SearchBookingToken { get; set; }

        public int SupplierId { get; set; }

        public string TermsAndConditions { get; set; }

        public string TermsAndConditionsUrl { get; set; }

        public decimal TotalCommission { get; set; }
    }
}
