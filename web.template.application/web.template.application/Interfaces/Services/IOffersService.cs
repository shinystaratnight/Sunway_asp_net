namespace Web.Template.Application.Interfaces.Services
{
    using System.Collections.Generic;
    using System.Xml;

    using Web.Template.Application.PageDefinition;

    /// <summary>
    /// service responsible for retrieving offers
    /// </summary>
    public interface IOffersService
    {
        /// <summary>
        /// Gets the offer by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="currencyId">The currency identifier.</param>
        /// <returns>an xml document container details of the offer for the matching id.</returns>
        XmlDocument GetOfferById(int id, int currencyId);

        /// <summary>
        /// Gets the offer pages.
        /// </summary>
        /// <returns>A list of entity pages</returns>
        List<EntityPage> GetOfferPages();

        /// <summary>
        /// Gets the offers.
        /// </summary>
        /// <param name="paramList">The parameter list.</param>
        /// <returns>The offers</returns>
        XmlDocument GetOffersByParams(List<string> paramList);
    }
}