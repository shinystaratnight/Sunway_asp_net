namespace Web.Template.Application.Booking.Factories
{
    using System.Collections.Generic;
    using System.Web;

    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Booking.Models;
    using Web.Template.Application.Interfaces.Booking.Factories;
    using Web.Template.Application.Interfaces.Booking.Models;
    using Web.Template.Application.IVectorConnect.Requests;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// A class for building a pre cancel connect request
    /// </summary>
    public class CancelComponentRequestFactory : ICancelComponentRequestFactory
    {
        /// <summary>
        /// The login details factory
        /// </summary>
        private readonly IConnectLoginDetailsFactory loginDetailsFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelComponentRequestFactory"/> class.
        /// </summary>
        /// <param name="loginDetailsFactory">The login details factory.</param>
        public CancelComponentRequestFactory(IConnectLoginDetailsFactory loginDetailsFactory)
        {
            this.loginDetailsFactory = loginDetailsFactory;
        }

        /// <summary>
        /// Creates the specified cancellation model.
        /// </summary>
        /// <param name="componentCancellationModel">The component cancellation model.</param>
        /// <returns>a cancel component connect request </returns>
        public iVectorConnectRequest Create(IComponentCancellationModel componentCancellationModel)
        {
            var connectComponents = new List<ivci.CancelComponentRequest.BookingComponent>();
            foreach (CancellationComponent cancellationComponent in componentCancellationModel.CancellationComponents)
            {
                var connectComponent = new ivci.CancelComponentRequest.BookingComponent() { CancellationToken = cancellationComponent.Token, ComponentBookingID = cancellationComponent.ComponentBookingId, ComponentType = cancellationComponent.Type };
                connectComponents.Add(connectComponent);
            }

            iVectorConnectRequest cancelComponentRequest = new ivci.CancelComponentRequest()
                                                               {
                                                                   LoginDetails = this.loginDetailsFactory.Create(HttpContext.Current), 
                                                                   BookingReference = componentCancellationModel.BookingReference, 
                                                                   BookingComponents = connectComponents
                                                               };

            return cancelComponentRequest;
        }
    }
}