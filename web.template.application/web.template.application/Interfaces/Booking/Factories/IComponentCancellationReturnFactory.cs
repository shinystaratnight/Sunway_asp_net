namespace Web.Template.Application.Interfaces.Booking.Factories
{
    using System.Collections.Generic;

    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Interfaces.Booking.Models;

    /// <summary>
    /// Defines a class the builds connect cancellation requests
    /// </summary>
    public interface IComponentCancellationReturnFactory
    {
        /// <summary>
        /// Creates the specified response.
        /// </summary>
        /// <typeparam name="T">The type of response we are using the build the return</typeparam>
        /// <param name="response">The response.</param>
        /// <param name="warnings">The warnings.</param>
        /// <returns>A cancellation return populated with values from the cancel or pre cancel response</returns>
        IComponentCancellationReturn Create<T>(T response, List<string> warnings) where T : class, iVectorConnectResponse, new();
    }
}