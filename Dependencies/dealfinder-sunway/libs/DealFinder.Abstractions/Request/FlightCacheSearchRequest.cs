namespace DealFinder.Request
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using DealFinder.Response;
    using MediatR;

    public class FlightCacheSearchRequest : IRequest<FlightCacheSearchResponse>
    {
        [XmlArrayItem("DepartureAirportID")]
        public List<int> DepartureAirports { get; set; }
        [XmlArrayItem("ArrivalAirportID")]
        public List<int> ArrivalAirports { get; set; }
        [XmlArrayItem("Month")]
        public List<string> Months { get; set; }
    }
}