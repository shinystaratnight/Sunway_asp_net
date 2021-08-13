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
    ///     Booking documentation Repository that is responsible for managing access to airports
    /// </summary>
    public class ConnectTradeGroupRepository : ConnectLookupBase<TradeGroup>, ITradeGroupRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectTradeGroupRepository" /> class.
        /// </summary>
        /// <param name="asyncLookup">A class using intuitive async cache to get lookup xml from connect.</param>
        public ConnectTradeGroupRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>A list of Booking documentation</returns>
        protected override List<TradeGroup> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("TradeGroup");
            XDocument xDoc = xml.ToXDocument();
            var tradeGroups = new List<TradeGroup>();

            foreach (XElement xElement in xDoc.Element("Lookups")?.Element("TradeGroups").Elements("TradeGroup"))
            {
                var id = (int)xElement.Element("TradeGroupID");
                if (id > 0)
                {
                    var tradeGroup = new TradeGroup()
                                         {
                                             Id = (int)xElement.Element("TradeGroupID"), 
                                             Name = (string)xElement.Element("TradeGroup"), 
                                             TradeParentGroup = (string)xElement.Element("TradeParentGroup"), 
                                             TradeParentGroupId =
                                                 (string)xElement.Element("TradeParentGroupID"), 
                                         };
                    tradeGroups.Add(tradeGroup);
                }
            }

            return tradeGroups;
        }
    }
}