namespace Web.Template.Application.Interfaces.Lookup.Services
{
    using System.Collections.Generic;

    using Web.Template.Domain.Entities.Booking;

    /// <summary>
    /// Booking service interface, defining a class responsible for managing access to booking information.
    /// </summary>
    public interface IBookingService
    {
        /// <summary>
        /// Gets all booking countries.
        /// </summary>
        /// <returns>A list of booking countries</returns>
        List<BookingCountry> GetAllBookingCountries();

        /// <summary>
        /// Gets all booking documentation.
        /// </summary>
        /// <returns>A List of BookingDocumentation</returns>
        List<BookingDocumentation> GetAllBookingDocumentation();

        /// <summary>
        /// Gets the brands.
        /// </summary>
        /// <returns>A list of brands</returns>
        List<Brand> GetAllBrands();

        /// <summary>
        /// Gets all brands geographies.
        /// </summary>
        /// <returns>All the geographies associated with the brand</returns>
        List<Brand> GetAllBrandsGeographies();

        /// <summary>
        /// Gets all languages.
        /// </summary>
        /// <returns>A list of Languages</returns>
        List<Language> GetAllLanguages();

        /// <summary>
        /// Gets all nationality.
        /// </summary>
        /// <returns>a list of nationality</returns>
        List<Nationality> GetAllNationality();

        /// <summary>
        /// Gets the booking country.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A Booking country</returns>
        BookingCountry GetBookingCountry(int id);

        /// <summary>
        /// Gets the booking documentation.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A single booking documentation.</returns>
        BookingDocumentation GetBookingDocumentation(int id);

        /// <summary>
        /// Gets the brand.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A single brand.</returns>
        Brand GetBrand(int id);

        /// <summary>
        /// Gets the brand with geography.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Gets a brand and all its geographies</returns>
        Brand GetBrandWithGeography(int id);

        /// <summary>
        /// Gets the language.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A single Language</returns>
        Language GetLanguage(int id);

        /// <summary>
        /// Gets the nationality.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A single nationality.</returns>
        Nationality GetNationality(int id);

        /// <summary>
        /// Gets the sales channel.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A single sales channel.</returns>
        SalesChannel GetSalesChannel(int id);

        /// <summary>
        /// Gets the sales channel.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>A single sales channel.</returns>
        SalesChannel GetSalesChannel(string name);

        /// <summary>
        /// Gets the selling exchange rate.
        /// </summary>
        /// <param name="sellingCurrencyId">The selling currency identifier.</param>
        /// <returns>The selling exchange rate.</returns>
        SellingExchangeRate GetSellingExchangeRate(int sellingCurrencyId);
    }
}