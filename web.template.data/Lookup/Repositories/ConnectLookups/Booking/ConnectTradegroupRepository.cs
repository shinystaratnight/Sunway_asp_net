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
    public class ConnectTradeParentGroupRepository : ConnectLookupBase<TradeParentGroup>, ITradeParentGroupRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectTradeParentGroupRepository" /> class.
        /// </summary>
        /// <param name="asyncLookup">A class using intuitive async cache to get lookup xml from connect.</param>
        public ConnectTradeParentGroupRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>
        /// A list of the entity
        /// </returns>
        protected override List<TradeParentGroup> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("TradeGroup");
            XDocument xDoc = xml.ToXDocument();
            var tradeGroups = new List<TradeParentGroup>();

            foreach (XElement xElement in xDoc.Element("Lookups")?.Element("TradeGroups").Elements("TradeGroup"))
            {
                var groupId = (int)xElement.Element("TradeGroupID");
                var parentGroupId = (int)xElement.Element("TradeParentGroupID");
                if (groupId == 0 && parentGroupId > 0)
                {
                    var tradeGroup = new TradeParentGroup()
                                         {
                                             Id = parentGroupId, 
                                             Name = (string)xElement.Element("TradeParentGroup"), 
                                         };
                    tradeGroups.Add(tradeGroup);
                }
            }

            return tradeGroups;
        }
    }
}