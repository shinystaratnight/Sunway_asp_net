namespace Web.Template.Application.IVectorConnect
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Web;
    using System.Xml;

    using Intuitive;
    using Intuitive.AsyncCache;

    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Interfaces.Services;

    /// <summary>
    ///     async lookup responsible for calling connects lookups
    /// </summary>
    public class AsyncLookup
    {
        /// <summary>
        ///     The lookup name
        /// </summary>
        private readonly string lookupName;

        /// <summary>
        /// The site service
        /// </summary>
        private readonly ISiteService siteService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncLookup" /> class.
        /// </summary>
        /// <param name="lookupName">Name of the lookup.</param>
        /// <param name="siteService">The site service.</param>
        public AsyncLookup(string lookupName, ISiteService siteService)
        {
            this.lookupName = lookupName;
            this.siteService = siteService;
        }

        /// <summary>
        ///     Gets the lookup.
        /// </summary>
        /// <typeparam name="T">The type of object we want to cast the xml </typeparam>
        /// <returns>
        ///     A list of objects of the type passed in
        /// </returns>
        public List<T> GetLookup<T>() where T : class
        {
            XmlDocument lookupxml = this.GetAsyncLookup();

            return this.XmlToGenericList<T>(lookupxml);
        }

        /// <summary>
        ///     takes an xml document which will contain a serialized generic list, and turns it into a generic list.
        /// </summary>
        /// <typeparam name="T">The type of object we want to cast each xml node to</typeparam>
        /// <param name="xml">The XML.</param>
        /// <returns> a list of the type passed in</returns>
        public List<T> XmlToGenericList<T>(XmlDocument xml) where T : class
        {
            var list = new List<T>();
            try
            {
                if (xml.ChildNodes.Count > 0)
                {
                    XmlNodeList nodeList = xml.ChildNodes[0].ChildNodes[0].ChildNodes;
                    foreach (XmlNode node in nodeList)
                    {
                        var item = Serializer.DeSerialize<T>(node.OuterXml);
                        list.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                FileFunctions.AddLogEntry("iVectorConnect/xmlToGenericList", "Error", ex.ToString());
            }

            return list;
        }

        /// <summary>
        ///     Sends the connect request.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="cacheMinutes">The cache minutes.</param>
        /// <returns>the xml response returned from connect</returns>
        private static XmlDocument SendiVcRequest(string url, int cacheMinutes)
        {
            var xml = new XmlDocument();

            WebRequest request = WebRequest.Create(url);

            WebResponse response = request.GetResponse();

            using (response)
            {
                Stream streamResponse = response.GetResponseStream();
                if (streamResponse != null)
                {
                    var responseStreamReader = new StreamReader(streamResponse);
                    string responseString = responseStreamReader.ReadToEnd();

                    xml.LoadXml(responseString);

                    Functions.AddToCache(url, xml, cacheMinutes);
                }
            }

            return xml;
        }

        /// <summary>
        ///     Takes a url, and queries it to return XML (this is how we get connect lookups)
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="cacheMinutes">The cache minutes.</param>
        /// <returns>an xml document</returns>
        private static XmlDocument UrltoXml(string url, int cacheMinutes)
        {
            var xml = Functions.GetCache<XmlDocument>(url);
            if (xml == null)
            {
                try
                {
                    xml = SendiVcRequest(url, cacheMinutes);
                }
                catch (Exception ex)
                {
                    xml = new XmlDocument();
                    xml.LoadXml($"<Error>{ex.Message}</Error>");
                    FileFunctions.AddLogEntry("iVectorConnect/URLToXML", "Error", xml.InnerXml);
                }
            }

            return xml;
        }

        /// <summary>
        ///     Gets the asynchronous lookup.
        /// </summary>
        /// <returns>an xml document</returns>
        private XmlDocument GetAsyncLookup()
        {
            string key = Controller<XmlDocument>.GenerateKey($"lookupxml_{this.lookupName.ToLower()}");

            Func<XmlDocument> lookupFunc = this.GetLookup;

            XmlDocument xml = Controller<XmlDocument>.GetCache(key, 600, lookupFunc, "^(?!<Lookups).*");

            return xml;
        }

        /// <summary>
        ///     RetrieveResult lookups this instance.
        /// </summary>
        /// <returns>
        ///     An xml document
        /// </returns>
        private XmlDocument GetLookup()
        {
            XmlDocument lookupXml;
            try
            {
                ISite site = this.siteService.GetSite(HttpContext.Current);
                string serviceurl = site.IvectorConnectBaseUrl;
                string login = site.IvectorConnectUsername;
                string password = site.IvectorConnectPassword;

                string url = $"{serviceurl}lookups/lookups.ashx?files={this.lookupName.ToLower()}&login={login}&password={password}";

                lookupXml = UrltoXml(url, 100);
            }
            catch (Exception ex)
            {
                lookupXml = new XmlDocument();
                lookupXml.LoadXml($"<Error>{ex.Message}</Error>");
                FileFunctions.AddLogEntry("iVectorConnect/URLToXML", "Error", lookupXml.InnerXml);
            }

            return lookupXml;
        }
    }
}