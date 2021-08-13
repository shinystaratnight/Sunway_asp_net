namespace Web.Template.Application.Net.IVectorConnect
{
    using System.Threading.Tasks;

    using iVectorConnectInterface.Interfaces;

    /// <summary>
    /// Connect Request Class interface.
    /// </summary>
    public interface IIVectorConnectRequest
    {
        /// <summary>
        /// Goes the specified formatType.
        /// </summary>
        /// <typeparam name="T">The type of the connect response.</typeparam>
        /// <param name="logRequest">if set to <c>true</c> [log request].</param>
        /// <returns>
        /// The response for the connect request
        /// </returns>
        T Go<T>(bool logRequest = false) where T : class, iVectorConnectResponse, new();

        /// <summary>
        /// Goes the specified formatType.
        /// </summary>
        /// <typeparam name="T">The type of the connect response.</typeparam>
        /// <param name="logRequest">if set to <c>true</c> [log request].</param>
        /// <returns>
        /// The response for the connect request
        /// </returns>
        Task<T> GoAsync<T>(bool logRequest = false) where T : class, iVectorConnectResponse, new();
    }
}