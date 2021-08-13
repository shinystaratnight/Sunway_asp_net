namespace Web.Template.Data.Connect
{
    using System;
    using System.IO;
    using System.Net;
    using System.Web;
    using System.Xml;

    using Intuitive;

    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Interfaces.User;

    /// <summary>
    /// Using intuitive async cache to get a lookup
    /// </summary>
    /// <seealso cref="Web.Template.Data.Connect.IAsyncLookup" />
    public class AsyncLookup : IAsyncLookup
    {
        /// <summary>
        /// The site service
        /// </summary>
        private readonly ISiteService siteService;

        /// <summary>
        ///     The cache lock object
        /// </summary>
        private static readonly object CacheLockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncLookup" /> class.
        /// </summary>
        /// <param name="siteService">The site service.</param>
        public AsyncLookup(ISiteService siteService)
        {
            this.siteService = siteService;
        }

        /// <summary>
        /// Gets the asynchronous CMS lookup.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>XmlDocument.</returns>
        public XmlDocument GetAsyncCMSLookup(string objectType, int id)
        {
            ISite site = this.siteService.GetSite(HttpContext.Current);
            var key = $"lookupxml_{objectType.ToLower()}_{id}_{site.Name.ToLower()}";
            var cachekey = Intuitive.AsyncCache.Controller<XmlDocument>.GenerateKey(key);

            XmlDocument xml = Intuitive.AsyncCache.Controller<XmlDocument>.GetCache(
                cachekey, 
                600, 
                () =>
                    {
                        var lookupXML = new XmlDocument();
                        try
                        {
                            string url = $"{site.IvectorConnectBaseUrl}cms/{objectType}/{id}";
                            lookupXML = this.UrlToXml(url, 0);
                        }
                        catch (Exception ex)
                        {
                        }

                        return lookupXML;
                    });

            return xml;
        }

        /// <summary>
        /// Gets the asynchronous lookup.
        /// </summary>
        /// <param name="lookup">The lookup.</param>
        /// <param name="clearCache">if set to <c>true</c> [clear cache].</param>
        /// <returns>
        /// An xml document.
        /// </returns>
        public XmlDocument GetAsyncLookup(string lookup, bool clearCache = false)
        {
            var displayLanguageId = 0;
            try
            {
                if (HttpContext.Current != null)
                {
                    var user = (IUserSession)HttpContext.Current.Session["userSession"];
                    displayLanguageId = user.SelectedLanguage.Id;
                }
            }
            catch (Exception ex)
            {
                displayLanguageId = 0;
                FileFunctions.AddLogEntry("Async Lookup", "error gettin language", ex.Message);
            }

            ISite site = this.siteService.GetSite(HttpContext.Current);
            var key = $"lookupxml_{lookup.ToLower()}_{site.Name.ToLower()}_{displayLanguageId}";

            var lookupXML = new XmlDocument();

            if (clearCache)
            {
                lookupXML = updateCache(lookup, site, displayLanguageId, lookupXML, key);
            }
            else
            {
                try
                {
                    var result = HttpRuntime.Cache[key] as XmlDocument;
                    if (result == null)
                        lock (CacheLockObject)
                        {
                            result = HttpRuntime.Cache[key] as XmlDocument;
                            if (result == null)
                            {
                                lookupXML = GetLookupsXML(lookup, site, displayLanguageId);

                                HttpRuntime.Cache.Insert(key,
                                    result,
                                    null,
                                    DateTime.Now.AddHours(12),
                                    TimeSpan.Zero);
                            }
                               
                        }
                }
                catch (Exception ex)
                {
                }                   
            }

            return lookupXML;
        }

        private XmlDocument GetLookupsXML(string lookup, ISite site, int displayLanguageId)
        {
            XmlDocument lookupXML;
            var url = "{0}lookups/lookups.ashx?files={1}&login={2}&password={3}&languageid={4}";
            url = string.Format(
                url,
                site.IvectorConnectBaseUrl,
                lookup,
                site.IvectorConnectUsername,
                site.IvectorConnectPassword,
                displayLanguageId);

            lookupXML = this.UrlToXml(url, 0);
            return lookupXML;
        }

        /// <summary>
        /// Updates the cache.
        /// </summary>
        /// <param name="lookup">The lookup.</param>
        /// <param name="site">The site.</param>
        /// <param name="displayLanguageId">The display language identifier.</param>
        /// <param name="xml">The XML.</param>
        /// <param name="key">The key.</param>
        /// <returns>The xml document</returns>
        private XmlDocument updateCache(string lookup, ISite site, int displayLanguageId, XmlDocument xml, string key)
        {
            try
            {
                var url = "{0}lookups/lookups.ashx?files={1}&login={2}&password={3}&languageid={4}";
                url = string.Format(
                    url,
                    site.IvectorConnectBaseUrl,
                    lookup,
                    site.IvectorConnectUsername,
                    site.IvectorConnectPassword,
                    displayLanguageId);

                xml = this.UrlToXml(url, 0);
                if (HttpContext.Current != null && HttpContext.Current.Cache[key] is XmlDocument)
                {
                    HttpContext.Current.Cache[key] = xml;
                }
                else
                {
                    if (HttpContext.Current != null) HttpContext.Current.Cache.Insert(key, xml);
                }
            }
            catch (Exception ex)
            {
            }

            return xml;
        }

        /// <summary>
        /// URLs to XML.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="cacheMinutes">The cache minutes.</param>
        /// <returns>
        /// an xml document
        /// </returns>
        public XmlDocument UrlToXml(string url, int cacheMinutes)
        {
            var xml = new XmlDocument();

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

            return xml;
        }

        /// <summary>
        /// Sends the connect request.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="cacheMinutes">The cache minutes.</param>
        /// <returns>An XML document</returns>
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
    }
}