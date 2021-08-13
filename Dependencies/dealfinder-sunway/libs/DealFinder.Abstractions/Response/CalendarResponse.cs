namespace DealFinder.Response
{
    using System.Xml.Serialization;

    public class CalendarResponse
    {
        [XmlArrayItem("Date")]
        public PackagePriceByDate[] Dates { get; set; }
    }
}
