namespace Web.Template.Application.Services
{
    using System.Collections.Generic;
    using System.Xml;

    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.PageDefinition;
    using Web.Template.Application.Utillity;

    /// <summary>
    ///     Service used to retrieve pages from the site builder
    /// </summary>
    /// <seealso cref="IOffersService" />
    public class OffersService : IOffersService
    {
        /// <summary>
        /// The custom query
        /// </summary>
        private ICustomQuery customQuery;

        /// <summary>
        /// Initializes a new instance of the <see cref="OffersService"/> class.
        /// </summary>
        /// <param name="customQuery">The custom query.</param>
        public OffersService(ICustomQuery customQuery)
        {
            this.customQuery = customQuery;
        }

        /// <summary>
        /// Gets the offer by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="currencyId">The currency identifier.</param>
        /// <returns>
        /// an xml document container details of the offer for the matching id.
        /// </returns>
        public XmlDocument GetOfferById(int id, int currencyId)
        {
            XmlDocument offerXml = this.customQuery.GetCustomQueryXml(new List<string>() { id.ToString(), currencyId.ToString() }, "Special Offer By Id");

            return offerXml;
        }

        /// <summary>
        /// Gets the offer pages.
        /// </summary>
        /// <returns>A List of all offers pages</returns>
        public List<EntityPage> GetOfferPages()
        {
            XmlDocument customQueryXml = this.customQuery.GetCustomQueryXml(new List<string>(), "Special Offer List");

            List<EntityPage> offerPages = XMLFunctions.XMLToGenericList<EntityPage>(customQueryXml, "CustomQueryResponse/CustomXML/EntityPages/EntityPage");

            return offerPages;
        }

        /// <summary>
        /// Gets the offers.
        /// </summary>
        /// <param name="paramList">The parameter list.</param>
        /// <returns>
        /// The offers
        /// </returns>
        public XmlDocument GetOffersByParams(List<string> paramList)
        {
            XmlDocument offersXml = this.customQuery.GetCustomQueryXml(paramList, "Special Offers");
            return offersXml;
        }
    }
}