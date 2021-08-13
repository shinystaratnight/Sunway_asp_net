namespace Web.Template.Application.Booking.Services
{
    using System.Collections.Generic;
    using System.Web;

    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Interfaces.Booking.Factories;
    using Web.Template.Application.Interfaces.Booking.Models;
    using Web.Template.Application.Interfaces.Booking.Services;
    using Web.Template.Application.Interfaces.PageBuilder.Factories;
    using Web.Template.Application.Net.IVectorConnect;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Service responsible for requesting the cancellation of core bookings
    /// </summary>
    /// <seealso cref="ICancellationService" />
    public class CancellationService : ICancellationService
    {
        /// <summary>
        /// The cancel component request factory
        /// </summary>
        private readonly ICancelComponentRequestFactory cancelComponentRequestFactory;

        /// <summary>
        /// The cancellation return factory
        /// </summary>
        private readonly ICancellationReturnFactory cancellationReturnFactory;

        /// <summary>
        /// The cancel request factory
        /// </summary>
        private readonly ICancelRequestFactory cancelRequestFactory;

        /// <summary>
        /// The component cancellation return factory
        /// </summary>
        private readonly IComponentCancellationReturnFactory componentCancellationReturnFactory;

        /// <summary>
        /// The connect request factory
        /// </summary>
        private readonly IIVectorConnectRequestFactory connectRequestFactory;

        /// <summary>
        /// The pre cancel component request factory
        /// </summary>
        private readonly IPreCancelComponentRequestFactory preCancelComponentRequestFactory;

        /// <summary>
        /// The pre cancel request factory
        /// </summary>
        private readonly IPreCancelRequestFactory preCancelRequestFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CancellationService" /> class.
        /// </summary>
        /// <param name="preCancelRequestFactory">The pre cancel request factory.</param>
        /// <param name="cancelRequestFactory">The cancel request factory.</param>
        /// <param name="cancellationReturnFactory">The cancellation return factory.</param>
        /// <param name="connectRequestFactory">The connect request factory.</param>
        /// <param name="preCancelComponentRequestFactory">The pre cancel component request factory.</param>
        /// <param name="cancelComponentRequestFactory">The cancel component request factory.</param>
        /// <param name="componentCancellationReturnFactory">The component cancellation return factory.</param>
        public CancellationService(
            IPreCancelRequestFactory preCancelRequestFactory, 
            ICancelRequestFactory cancelRequestFactory, 
            ICancellationReturnFactory cancellationReturnFactory, 
            IIVectorConnectRequestFactory connectRequestFactory, 
            IPreCancelComponentRequestFactory preCancelComponentRequestFactory, 
            ICancelComponentRequestFactory cancelComponentRequestFactory, 
            IComponentCancellationReturnFactory componentCancellationReturnFactory)
        {
            this.preCancelRequestFactory = preCancelRequestFactory;
            this.cancelRequestFactory = cancelRequestFactory;
            this.cancellationReturnFactory = cancellationReturnFactory;
            this.connectRequestFactory = connectRequestFactory;
            this.preCancelComponentRequestFactory = preCancelComponentRequestFactory;
            this.cancelComponentRequestFactory = cancelComponentRequestFactory;
            this.componentCancellationReturnFactory = componentCancellationReturnFactory;
        }

        /// <summary>
        /// sends a request to get information about canceling booking.
        /// </summary>
        /// <param name="cancellationModel">The cancellation model.</param>
        /// <returns>A cancellation return</returns>
        public IComponentCancellationReturn CancelComponents(IComponentCancellationModel cancellationModel)
        {
            iVectorConnectRequest requestBody = this.cancelComponentRequestFactory.Create(cancellationModel);

            IComponentCancellationReturn cancellationReturn = this.GetComponentReturn<ivci.CancelComponentResponse>(requestBody);

            return cancellationReturn;
        }

        /// <summary>
        /// sends a request to get information about canceling booking.
        /// </summary>
        /// <param name="cancellationModel">The cancellation model.</param>
        /// <returns>A cancellation return</returns>
        public ICancellationReturn PreCancelBooking(ICancellationModel cancellationModel)
        {
            iVectorConnectRequest requestBody = this.preCancelRequestFactory.Create(cancellationModel);

            ICancellationReturn cancellationReturn = this.GetReturn<ivci.PreCancelResponse>(requestBody);

            return cancellationReturn;
        }

        /// <summary>
        /// sends a request to get information about canceling booking.
        /// </summary>
        /// <param name="cancellationModel">The cancellation model.</param>
        /// <returns>A cancellation return</returns>
        public IComponentCancellationReturn PreCancelComponents(IComponentCancellationModel cancellationModel)
        {
            iVectorConnectRequest requestBody = this.preCancelComponentRequestFactory.Create(cancellationModel);

            IComponentCancellationReturn cancellationReturn = this.GetComponentReturn<ivci.PreCancelComponentResponse>(requestBody);

            return cancellationReturn;
        }

        /// <summary>
        /// Requests the booking cancellation.
        /// </summary>
        /// <param name="cancellationModel">The cancellation model.</param>
        /// <returns>A cancellation return</returns>
        public ICancellationReturn RequestBookingCancellation(ICancellationModel cancellationModel)
        {
            iVectorConnectRequest requestBody = this.cancelRequestFactory.Create(cancellationModel);

            ICancellationReturn cancellationReturn = this.GetReturn<ivci.CancelResponse>(requestBody);

            return cancellationReturn;
        }

        /// <summary>
        /// Gets the return.
        /// </summary>
        /// <typeparam name="T">The type of connect response we expect to get back</typeparam>
        /// <param name="requestBody">The request body.</param>
        /// <returns>A cancellation return</returns>
        private IComponentCancellationReturn GetComponentReturn<T>(iVectorConnectRequest requestBody) where T : class, iVectorConnectResponse, new()
        {
            IIVectorConnectRequest ivcRequest = this.connectRequestFactory.Create(requestBody, HttpContext.Current);
            T viewDocsResponse = null;

            List<string> warnings = requestBody.Validate();
            if (warnings.Count == 0)
            {
                viewDocsResponse = ivcRequest.Go<T>(true);
            }

            IComponentCancellationReturn cancellationReturn = this.componentCancellationReturnFactory.Create<T>(viewDocsResponse, warnings);

            return cancellationReturn;
        }

        /// <summary>
        /// Gets the return.
        /// </summary>
        /// <typeparam name="T">The type of connect response we expect to get back</typeparam>
        /// <param name="requestBody">The request body.</param>
        /// <returns>A cancellation return</returns>
        private ICancellationReturn GetReturn<T>(iVectorConnectRequest requestBody) where T : class, iVectorConnectResponse, new()
        {
            IIVectorConnectRequest ivcRequest = this.connectRequestFactory.Create(requestBody, HttpContext.Current);
            T viewDocsResponse = null;

            List<string> warnings = requestBody.Validate();
            if (warnings.Count == 0)
            {
                viewDocsResponse = ivcRequest.Go<T>(true);
            }

            ICancellationReturn cancellationReturn = this.cancellationReturnFactory.Create<T>(viewDocsResponse, warnings);

            return cancellationReturn;
        }
    }
}