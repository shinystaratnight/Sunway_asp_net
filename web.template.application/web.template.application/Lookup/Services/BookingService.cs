namespace Web.Template.Application.Lookup.Services
{
    using System.Collections.Generic;
    using System.Linq;

    using Web.Template.Application.Interfaces.Lookup.Services;
    using Web.Template.Domain.Entities.Booking;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Booking;

    /// <summary>
    /// Service responsible for retrieving all lookup information concerning bookings.
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Lookup.Services.IBookingService" />
    public class BookingService : IBookingService
    {
        /// <summary>
        /// The booking country repository
        /// </summary>
        private readonly IBookingCountryRepository bookingCountryRepository;

        /// <summary>
        /// The booking documentation repository
        /// </summary>
        private readonly IBookingDocumentationRepository bookingDocumentationRepository;

        /// <summary>
        /// The brand repository
        /// </summary>
        private readonly IBrandRepository brandRepository;

        /// <summary>
        /// The language repository
        /// </summary>
        private readonly ILanguageRepository languageRepository;

        /// <summary>
        /// The nationality repository
        /// </summary>
        private readonly INationalityRepository nationalityRepository;

        /// <summary>
        /// The sales channel repository
        /// </summary>
        private readonly ISalesChannelRepository salesChannelRepository;

        /// <summary>
        /// The selling exchange rate repository
        /// </summary>
        private readonly ISellingExchangeRateRepository sellingExchangeRateRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingService" /> class.
        /// </summary>
        /// <param name="bookingCountryRepository">The booking country repository.</param>
        /// <param name="bookingDocumentationRepository">The booking documentation repository.</param>
        /// <param name="brandRepository">The brand repository.</param>
        /// <param name="languageRepository">The language repository.</param>
        /// <param name="nationalityRepository">The nationality repository.</param>
        /// <param name="salesChannelRepository">The sales channel repository.</param>
        /// <param name="sellingExchangeRateRepository">The selling exchange rate repository.</param>
        public BookingService(
            IBookingCountryRepository bookingCountryRepository, 
            IBookingDocumentationRepository bookingDocumentationRepository, 
            IBrandRepository brandRepository, 
            ILanguageRepository languageRepository, 
            INationalityRepository nationalityRepository, 
            ISalesChannelRepository salesChannelRepository,
            ISellingExchangeRateRepository sellingExchangeRateRepository)
        {
            this.bookingCountryRepository = bookingCountryRepository;
            this.bookingDocumentationRepository = bookingDocumentationRepository;
            this.brandRepository = brandRepository;
            this.languageRepository = languageRepository;
            this.nationalityRepository = nationalityRepository;
            this.salesChannelRepository = salesChannelRepository;
            this.sellingExchangeRateRepository = sellingExchangeRateRepository;
        }

        /// <summary>
        /// Gets all booking countries.
        /// </summary>
        /// <returns>A list of booking countries</returns>
        public List<BookingCountry> GetAllBookingCountries()
        {
            return this.bookingCountryRepository.GetAll().ToList();
        }

        /// <summary>
        /// Gets all booking documentation.
        /// </summary>
        /// <returns>
        /// A List of BookingDocumentation
        /// </returns>
        public List<BookingDocumentation> GetAllBookingDocumentation()
        {
            return this.bookingDocumentationRepository.GetAll().ToList();
        }

        /// <summary>
        /// Gets the brands.
        /// </summary>
        /// <returns>A list of all brands.</returns>
        public List<Brand> GetAllBrands()
        {
            return this.brandRepository.GetAll().ToList();
        }

        /// <summary>
        /// Gets all brands geographies.
        /// </summary>
        /// <returns>
        /// All the geographies associated with the brand
        /// </returns>
        public List<Brand> GetAllBrandsGeographies()
        {
            return this.brandRepository.GetBrandsWithGeography().ToList();
        }

        /// <summary>
        /// Gets all languages.
        /// </summary>
        /// <returns>
        /// A list of Languages
        /// </returns>
        public List<Language> GetAllLanguages()
        {
            return this.languageRepository.GetAll().ToList();
        }

        /// <summary>
        /// Gets all nationality.
        /// </summary>
        /// <returns>
        /// a list of nationality
        /// </returns>
        public List<Nationality> GetAllNationality()
        {
            return this.nationalityRepository.GetAll().ToList();
        }

        /// <summary>
        /// Gets the booking country.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// A Booking country
        /// </returns>
        public BookingCountry GetBookingCountry(int id)
        {
            return this.bookingCountryRepository.GetSingle(id);
        }

        /// <summary>
        /// Gets the booking documentation.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// A single booking documentation.
        /// </returns>
        public BookingDocumentation GetBookingDocumentation(int id)
        {
            return this.bookingDocumentationRepository.GetSingle(id);
        }

        /// <summary>
        /// Gets the brand.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// A single brand.
        /// </returns>
        public Brand GetBrand(int id)
        {
            return this.brandRepository.GetSingle(id);
        }

        /// <summary>
        /// Gets the brand with geography.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// Gets a brand and all its geographies
        /// </returns>
        public Brand GetBrandWithGeography(int id)
        {
            return this.brandRepository.GetBrandWithGeography(id);
        }

        /// <summary>
        /// Gets the language.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// A single Language
        /// </returns>
        public Language GetLanguage(int id)
        {
            return this.languageRepository.GetSingle(id);
        }

        /// <summary>
        /// Gets the nationality.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// A single nationality.
        /// </returns>
        public Nationality GetNationality(int id)
        {
            return this.nationalityRepository.GetSingle(id);
        }

        /// <summary>
        /// Gets the sales channel.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// A single sales channel.
        /// </returns>
        public SalesChannel GetSalesChannel(int id)
        {
            return this.salesChannelRepository.GetSingle(id);
        }

        /// <summary>
        /// Gets the sales channel.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>A single sales channel.</returns>
        public SalesChannel GetSalesChannel(string name)
        {
            return this.salesChannelRepository.FindBy(sc => sc.Name == name).FirstOrDefault();
        }

        /// <summary>
        /// Gets the selling exchange rate.
        /// </summary>
        /// <param name="sellingCurrencyId">The selling currency identifier.</param>
        /// <returns>The selling exchange rate.</returns>
        public SellingExchangeRate GetSellingExchangeRate(int sellingCurrencyId)
        {
            return this.sellingExchangeRateRepository.GetSingle(sellingCurrencyId);
        }
    }
}