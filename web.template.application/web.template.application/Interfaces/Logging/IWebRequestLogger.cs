namespace Web.Template.Application.Interfaces.Logging
{
    using System.Net;

    /// <summary>
    ///     Interface for logging web requests
    /// </summary>
    public interface IWebRequestLogger
    {
        /// <summary>
        /// Logs the specified request and response.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="requestBody">The request body.</param>
        /// <param name="moduleName">Name of the module.</param>
        /// <param name="response">The response.</param>
        /// <param name="responseBody">The response body.</param>
        void Log(WebRequest request, string requestBody, string moduleName, WebResponse response, string responseBody);

        /// <summary>
        ///     Logs the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="requestBody">The request body.</param>
        /// <param name="exception">The exception.</param>
        void Log(WebRequest request, string requestBody, WebException exception);
    }
}