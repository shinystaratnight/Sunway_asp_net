namespace Web.Booking.API.Lookup
{
    using System.Collections.Generic;
    using System.Web.Http;

    using Web.Template.Application.Interfaces.Lookup.Services;
    using Web.Template.Domain.Entities.Extras;

    /// <summary>
    /// Class ExtraController.
    /// </summary>
    public class ExtraController : ApiController
    {
        /// <summary>
        /// The extra service
        /// </summary>
        private readonly IExtraService extraService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtraController"/> class.
        /// </summary>
        /// <param name="extraService">The extra service.</param>
        public ExtraController(IExtraService extraService)
        {
            this.extraService = extraService;
        }

        /// <summary>
        /// Gets the extra types.
        /// </summary>
        /// <returns>List of extra types.</returns>
        [Route("api/extra/extratypes")]
        [HttpGet]
        public List<ExtraType> GetExtraTypes()
        {
            return this.extraService.GetExtraTypes();
        }
    }
}