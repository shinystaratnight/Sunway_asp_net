namespace Web.Template.Application.Interfaces.PageBuilder.Factories
{
    using System.Threading;
    using System.Web;

    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Net.IVectorConnect;

    /// <summary>
    ///     A factory responsible for building Connect requests
    /// </summary>
    public interface IIVectorConnectRequestFactory
    {
        /// <summary>
        /// Creates an instance of an IVectorConnectRequest.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="context">The context.</param>
        /// <returns>An instance of an IIVectorConnectRequest class</returns>
        IIVectorConnectRequest Create(iVectorConnectRequest request, HttpContext context);

        /// <summary>
        /// Creates an instance of an IVectorConnectRequest.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="url">The URL.</param>
        /// <param name="formatType">Type of the format.</param>
        /// <param name="context">The context.</param>
        /// <returns>An instance of an IIVectorConnectRequest class</returns>
        IIVectorConnectRequest Create(iVectorConnectRequest request, string url, FormatType formatType, HttpContext context);

        /// <summary>
        /// Creates the specified URL path.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="token">The token.</param>
        /// <returns>A connect request</returns>
        IIVectorConnectRequest Create(iVectorConnectRequest request, CancellationToken token);
    }
}