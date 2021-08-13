namespace Web.Booking.API.BookingJourney
{
    using System.Collections.Generic;
    using System.Web.Http;

    using Web.Template.Application.Booking.Enums;
    using Web.Template.Application.Booking.Models;
    using Web.Template.Application.Interfaces.Booking;
    using Web.Template.Application.Interfaces.Booking.Models;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Services;

    /// <summary>
    ///     Booking controller used to expose to the front end operations on bookings
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class BookingDetailsController : ApiController
    {
        /// <summary>
        /// The booking service
        /// </summary>
        private readonly IBookingService bookingService;

        /// <summary>
        /// The search booking model
        /// </summary>
        private readonly ISearchBookingsModel searchBookingModel;

        /// <summary>
        /// The documentation model
        /// </summary>
        private readonly IBookingDocumentationModel documentationModel;

        /// <summary>
        /// The cancellation model
        /// </summary>
        private readonly ICancellationModel cancellationModel;

        /// <summary>
        /// The component cancellation model
        /// </summary>
        private readonly IComponentCancellationModel componentCancellationModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingDetailsController"/> class.
        /// </summary>
        /// <param name="bookingService">The booking service.</param>
        /// <param name="searchBookingModel"></param>
        public BookingDetailsController(IBookingService bookingService, ISearchBookingsModel searchBookingModel, IBookingDocumentationModel documentationModel, ICancellationModel cancellationModel, IComponentCancellationModel componentCancellationModel)
        {
            this.bookingService = bookingService;
            this.searchBookingModel = searchBookingModel;
            this.documentationModel = documentationModel;
            this.cancellationModel = cancellationModel;
            this.componentCancellationModel = componentCancellationModel;
        }

        /// <summary>
        /// Retrieves the booking.
        /// </summary>
        /// <param name="bookingReference">The booking reference.</param>
        /// <returns>A booking retrieve return</returns>
        [Route("api/bookingdetails/{bookingReference}")]
        [HttpGet]
        public IBookingRetrieveReturn RetrieveBooking(string bookingReference)
        {
            return this.bookingService.GetBooking(bookingReference);
        }

        /// <summary>
        /// Retrieves the booking.
        /// </summary>
        /// <param name="tradeReference"></param>
        /// <returns>A booking retrieve return</returns>
        [Route("api/bookingdetails/Search")]
        [HttpGet]
        public IBookingSearchReturn SearchBookings([FromUri] string tradeReference = "")
        {
            this.searchBookingModel.TradeReference = tradeReference;

            return this.bookingService.SearchBookings(this.searchBookingModel);
        }

        /// <summary>
        /// Sends the booking documentation.
        /// </summary>
        /// <param name="bookingDocumentationId">The booking documentation identifier.</param>
        /// <param name="bookingReference">The booking reference.</param>
        /// <param name="overrideEmail">The override email.</param>
        /// <returns></returns>
        [Route("api/bookingdetails/documentation/{bookingDocumentationID}/Send/{bookingReference}")]
        [HttpGet]
        public IBookingDocumentationReturn SendBookingDocumentation(int bookingDocumentationId, string bookingReference, [FromUri] string overrideEmail = "")
        {
            this.documentationModel.DocumentationAction = DocumentationAction.Send;
            this.documentationModel.DocumentationId = bookingDocumentationId;
            this.documentationModel.BookingReference = bookingReference;
            this.documentationModel.OverrideEmail = overrideEmail;

            return this.bookingService.SendBookingDocumentation(this.documentationModel);
        }

        /// <summary>
        /// Sends the booking documentation.
        /// </summary>
        /// <param name="bookingDocumentationId">The booking documentation identifier.</param>
        /// <param name="bookingReference">The booking reference.</param>
        /// <returns></returns>
        [Route("api/bookingdetails/documentation/{bookingDocumentationID}/View/{bookingReference}")]
        [HttpGet]
        public IBookingDocumentationReturn ViewBookingDocumentation(int bookingDocumentationId, string bookingReference)
        {
            this.documentationModel.DocumentationAction = DocumentationAction.View;
            this.documentationModel.DocumentationId = bookingDocumentationId;
            this.documentationModel.BookingReference = bookingReference;
            this.documentationModel.OverrideEmail = "";

            return this.bookingService.ViewBookingDocumentation(this.documentationModel);
        }

        /// <summary>
        /// sends a pre cancellation request
        /// </summary>
        /// <param name="bookingReference"></param>
        /// <returns>A cancellation return, containing the cancellation cost and a token to be passed to the cancellation</returns>
        [Route("api/bookingdetails/precancel/{bookingReference}")]
        [HttpGet]
        public ICancellationReturn PreCancelBooking(string bookingReference)
        {
            this.cancellationModel.BookingReference = bookingReference;

            return this.bookingService.PreCancelBooking(this.cancellationModel);
        }

        /// <summary>
        /// Cancels the booking
        /// </summary>
        /// <param name="bookingReference">The booking reference.</param>
        /// <param name="cancellationCost">The cancellation cost.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A cancellation return, telling you whether the cancellation attempt has been an success or not
        /// </returns>
        [Route("api/bookingdetails/cancel/{bookingReference}")]
        [HttpGet]
        public ICancellationReturn CancelBooking(string bookingReference, [FromUri] decimal cancellationCost, [FromUri] string cancellationToken)
        {
            this.cancellationModel.BookingReference = bookingReference;
            this.cancellationModel.Cost = cancellationCost;
            this.cancellationModel.Token = cancellationToken;

            return this.bookingService.PreCancelBooking(this.cancellationModel);
        }

        /// <summary>
        /// Cancels the booking
        /// </summary>
        /// <param name="bookingReference">The booking reference.</param>
        /// <param name="components">The components.</param>
        /// <returns>
        /// A cancellation return, telling you whether the cancellation attempt has been an success or not
        /// </returns>
        [Route("api/bookingdetails/precancelcomponent/{bookingReference}")]
        [HttpPost]
        public IComponentCancellationReturn PreCancelComponents(string bookingReference, [FromBody] List<CancellationComponent> components)
        {
            this.componentCancellationModel.BookingReference = bookingReference;
            this.componentCancellationModel.CancellationComponents = components;

            return this.bookingService.PreCancelComponent(this.componentCancellationModel);
        }

        /// <summary>
        /// Cancels the booking
        /// </summary>
        /// <param name="bookingReference">The booking reference.</param>
        /// <param name="components">The components.</param>
        /// <returns>
        /// A cancellation return, telling you whether the cancellation attempt has been an success or not
        /// </returns>
        [Route("api/bookingdetails/cancelcomponent/{bookingReference}")]
        [HttpPost]
        public IComponentCancellationReturn CancelComponents(string bookingReference, [FromBody] List<CancellationComponent> components)
        {
            this.componentCancellationModel.BookingReference = bookingReference;
            this.componentCancellationModel.CancellationComponents = components;

            return this.bookingService.CancelComponent(this.componentCancellationModel);
        }
    }
}