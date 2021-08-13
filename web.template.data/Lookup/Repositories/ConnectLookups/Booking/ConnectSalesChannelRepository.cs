namespace Web.Template.Data.Lookup.Repositories.ConnectLookups.Booking
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Linq;

    using Intuitive;

    using Web.Template.Data.Connect;
    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Booking;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Booking;

    /// <summary>
    /// Class ConnectSalesChannelRepository.
    /// </summary>
    public class ConnectSalesChannelRepository : ConnectLookupBase<SalesChannel>, ISalesChannelRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectSalesChannelRepository" /> class.
        /// </summary>
        /// <param name="asyncLookup">A class using intuitive async cache to get lookup xml from connect.</param>
        public ConnectSalesChannelRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>A list of the entity</returns>
        protected override List<SalesChannel> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("SalesChannel");
            XDocument xdoc = xml.ToXDocument();

            var salesChannels = new List<SalesChannel>();

            XElement element = xdoc.Element("Lookups")?.Element("SalesChannels");
            if (element != null)
            {
                foreach (XElement xElement in element.Elements("SalesChannel"))
                {
                    var salesChannel = new SalesChannel()
                                           {
                                               Id = (int)xElement.Element("SalesChannelID"), 
                                               Name = (string)xElement.Element("SalesChannel")
                                           };
                    salesChannels.Add(salesChannel);
                }
            }

            return salesChannels;
        }
    }
}