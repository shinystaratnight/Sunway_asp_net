namespace DealFinder.Request
{
    using System;
    using DealFinder.Response;
    using MediatR;

    public class SearchRequest : IRequest<SearchResponse>
    {
        public string DepartureAirportIDs { get; set; }
        public string GeographyLevel1IDs { get; set; }
        public string GeographyLevel2IDs { get; set; }
        public string GeographyLevel3IDs { get; set; }
        public string PropertyReferenceIDs { get; set; }
        public string ProductAttributeIDs { get; set; }
        public string FacilityIDs { get; set; }
        public string MealBasisIDs { get; set; }
        public string Ratings { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Durations { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public int MinInterestness { get; set; }
        public int Results { get; set; }
    }
}
