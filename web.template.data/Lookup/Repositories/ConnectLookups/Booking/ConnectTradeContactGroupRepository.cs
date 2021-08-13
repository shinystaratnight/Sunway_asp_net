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
    /// Class ConnectTradeContactGroupRepository.
    /// </summary>
    public class ConnectTradeContactGroupRepository : ConnectLookupBase<TradeContactGroup>, ITradeContactGroupRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectTradeContactGroupRepository" /> class.
        /// </summary>
        /// <param name="asyncLookup">A class using intuitive async cache to get lookup xml from connect.</param>
        public ConnectTradeContactGroupRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>A list of trade contact groups</returns>
        protected override List<TradeContactGroup> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("TradeContactGroup");
            XDocument xDoc = xml.ToXDocument();
            var tradeContactGroups = new List<TradeContactGroup>();

            XElement element = xDoc.Element("Lookups")?.Element("TradeContactGroups");
            if (element != null)
            {
                foreach (XElement xElement in element.Elements("TradeContactGroup"))
                {
                    var tradeContactGroup = new TradeContactGroup()
                                                {
                                                    Id = (int)xElement.Element("TradeContactGroupID"), 
                                                    Name =
                                                        (string)xElement.Element("TradeContactGroup")
                                                };
                    tradeContactGroups.Add(tradeContactGroup);
                }
            }

            return tradeContactGroups;
        }
    }
}