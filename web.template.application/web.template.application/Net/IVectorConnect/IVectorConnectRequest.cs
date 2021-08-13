namespace Web.Template.Application.Net.IVectorConnect
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;

    using Intuitive;

    using iVectorConnectInterface;
    using iVectorConnectInterface.Interfaces;

    using Newtonsoft.Json;

    using Web.Template.Application.Configuration;
    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Interfaces.Logging;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Support;

    /// <summary>
    ///     Manages a request to ivector connect.
    /// </summary>
    public enum FormatType
    {
        /// <summary>
        ///     JSON format
        /// </summary>
        JSON, 

        /// <summary>
        ///     XML format
        /// </summary>
        XML
    }

    /// <summary>
    ///     An ivector connect request
    /// </summary>
    public class IVectorConnectRequest : IIVectorConnectRequest
    {
        /// <summary>
        /// The site
        /// </summary>
        private readonly ISite site;

        /// <summary>
        ///     The request
        /// </summary>
        private readonly WebRequest request;

        /// <summary>
        ///     The web request logger
        /// </summary>
        private readonly IWebRequestLogger webRequestLogger;

        /// <summary>
        /// The cancellation token
        /// </summary>
        private CancellationToken cancellationToken;

        /// <summary>
        ///     The request body
        /// </summary>
        private string requestBody;

        /// <summary>
        ///     The response
        /// </summary>
        private WebResponse response;

        /// <summary>
        /// Initializes a new instance of the <see cref="IVectorConnectRequest" /> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="request">The request.</param>
        /// <param name="formatType">Type of the format.</param>
        /// <param name="webRequestLogger">The web request logger.</param>
        /// <param name="siteService">The site service.</param>
        /// <param name="context">The context.</param>
        public IVectorConnectRequest(string url, string method, iVectorConnectRequest request, FormatType formatType, IWebRequestLogger webRequestLogger, ISiteService siteService, HttpContext context)
            : this(url, method, (object)request, formatType, webRequestLogger, siteService, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IVectorConnectRequest" /> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="request">The request.</param>
        /// <param name="formatType">Type of the format.</param>
        /// <param name="webRequestLogger">The web request logger.</param>
        /// <param name="siteService">The site service.</param>
        /// <param name="token">The token.</param>
        public IVectorConnectRequest(string url, string method, object request, FormatType formatType, IWebRequestLogger webRequestLogger, ISiteService siteService, CancellationToken token)
        {
            this.site = siteService.GetSite(HttpContext.Current);
            this.request = WebRequest.Create(this.site.IvectorConnectBaseUrl + url);
            this.request.Method = method;
            this.SetRequestHeaders(formatType);
            this.SetRequestBody(this.SerializeRequest(request, formatType));
            this.webRequestLogger = webRequestLogger;
            this.cancellationToken = token;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IVectorConnectRequest" /> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="request">The request.</param>
        /// <param name="formatType">Type of the format.</param>
        /// <param name="webRequestLogger">The web request logger.</param>
        /// <param name="siteService">The site service.</param>
        /// <param name="context">The context.</param>
        private IVectorConnectRequest(string url, string method, object request, FormatType formatType, IWebRequestLogger webRequestLogger, ISiteService siteService, HttpContext context)
        {
            this.site = siteService.GetSite(context);
            this.request = WebRequest.Create(this.site.IvectorConnectBaseUrl + url);
            this.request.Method = method;
            this.SetRequestHeaders(formatType);
            this.SetRequestBody(this.SerializeRequest(request, formatType));
            this.webRequestLogger = webRequestLogger;
        }

        /// <summary>
        /// Initiates the request to ivectorconnect.
        /// </summary>
        /// <typeparam name="T">The response class</typeparam>
        /// <param name="logRequest">if set to <c>true</c> [log request].</param>
        /// <returns>
        /// Return object of type T
        /// </returns>
        public T Go<T>(bool logRequest = false) where T : class, iVectorConnectResponse, new()
        {
            using (this.cancellationToken.Register(() => this.request.Abort(), useSynchronizationContext: false))
            {
                try
                {
                    this.response = this.request.GetResponse();
                    string responsebody = this.ReadResponse();

#if DEBUG
                    logRequest = true;
#endif
                    if (logRequest)
                    {
                        this.LogRequest<T>(responsebody);
                    }

                    var responseFormat = this.GetFormatTypeFromResponseContentType<T>();

                    return this.DeserializeResponse<T>(responsebody, responseFormat);
                }
                catch (WebException ex)
                {
                    this.webRequestLogger.Log(this.request, this.requestBody, ex);

                    return default(T);
                }
            }
        }

        /// <summary>
        /// Gets the type of the format type from response content.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private FormatType GetFormatTypeFromResponseContentType<T>() where T : class, iVectorConnectResponse, new()
        {
            FormatType responseFormat = FormatType.XML;

            switch (this.response.ContentType)
            {
                case ("application/xml"):
                case ("application/xml; charset=utf-8"):
                    responseFormat = FormatType.XML;
                    break;
                case ("application/json"):
                case ("application/json; charset=utf-8"):
                    responseFormat = FormatType.JSON;
                    break;
            }
            return responseFormat;
        }

        /// <summary>
        /// Initiates the request to ivectorconnect.
        /// </summary>
        /// <typeparam name="T">The response class</typeparam>
        /// <param name="logRequest">if set to <c>true</c> [log request].</param>
        /// <returns>
        /// Return object of type T
        /// </returns>
        public async Task<T> GoAsync<T>(bool logRequest = false) where T : class, iVectorConnectResponse, new()
        {
            using (this.cancellationToken.Register(() => this.request.Abort(), useSynchronizationContext: false))
            {
                try
                {
                    this.response = await this.request.GetResponseAsync();
                    string responsebody = this.ReadResponse();

#if DEBUG
                    logRequest = true;
#endif
                    if (logRequest)
                    {
                        this.LogRequest<T>(responsebody);
                    }

                    return this.DeserializeResponse<T>(responsebody, FormatType.XML);
                }
                catch (WebException ex)
                {
                    this.webRequestLogger.Log(this.request, this.requestBody, ex);

                    return default(T);
                }
            }
        }

        /// <summary>
        ///     Deserializes the response.
        /// </summary>
        /// <typeparam name="T">response object type</typeparam>
        /// <param name="responseBody">The response body.</param>
        /// <param name="formatType">Type of the format.</param>
        /// <returns>
        ///     Response object T
        /// </returns>
        private T DeserializeResponse<T>(string responseBody, FormatType formatType) where T : class, iVectorConnectResponse, new()
        {
            // deserialize depending on format type
            try
            {
                switch (formatType)
                {
                    case FormatType.JSON:
                        return JsonConvert.DeserializeObject<T>(responseBody);

                    case FormatType.XML:
                        return Serializer.DeSerialize<T>(responseBody, false);

                    default:
                        return null;
                }
            }
            catch (InvalidOperationException)
            {
                var obj = new T();

                switch (formatType)
                {
                    case FormatType.JSON:
                        obj.ReturnStatus = JsonConvert.DeserializeObject<ResponseStatus>(responseBody).ReturnStatus;
                        break;

                    case FormatType.XML:

                        obj.ReturnStatus = Serializer.DeSerialize<ResponseStatus>(responseBody, false).ReturnStatus;
                        break;
                }

                return obj;
            }
        }

        /// <summary>
        /// Logs the request.
        /// </summary>
        /// <typeparam name="T">the type of the request</typeparam>
        /// <param name="responsebody">The response body.</param>
        private void LogRequest<T>(string responsebody) where T : class, iVectorConnectResponse, new()
        {
            var logpath = $"iVectorConnect/{typeof(T).ToString()}".Replace("Response", string.Empty);
            this.webRequestLogger.Log(this.request, this.requestBody, logpath, this.response, responsebody);
        }

        /// <summary>
        ///     Reads the response.
        /// </summary>
        /// <returns>b
        ///     String of the response
        /// </returns>
        private string ReadResponse()
        {
            // Send request to get the response
            this.response = this.request.GetResponse();

            // Return the response body
            var responseStreamReader = new StreamReader(this.response.GetResponseStream());
            return responseStreamReader.ReadToEnd();
        }

        /// <summary>
        ///     Serializes the request.
        /// </summary>
        /// <param name="requestObject">The request object.</param>
        /// <param name="formatType">Type of the format.</param>
        /// <returns>
        ///     String of the request in chosen format
        /// </returns>
        private string SerializeRequest(object requestObject, FormatType formatType)
        {
            switch (formatType)
            {
                case FormatType.JSON:
                    return JsonConvert.SerializeObject(requestObject);

                case FormatType.XML:
                    return Serializer.Serialize(requestObject).InnerXml;

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        ///     Sets the request body.
        /// </summary>
        /// <param name="bodyText">The body text.</param>
        private void SetRequestBody(string bodyText)
        {
            this.requestBody = bodyText;

            byte[] buffer = new UTF8Encoding().GetBytes(this.requestBody);

            // Set the content length
            this.request.ContentLength = buffer.Length;

            // Write to request
            Stream requestStream = this.request.GetRequestStream();
            requestStream.Write(buffer, 0, buffer.Length);
            requestStream.Close();
        }

        /// <summary>
        ///     Sets the request headers.
        /// </summary>
        /// <param name="formatType">Type of the format.</param>
        private void SetRequestHeaders(FormatType formatType)
        {
            // Set authorization header
            string auth = $"{this.site.IvectorConnectUsername}|{this.site.IvectorConnectPassword}";

            //// string _enc = Convert.ToBase64String(Encoding.ASCII.GetBytes(_auth));
            //// string _cred = string.Format("{0} {1}", "Basic", _enc);

            //// this._request.Headers[HttpRequestHeader.Authorization] = _cred;
            this.request.Headers[HttpRequestHeader.Authorization] = auth;

            // Set the content type
            switch (formatType)
            {
                case FormatType.JSON:
                    this.request.ContentType = "application/json";
                    break;

                case FormatType.XML:
                    this.request.ContentType = "application/xml";
                    break;
            }
        }
    }
}