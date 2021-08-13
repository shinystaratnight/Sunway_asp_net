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
    /// <seealso cref="Web.Template.Application.Interfaces.Booking.Factories.IPreCancelComponentRequestFactory" />
    public class PreCancelComponentRequestFactory : IPreCancelComponentRequestFactory
    {
        /// <summary>
        /// The login details factory
        /// </summary>
        private readonly IConnectLoginDetailsFactory loginDetailsFactory;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="PreCancelComponentRequestFactory"/> class.
        /// </summary>
        /// <param name="loginDetailsFactory">The login details factory.</param>
        public PreCancelComponentRequestFactory(IConnectLoginDetailsFactory loginDetailsFactory)
        {
            this.loginDetailsFactory = loginDetailsFactory;
        }

        /// <summary>
        /// Creates the specified cancellation model.
        /// </summary>
        /// <param name="componentCancellationModel">The component cancellation model.</param>
        /// <returns>a connect request</returns>
        public iVectorConnectRequest Create(IComponentCancellationModel componentCancellationModel)
        {
            var connectComponents = new List<ivci.PreCancelComponentRequest.BookingComponent>();
            foreach (CancellationComponent cancellationComponent in componentCancellationModel.CancellationComponents)
            {
                var connectComponent = new ivci.PreCancelComponentRequest.BookingComponent() { ComponentBookingID = cancellationComponent.ComponentBookingId, ComponentType = cancellationComponent.Type };
                connectComponents.Add(connectComponent);
            }

            iVectorConnectRequest preCancelComponentRequest = new ivci.PreCancelComponentRequest()
                                                                  {
                                                                      LoginDetails = this.loginDetailsFactory.Create(HttpContext.Current), 
                                                                      BookingReference = componentCancellationModel.BookingReference, 
                                                                      BookingComponents = connectComponents
                                                                  };

            return preCancelComponentRequest;
        }
    }
}