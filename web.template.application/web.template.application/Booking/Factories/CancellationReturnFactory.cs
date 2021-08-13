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
    /// Class responsible for building cancellation returns
    /// </summary>
    /// <seealso cref="ICancellationReturnFactory" />
    public class CancellationReturnFactory : ICancellationReturnFactory
    {
        /// <summary>
        /// Creates the specified response.
        /// </summary>
        /// <typeparam name="T">The type of the response that we will be using to build the return</typeparam>
        /// <param name="response">The response.</param>
        /// <param name="warnings">The warnings.</param>
        /// <returns>
        /// A cancellation return populated with values from the cancel or pre cancel response
        /// </returns>
        public ICancellationReturn Create<T>(T response, List<string> warnings) where T : class, iVectorConnectResponse, new()
        {
            if (warnings == null)
            {
                warnings = new List<string>();
            }

            var cancellationReturn = new CancellationReturn { Warnings = warnings, Success = response.ReturnStatus.Success };
            cancellationReturn.Warnings.AddRange(response.ReturnStatus.Exceptions);

            Type responseType = response.GetType();
            if (responseType == typeof(ivci.PreCancelResponse))
            {
                this.ProcessPreCancel(response as ivci.PreCancelResponse, cancellationReturn);
            }
            else if (responseType == typeof(ivci.CancelResponse))
            {
                this.ProcessCancel(response as ivci.CancelResponse, cancellationReturn);
            }

            return cancellationReturn;
        }

        /// <summary>
        /// Processes the cancel response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="cancellationReturn">The cancellation return.</param>
        private void ProcessCancel(ivci.CancelResponse response, ICancellationReturn cancellationReturn)
        {
            cancellationReturn.Stage = CancellationStage.ConfirmationCancellation;
        }

        /// <summary>
        /// Processes the pre cancel response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="cancellationReturn">The cancellation return.</param>
        private void ProcessPreCancel(ivci.PreCancelResponse response, ICancellationReturn cancellationReturn)
        {
            cancellationReturn.Stage = CancellationStage.PreCancellation;
            cancellationReturn.Cost = response.CancellationCost;
            cancellationReturn.Token = response.CancellationToken;
        }
    }
}