namespace Web.Template.Application.Interfaces.Booking.Factories
{
    using System.Collections.Generic;

    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Interfaces.Booking.Models;

    /// <summary>
    /// defines a class responsible for building cancellation returns
    /// </summary>
    public interface ICancellationReturnFactory
    {
        /// <summary>
        /// Creates the specified response.
        /// </summary>
        /// <typeparam name="T">the type of response the cancellation return will be build from</typeparam>
        /// <param name="response">The response.</param>
        /// <param name="warnings">The warnings.</param>
        /// <returns>a cancellation return</returns>
        ICancellationReturn Create<T>(T response, List<string> warnings) where T : class, iVectorConnectResponse, new();
    }
}