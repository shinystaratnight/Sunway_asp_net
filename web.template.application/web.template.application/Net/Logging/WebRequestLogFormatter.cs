namespace Web.Template.Application.Net.Logging
{
    using System;
    using System.Net;
    using System.Text;

    /// <summary>
    ///     Creates display formats for the WebRequest class
    /// </summary>
    public class WebRequestLogFormatter
    {
        /// <summary>
        ///     Formats the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="requestBody">The request body.</param>
        /// <returns>
        ///     A formatted string representing the request.
        /// </returns>
        /// <remarks>The request body is only required as the body can't be read from the stream after it has been written.</remarks>
        public virtual string Format(WebRequest request, string requestBody)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var output = new StringBuilder();

            this.WriteUrl(request, output);
            this.WriteHeaders(request, output);
            this.WriteBody(requestBody, output);

            return output.ToString();
        }

        /// <summary>
        ///     Writes the body.
        /// </summary>
        /// <param name="requestBody">The request body.</param>
        /// <param name="output">The output that the Url information is written to.</param>
        private void WriteBody(string requestBody, StringBuilder output)
        {
            output.AppendLine("Body:");
            output.AppendLine(requestBody);
        }

        /// <summary>
        ///     Writes the headers.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="output">The output that the Url information is written to.</param>
        private void WriteHeaders(WebRequest request, StringBuilder output)
        {
            output.AppendLine("Headers:");

            foreach (string headerKey in request.Headers.AllKeys)
            {
                output.AppendLine(string.Concat(headerKey, " - ", request.Headers[headerKey]));
            }
        }

        /// <summary>
        ///     Writes the URL to the output.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="output">The output that the Url information is written to.</param>
        private void WriteUrl(WebRequest request, StringBuilder output)
        {
            output.AppendLine(string.Concat("URL: ", request.RequestUri.AbsoluteUri));
        }
    }
}