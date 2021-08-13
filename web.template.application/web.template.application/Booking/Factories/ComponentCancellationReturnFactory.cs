namespace Web.Template.Application.Booking.Factories
{
    using System;
    using System.Collections.Generic;

    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Booking.Enums;
    using Web.Template.Application.Booking.Models;
    using Web.Template.Application.Interfaces.Booking.Factories;
    using Web.Template.Application.Interfaces.Booking.Models;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Factory for building returns models of component cancellations. Takes a connect component response and processes the values
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Booking.Factories.IComponentCancellationReturnFactory" />
    public class ComponentCancellationReturnFactory : IComponentCancellationReturnFactory
    {
        /// <summary>
        /// Creates the specified response.
        /// </summary>
        /// <typeparam name="T">The type of the connect response used to build the return</typeparam>
        /// <param name="response">The response.</param>
        /// <param name="warnings">The warnings.</param>
        /// <returns>
        /// A cancellation return populated with values from the cancel or pre cancel response
        /// </returns>
        public IComponentCancellationReturn Create<T>(T response, List<string> warnings) where T : class, iVectorConnectResponse, new()
        {
            if (warnings == null)
            {
                warnings = new List<string>();
            }

            var cancellationReturn = new ComponentCancellationReturn() { Warnings = warnings, Success = response != null && response.ReturnStatus.Success, CancellationComponents = new List<CancellationComponent>() };
            if (response != null)
            {
                cancellationReturn.Warnings.AddRange(response.ReturnStatus.Exceptions);

                Type responseType = response.GetType();
                if (responseType == typeof(ivci.PreCancelComponentResponse))
                {
                    this.ProcessPreCancel(response as ivci.PreCancelComponentResponse, cancellationReturn);
                }
                else if (responseType == typeof(ivci.CancelComponentResponse))
                {
                    this.ProcessCancel(response as ivci.CancelComponentResponse, cancellationReturn);
                }
            }

            return cancellationReturn;
        }

        /// <summary>
        /// Processes the cancel response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="cancellationReturn">The cancellation return.</param>
        private void ProcessCancel(ivci.CancelComponentResponse response, IComponentCancellationReturn cancellationReturn)
        {
            cancellationReturn.Stage = CancellationStage.ConfirmationCancellation;
        }

        /// <summary>
        /// Processes the pre cancel response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="cancellationReturn">The cancellation return.</param>
        private void ProcessPreCancel(ivci.PreCancelComponentResponse response, IComponentCancellationReturn cancellationReturn)
        {
            cancellationReturn.Stage = CancellationStage.PreCancellation;

            foreach (ivci.PreCancelComponentResponse.BookingComponent bookingComponent in response.BookingComponents)
            {
                var cancellationComponent = new CancellationComponent()
                                                {
                                                    ComponentBookingId = bookingComponent.ComponentBookingID, 
                                                    CancellationCost = bookingComponent.CancellationCost, 
                                                    Token = bookingComponent.CancellationToken, 
                                                    Type = bookingComponent.ComponentType
                                                };
                cancellationReturn.CancellationComponents.Add(cancellationComponent);
            }
        }
    }
}