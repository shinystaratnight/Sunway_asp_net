namespace Web.Template.Application.Interfaces.Lookup.Services
{
    using System.Collections.Generic;

    using Web.Template.Domain.Entities.Geography;

    /// <summary>
    /// Service responsible for access to Geography Information
    /// </summary>
    public interface IGeographyService
    {
        /// <summary>
        /// Gets the countries.
        /// </summary>
        /// <returns>All countries</returns>
        List<Country> GetCountries();

        /// <summary>
        /// Gets the country.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A country that matches the given ID</returns>
        Country GetCountry(int id);

        /// <summary>
        /// Gets the region.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A region that matches the given identifier</returns>
        Region GetRegion(int id);

        /// <summary>
        /// Gets the regions.
        /// </summary>
        /// <returns>All regions</returns>
        List<Region> GetRegions();

        /// <summary>
        /// Gets the resort.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A resort that matches the given identifier</returns>
        Resort GetResort(int id);

        /// <summary>
        /// Gets the resorts.
        /// </summary>
        /// <returns>All resorts</returns>
        List<Resort> GetResorts();
    }
}