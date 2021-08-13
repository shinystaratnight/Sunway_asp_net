namespace Web.Template.Application.Booking.Services
{
    using System.Collections.Generic;
    using System.Web;

    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Booking.Enums;
    using Web.Template.Application.Booking.Models;
    using Web.Template.Application.Interfaces.Booking.Factories;
    using Web.Template.Application.Interfaces.Booking.Models;
    using Web.Template.Application.Interfaces.Booking.Services;
    using Web.Template.Application.Interfaces.PageBuilder.Factories;
    using Web.Template.Application.Net.IVectorConnect;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Class responsible for talking to connect regarding retrieving bookings
    /// </summary>
    /// <seealso cref="IBookingRetrieveService" />
    public class BookingDocumentationService : IBookingDocumentationService
    {
        /// <summary>
        /// The connect request factory
        /// </summary>
        private readonly IIVectorConnectRequestFactory connectRequestFactory;

        /// <summary>
        /// The booking retrieve return
        /// </summary>
        private readonly IBookingDocumentationReturn docsReturn;

        /// <summary>
        /// The send documentation request factory
        /// </summary>
        private readonly ISendDocumentationRequestFactory sendDocumentationRequestFactory;

        /// <summary>
        /// The trade login request factory
        /// </summary>
        private readonly IViewDocumentationRequestFactory viewDocumentationRequestFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingDocumentationService"/> class.
        /// </summary>
        /// <param name="viewDocumentationRequestFactory">The view documentation request factory.</param>
        /// <param name="sendDocumentationRequestFactory">The send documentation request factory.</param>
        /// <param name="docsReturn">The docs return.</param>
        /// <param name="connectRequestFactory">The connect request factory.</param>
        public BookingDocumentationService(
            IViewDocumentationRequestFactory viewDocumentationRequestFactory, 
            ISendDocumentationRequestFactory sendDocumentationRequestFactory, 
            IBookingDocumentationReturn docsReturn, 
            IIVectorConnectRequestFactory connectRequestFactory)
        {
            this.viewDocumentationRequestFactory = viewDocumentationRequestFactory;
            this.sendDocumentationRequestFactory = sendDocumentationRequestFactory;
            this.docsReturn = docsReturn;
            this.connectRequestFactory = connectRequestFactory;
        }

        /// <summary>
        /// Talks to connect to send the documentation for the passed in booking
        /// </summary>
        /// <param name="docsModel">A documentation model containing information of what document we want to send, who we want to send it to and for what booking</param>
        /// <returns>A model that shows if the request was successful and any warnings that were raised</returns>
        public IBookingDocumentationReturn SendDocumentation(IBookingDocumentationModel docsModel)
        {
            this.docsReturn.Success = false;

            iVectorConnectRequest requestBody = this.sendDocumentationRequestFactory.Create(docsModel);

            this.docsReturn.Warnings = requestBody.Validate();

            if (this.docsReturn.Warnings.Count == 0)
            {
                this.GetSendDocumentationResponse(requestBody);
            }

            return this.docsReturn;
        }

        /// <summary>
        /// Talks to connect to generate and return links to the documentation for the passed in booking id
        /// </summary>
        /// <param name="docsModel">A documentation model containing information of what document we want to view, and for what booking</param>
        /// <returns>A model that shows if the request was successful and any warnings that were raised</returns>
        public IBookingDocumentationReturn ViewDocumentation(IBookingDocumentationModel docsModel)
        {
            this.docsReturn.Success = false;

            iVectorConnectRequest requestBody = this.viewDocumentationRequestFactory.Create(docsModel);

            this.docsReturn.Warnings = requestBody.Validate();

            if (this.docsReturn.Warnings.Count == 0)
            {
                this.GetViewDocumentationResponse(requestBody);
            }

            return this.docsReturn;
        }

        /// <summary>
        /// Gets the response, and passes it on to the bookingAdaptor
        /// </summary>
        /// <param name="requestBody">The request body.</param>
        private void GetSendDocumentationResponse(iVectorConnectRequest requestBody)
        {
            IIVectorConnectRequest ivcRequest = this.connectRequestFactory.Create(requestBody, HttpContext.Current);
            ivci.SendDocumentationResponse sendDocsResponse = ivcRequest.Go<ivci.SendDocumentationResponse>();

            this.docsReturn.Success = sendDocsResponse.ReturnStatus.Success;
            this.docsReturn.Warnings.AddRange(sendDocsResponse.ReturnStatus.Exceptions);
            this.docsReturn.DocumentationAction = DocumentationAction.Send;
        }

        /// <summary>
        /// Gets the response, and passes it on to the bookingAdaptor
        /// </summary>
        /// <param name="requestBody">The request body.</param>
        private void GetViewDocumentationResponse(iVectorConnectRequest requestBody)
        {
            IIVectorConnectRequest ivcRequest = this.connectRequestFactory.Create(requestBody, HttpContext.Current);
            ivci.ViewDocumentationResponse viewDocsResponse = ivcRequest.Go<ivci.ViewDocumentationResponse>();

            this.docsReturn.Success = viewDocsResponse.ReturnStatus.Success;
            this.docsReturn.DocumentationAction = DocumentationAction.View;
            this.docsReturn.Warnings.AddRange(viewDocsResponse.ReturnStatus.Exceptions);

            if (this.docsReturn.Warnings.Count == 0)
            {
                if (viewDocsResponse.DocumentPaths.Count > 0)
                {
                    this.docsReturn.DocumentPaths = new List<string>();
                    this.docsReturn.DocumentPaths.AddRange(viewDocsResponse.DocumentPaths);
                }
                else
                {
                    this.docsReturn.Warnings.Add("No documents returned");
                }
            }
        }
    }
}