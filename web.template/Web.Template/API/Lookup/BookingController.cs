namespace Web.Template.API.Lookup
{
    using System.Collections.Generic;
    using System.Web.Http;

    using Web.Template.Application.Interfaces.Lookup.Services;
    using Web.Template.Domain.Entities.Booking;

    /// <summary>
    /// A lookup controller responsible for exposing to the front end any information concerning the booking.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class BookingController : ApiController
    {
        /// <summary>
        /// The booking service
        /// </summary>
        private readonly IBookingService bookingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingController"/> class.
        /// </summary>
        /// <param name="bookingService">The booking service.</param>
        public BookingController(IBookingService bookingService)
        {
            this.bookingService = bookingService;
        }

        /// <summary>
        /// Gets all booking documentation.
        /// </summary>
        /// <returns>All booking documentation.</returns>
        [Route("api/booking/documentation")]
        [HttpGet]
        public List<BookingDocumentation> GetAllBookingDocumentation()
        {
            return this.bookingService.GetAllBookingDocumentation();
        }

        /// <summary>
        /// Gets all brand and geography.
        /// </summary>
        /// <returns>the brand and geographies</returns>
        [Route("api/booking/brandandgeography")]
        [HttpGet]
        public List<Brand> GetAllBrandAndGeography()
        {
            return this.bookingService.GetAllBrandsGeographies();
        }

        /// <summary>
        /// Gets all the brands.
        /// </summary>
        /// <returns>All brands</returns>
        [Route("api/booking/brand")]
        [HttpGet]
        public List<Brand> GetAllBrands()
        {
            return this.bookingService.GetAllBrands();
        }

        /// <summary>
        /// Gets all languages.
        /// </summary>
        /// <returns>All languages</returns>
        [Route("api/booking/language")]
        [HttpGet]
        public List<Language> GetAllLanguages()
        {
            return this.bookingService.GetAllLanguages();
        }

        /// <summary>
        /// Gets all nationality.
        /// </summary>
        /// <returns>All nationalities</returns>
        [Route("api/booking/nationality")]
        [HttpGet]
        public List<Nationality> GetAllNationality()
        {
            return this.bookingService.GetAllNationality();
        }

        /// <summary>
        /// Gets the booking country.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A booking country that matches the provided Id</returns>
        [Route("api/booking/country/{id}")]
        [HttpGet]
        public BookingCountry GetBookingCountry(int id)
        {
            return this.bookingService.GetBookingCountry(id);
        }

        /// <summary>
        /// Gets the booking documentation.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A booking documentation that matches a provided Id.</returns>
        [Route("api/booking/documentation/{id}")]
        [HttpGet]
        public BookingDocumentation GetBookingDocumentation(int id)
        {
            return this.bookingService.GetBookingDocumentation(id);
        }

        /// <summary>
        /// Gets the brand.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A brand that matches the given Id.</returns>
        [Route("api/booking/brand/{id}")]
        [HttpGet]
        public Brand GetBrand(int id)
        {
            return this.bookingService.GetBrand(id);
        }

        /// <summary>
        /// Gets the brand with geography.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>the brand with geography</returns>
        [Route("api/booking/brandandgeography/{id}")]
        [HttpGet]
        public Brand GetBrandWithGeography(int id)
        {
            return this.bookingService.GetBrandWithGeography(id);
        }

        /// <summary>
        /// Gets the language.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A language that matches the given Id.</returns>
        [Route("api/booking/language/{id}")]
        [HttpGet]
        public Language GetLanguage(int id)
        {
            return this.bookingService.GetLanguage(id);
        }

        /// <summary>
        /// Gets the nationality.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A nationality that matches the given Id.</returns>
        [Route("api/booking/nationality/{id}")]
        [HttpGet]
        public Nationality GetNationality(int id)
        {
            return this.bookingService.GetNationality(id);
        }

        /// <summary>
        /// Gets the sales channel.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A sales channel</returns>
        [Route("api/booking/saleschannel/{id}")]
        [HttpGet]
        public SalesChannel GetSalesChannel(int id)
        {
            return this.bookingService.GetSalesChannel(id);
        }
    }
}