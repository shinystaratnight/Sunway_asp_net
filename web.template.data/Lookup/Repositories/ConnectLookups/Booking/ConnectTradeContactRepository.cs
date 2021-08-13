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
    /// Class ConnectTradeContactRepository.
    /// </summary>
    public class ConnectTradeContactRepository : ConnectLookupBase<TradeContact>, ITradeContactRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectTradeContactRepository" /> class.
        /// </summary>
        /// <param name="asyncLookup">A class using intuitive async cache to get lookup xml from connect.</param>
        public ConnectTradeContactRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>A list of the entity</returns>
        protected override List<TradeContact> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("TradeContact");
            XDocument xDoc = xml.ToXDocument();
            var tradeContacts = new List<TradeContact>();

            XElement element = xDoc.Element("Lookups")?.Element("TradeContacts");
            if (element != null)
            {
                foreach (XElement xElement in element.Elements("TradeContact"))
                {
                    var tradeContact = new TradeContact()
                                           {
                                               Id = (int)xElement.Element("TradeContactID"), 
                                               Address1 = (string)xElement.Element("Address1"), 
                                               Address2 = (string)xElement.Element("Address2"), 
                                               Country = (string)xElement.Element("Country"), 
                                               Email = (string)xElement.Element("Email"), 
                                               Forename = (string)xElement.Element("Forename"), 
                                               TradeContactGroupId =
                                                   (int)xElement.Element("TradeContactGroupID"), 
                                               PostCode = (string)xElement.Element("PostCode"), 
                                               Surname = (string)xElement.Element("Surname"), 
                                               Telephone = (string)xElement.Element("Telephone"), 
                                               Title = (string)xElement.Element("Title"), 
                                               TownCity = (string)xElement.Element("TownCity")
                                           };
                    tradeContacts.Add(tradeContact);
                }
            }

            return tradeContacts;
        }
    }
}