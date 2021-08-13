namespace Web.Template.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Xml;

    using Intuitive;
    using Intuitive.Net;

    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Interfaces.Services;

    /// <summary>
    /// Class responsible for talking to connect for custom queries
    /// </summary>
    /// <seealso cref="Web.Template.Application.Services.ICustomQuery" />
    public class CustomQuery : ICustomQuery
    {
        /// <summary>
        /// The log writer
        /// </summary>
        private readonly ILogWriter logWriter;

        /// <summary>
        /// The site service
        /// </summary>
        private readonly ISiteService siteService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomQuery" /> class.
        /// </summary>
        /// <param name="logWriter">The class responsible for writing logs</param>
        /// <param name="siteService">The site service.</param>
        public CustomQuery(ILogWriter logWriter, ISiteService siteService)
        {
            this.logWriter = logWriter;
            this.siteService = siteService;
        }

        /// <summary>
        /// Gets the custom query XML.
        /// </summary>
        /// <param name="params">The parameters.</param>
        /// <param name="customQueryName">Name of the custom query.</param>
        /// <returns>
        /// an xml doc
        /// </returns>
        public XmlDocument GetCustomQueryXml(List<string> @params, string customQueryName)
        {
            XmlDocument offersXmlDocument = new XmlDocument();

            // Build Url
            ISite site = this.siteService.GetSite(HttpContext.Current);
            string url = site.IvectorConnectBaseUrl + string.Format("customquery.ashx?login={0}&password={1}&query={2}", site.IvectorConnectContentUsername, site.IvectorConnectContentPassword, customQueryName);

            // Add any query string parameters supplied
            if (@params.Count > 0)
            {
                string queryString = string.Empty;
                int paramNumber = 1;
                foreach (var param in @params)
                {
                    queryString += "&param" + paramNumber + "=" + param;
                    paramNumber += 1;
                }

                url += queryString;
            }

            try
            {
                if (HttpContext.Current.Cache[url.ToLower()] != null)
                {
                    offersXmlDocument = (XmlDocument)HttpContext.Current.Cache[url.ToLower()];
                }
                else
                {
                    offersXmlDocument = this.SendCustomQueryRequest(url);
                    if (offersXmlDocument != new XmlDocument())
                    {
                        HttpContext.Current.Cache.Insert(url.ToLower(), offersXmlDocument);
                    }
                }
            }
            catch (Exception exception)
            {
                offersXmlDocument.LoadXml("<Error>" + exception.ToString() + "</error>");
                this.logWriter.Write("iVectorConnect/UrlToXml", "error", offersXmlDocument.InnerXml);
            }

            offersXmlDocument = this.SendCustomQueryRequest(url);

            return offersXmlDocument;
        }

        /// <summary>g
        /// Sends the custom query request.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>an xml document containing the response from connect</returns>
        private XmlDocument SendCustomQueryRequest(string url)
        {
            var offersXmlDocument = new XmlDocument();
            try
            {
                WebRequests.Request request = new WebRequests.Request();
                request.EndPoint = url;
                request.Method = WebRequests.eRequestMethod.GET;
                request.Send();

                offersXmlDocument = request.ResponseXML;
            }
            catch (Exception exception)
            {
                offersXmlDocument.LoadXml("<Error>" + exception.ToString() + "</error>");
                this.logWriter.Write("iVectorConnect/UrlToXml", "error", offersXmlDocument.InnerXml);
            }

            return offersXmlDocument;
        }
    }
}