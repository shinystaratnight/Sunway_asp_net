namespace Web.Template.Application.SiteBuilderService
{
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;

    /// <summary>
    /// Class SiteBuilderRequest.
    /// </summary>
    public class SiteBuilderRequest : ISiteBuilderRequest
    {
        /// <summary>
        /// Simple web request wrapper, should probably centralize.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="url">The URL.</param>
        /// <param name="body">The body.</param>
        /// <param name="headers">The headers.</param>
        /// <returns>The response body as a string</returns>
        public string Send(string method, string url, string body = null, Dictionary<string, string> headers = null)
        {
            var request = WebRequest.Create(url);
            request.Method = method;
            request.Timeout = 30000;

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            if (!string.IsNullOrEmpty(body))
            {
                byte[] buffer = new UTF8Encoding().GetBytes(body);

                // Set the content length
                request.ContentLength = buffer.Length;
                request.ContentType = "application/json";

                // Write to request
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(buffer, 0, buffer.Length);
                requestStream.Close();
            }
            else
            {
                request.ContentLength = 0;
            }

            var responsebody = this.ReadResponse(request);

            return responsebody;
        }

        /// <summary>
        /// Reads the response.
        /// </summary>
        /// <param name="webRequest">The web request.</param>
        /// <returns>The response</returns>
        private string ReadResponse(WebRequest webRequest)
        {
            var responseString = string.Empty;
            try
            {
                // Send request to get the response
                var response = webRequest.GetResponse();

                Stream stream = response.GetResponseStream();
                if (stream != null)
                {
                    // get the response body
                    var responseStreamReader = new StreamReader(stream);
                    responseString = responseStreamReader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                using (WebResponse response = ex.Response)
                {
                    Stream stream = response.GetResponseStream();
                    if (stream != null)
                    {
                        var responseStreamReader = new StreamReader(stream);
                        responseString = responseStreamReader.ReadToEnd();
                    }
                }
            }

            return responseString;
        }
    }
}