namespace Web.Template.Domain.Entities.Flight
{
    using System;
    using System.Xml.Serialization;

    public class FlightCacheRouteDate
    {
        public FlightCacheRouteDate() { }

        public FlightCacheRouteDate(
            DateTime departureDate,
            int[] durations, 
            bool ownStock)
        {
            DepartureDate = departureDate;
            Durations = durations;
            OwnStock = ownStock;
        }

        public DateTime DepartureDate { get; set; }
        [XmlArrayItem("Duration")]
        public int[] Durations { get; set; }
        public bool OwnStock { get; set; }
    }
}
