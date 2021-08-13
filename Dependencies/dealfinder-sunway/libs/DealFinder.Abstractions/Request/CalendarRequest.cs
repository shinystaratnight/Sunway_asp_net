namespace DealFinder.Request
{
    using System;
    using DealFinder.Response;
    using MediatR;

    public class CalendarRequest : IRequest<CalendarResponse>
    {
        public int PropertyReferenceID { get; set; }
        public int DepartureAirportID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Duration { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public int MealBasisID { get; set; }
    }
}
