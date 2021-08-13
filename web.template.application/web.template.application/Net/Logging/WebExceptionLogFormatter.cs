namespace Web.Template.Application.Net.Logging
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;

    /// <summary>
    ///     Creates display formats for the WebException class
    /// </summary>
    public class WebExceptionLogFormatter
    {
        /// <summary>
        ///     Formats the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>A formatted string representing the exception.</returns>
        public virtual string Format(WebException exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            var output = new StringBuilder();

            output.AppendLine(string.Concat("Status Code - ", exception.Status));
            output.AppendLine(string.Concat("Message - ", exception.Message));

            if (exception.Response != null)
            {
                using (var reader = new StreamReader(exception.Response.GetResponseStream()))
                {
                    output.AppendLine(string.Concat("Response - ", reader.ReadToEnd()));
                }
            }

            return output.ToString();
        }
    }
}