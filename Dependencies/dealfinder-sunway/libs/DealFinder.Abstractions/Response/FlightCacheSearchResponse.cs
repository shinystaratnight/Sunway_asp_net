namespace DealFinder.Response
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class FlightCacheSearchResponse
    {
        public FlightCacheSearchResponse() { }
        public FlightCacheSearchResponse(DepartureDateAndDurations[] dates)
        {
            Dates = dates;
        }

        [XmlArrayItem("Date")]
        public DepartureDateAndDurations[] Dates { get; set; }
    }
}
