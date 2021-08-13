namespace Web.Template.Application.Booking.Services
{
    using System.Web;

    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Booking.Adapters;
    using Web.Template.Application.Interfaces.Booking.Adapters;
    using Web.Template.Application.Interfaces.Booking.Factories;
    using Web.Template.Application.Interfaces.Booking.Models;
    using Web.Template.Application.Interfaces.Booking.Services;
    using Web.Template.Application.Interfaces.PageBuilder.Factories;
    using Web.Template.Application.Net.IVectorConnect;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Defines a class that can return direct debit search results
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Booking.Services.IDirectDebitRetrieveService" />
    public class DirectDebitRetrieveService : IDirectDebitRetrieveService
    {
        /// <summary>
        /// The direct debit adapter
        /// </summary>
        private readonly IDirectDebitAdapter directDebitAdapter;

        /// <summary>
        /// The module request factory
        /// </summary>
        private readonly IModuleRequestFactory moduleRequestFactory;

        /// <summary>
        /// The connect request factory
        /// </summary>
        private readonly IIVectorConnectRequestFactory connectRequestFactory;

        /// <summary>
        /// The booking retrieve return
        /// </summary>
        private IDirectDebitRetrieveReturn directDebitRetrieveReturn;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectDebitRetrieveService" /> class.
        /// </summary>
        /// <param name="connectRequestFactory">The connect request factory.</param>
        /// <param name="directDebitRetrieveReturn">The direct debit retrieve return.</param>
        /// <param name="moduleRequestFactory">The module request factory.</param>
        /// <param name="directDebitAdapter">The direct debit adapter.</param>
        public DirectDebitRetrieveService(
            IIVectorConnectRequestFactory connectRequestFactory,
            IDirectDebitRetrieveReturn directDebitRetrieveReturn,
            IModuleRequestFactory moduleRequestFactory,
            IDirectDebitAdapter directDebitAdapter)
        {
            this.connectRequestFactory = connectRequestFactory;
            this.directDebitRetrieveReturn = directDebitRetrieveReturn;
            this.moduleRequestFactory = moduleRequestFactory;
            this.directDebitAdapter = directDebitAdapter;
        }

        /// <summary>
        /// Retrieves the direct debits.
        /// </summary>
        /// <returns>an IDirectDebitRetrieveReturn</returns>
        public IDirectDebitRetrieveReturn RetrieveDirectDebits()
        {
            this.directDebitRetrieveReturn.RetrieveSuccessful = false;

            iVectorConnectRequest requestBody = this.moduleRequestFactory.Create(
                "DirectDebitManagement",
                "directdebit/retrieve",
                "GET");

            this.directDebitRetrieveReturn.Warnings = requestBody.Validate();

            if (this.directDebitRetrieveReturn.Warnings.Count == 0)
            {
                this.GetResponse(requestBody);
            }

            return this.directDebitRetrieveReturn;
        }

        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <param name="requestBody">The request body.</param>
        private void GetResponse(iVectorConnectRequest requestBody)
        {
            IIVectorConnectRequest ivcRequest = this.connectRequestFactory.Create(requestBody, "module", FormatType.JSON, HttpContext.Current);

            ivci.Modules.ModuleResponse moduleResponse = ivcRequest.Go<ivci.Modules.ModuleResponse>();

            if (moduleResponse != null)
            {
                this.directDebitRetrieveReturn.RetrieveSuccessful = moduleResponse.ReturnStatus.Success;
                this.directDebitRetrieveReturn.Warnings.AddRange(moduleResponse.ReturnStatus.Exceptions);

                if (this.directDebitRetrieveReturn.Warnings.Count == 0 && moduleResponse.Response != null)
                {
                    this.directDebitRetrieveReturn = this.directDebitAdapter.CreateBookingLineFromDirectDebitRetrieveResponse(moduleResponse);
                }
            }
        }
    }
}