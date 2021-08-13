namespace Web.Template.Application.Net.Logging
{
    using System;
    using System.Net;

    using Intuitive;

    using Web.Template.Application.Interfaces.Logging;

    /// <summary>
    ///     Logs information from the web request to a logging folder
    /// </summary>
    /// <seealso cref="IWebRequestLogger" />
    public class WebRequestLogger : IWebRequestLogger
    {
        /// <summary>
        ///     The exception formatter
        /// </summary>
        private readonly WebExceptionLogFormatter exceptionFormatter;

        /// <summary>
        ///     Write the log
        /// </summary>
        private readonly ILogWriter logWriter;

        /// <summary>
        ///     The request formatter
        /// </summary>
        private readonly WebRequestLogFormatter requestFormatter;

        /// <summary>
        ///     Initializes a new instance of the <see cref="WebRequestLogger" /> class.
        /// </summary>
        /// <param name="requestFormatter">The request formatter to control how the request is displayed.</param>
        /// <param name="exceptionFormatter">The request formatter to control how the exception is displayed.</param>
        /// <param name="logWriter">The log writer.</param>
        public WebRequestLogger(WebRequestLogFormatter requestFormatter, WebExceptionLogFormatter exceptionFormatter, ILogWriter logWriter)
        {
            this.requestFormatter = requestFormatter;
            this.exceptionFormatter = exceptionFormatter;
            this.logWriter = logWriter;
        }

        /// <summary>
        /// Logs the specified request and response
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="requestBody">The request body.</param>
        /// <param name="moduleName">Name of the module.</param>
        /// <param name="response">The response.</param>
        /// <param name="responseBody">The response body</param>
        public void Log(WebRequest request, string requestBody, string moduleName, WebResponse response, string responseBody)
        {
            string logTitle = "request";
            string logContent = string.Join(Environment.NewLine, this.requestFormatter.Format(request, requestBody));
            this.logWriter.Write(moduleName, logTitle, logContent);

            string responseTitle = "response";
            string responseContent = string.Join(Environment.NewLine, this.requestFormatter.Format(request, responseBody));
            this.logWriter.Write(moduleName, responseTitle, responseContent);
        }

        /// <summary>
        ///     Logs the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="requestBody">The request body.</param>
        /// <param name="exception">The exception.</param>
        public void Log(WebRequest request, string requestBody, WebException exception)
        {
            const string ModuleName = "Web Request Exception";
            string logTitle = string.Concat("Request to ", request.RequestUri.ToString());
            string logContent = string.Join(Environment.NewLine, this.requestFormatter.Format(request, requestBody), this.exceptionFormatter.Format(exception));

            this.logWriter.Write(ModuleName, logTitle, logContent);
        }
    }
}