namespace Web.Template.Data.Lookup.Repositories.ConnectLookups.Booking
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Linq;

    using Intuitive;

    using Web.Template.Data.Connect;
    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Booking;

    /// <summary>
    /// Class ConnectTradeRepository.
    /// </summary>
    public class ConnectTradeRepository : ConnectLookupBase<Trade>, ITradeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectTradeRepository" /> class.
        /// </summary>
        /// <param name="asyncLookup">A class using intuitive async cache to get lookup xml from connect.</param>
        public ConnectTradeRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>A list of trade</returns>
        protected override List<Trade> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("Trade");
            XDocument xDoc = xml.ToXDocument();
            var trades = new List<Trade>();

            XElement element = xDoc.Element("Lookups")?.Element("Trades");
            if (element != null)
            {
                foreach (XElement xElement in element.Elements("Trade"))
                {
                    var trade = new Trade()
                                    {
                                        ABTAATOLNumber = (string)xElement.Element("ABTAATOLNumber"), 
                                        Address1 = (string)xElement.Element("Address1"), 
                                        Address2 = (string)xElement.Element("Address2"), 
                                        BookingCountryId = (int)xElement.Element("BookingCountryID"), 
                                        Email = (string)xElement.Element("Email"), 
                                        Id = (int)xElement.Element("TradeID"), 
                                        Name = (string)xElement.Element("TradeName"), 
                                        NonTransacting =
                                            xElement.Element("NonTransacting") != null
                                            && (bool)xElement.Element("NonTransacting"), 
                                        PostCode = (string)xElement.Element("Postcode"), 
                                        Telephone = (string)xElement.Element("Telephone"), 
                                        TownCity = (string)xElement.Element("TownCity"), 
                                        TradeGroupId = (int)xElement.Element("TradeGroupID"), 
                                        Website = (string)xElement.Element("Website"),
                                        County = (string)xElement.Element("County")
                                    };
                    trades.Add(trade);
                }
            }

            return trades;
        }
    }
}