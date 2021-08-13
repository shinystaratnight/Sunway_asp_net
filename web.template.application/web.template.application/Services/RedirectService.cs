namespace Web.Template.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Domain.Entities.Booking;
    using Domain.Interfaces.Lookup.Repositories.Booking;
    using Interfaces.PageBuilder.Factories;
    using Interfaces.Services;
    using Interfaces.Site;
    using Interfaces.Site.ivcRequests;
    using Interfaces.SiteBuilder;
    using Intuitive;
    using iVectorConnectInterface.Interfaces;
    using Net.IVectorConnect;
    using PageDefinition;
    using Site;
    using SiteBuilderService.Models;
    using ivci = iVectorConnectInterface;

    /// <summary>
    ///     Service used to retrieve pages from the site builder
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Services.IPageService" />
    public class RedirectService : IRedirectService
    {
        /// <summary>
        ///     The add URL redirect request factory
        /// </summary>
        private readonly IAddUrlRedirectRequestFactory addUrlRedirectRequestFactory;

        /// <summary>
        ///     The connect request factory
        /// </summary>
        private readonly IIVectorConnectRequestFactory connectRequestFactory;

        /// <summary>
        ///     The delete URL redirect request factory
        /// </summary>
        private readonly IDeleteUrlRedirectRequestFactory deleteUrlRedirectRequestFactory;

        /// <summary>
        ///     The log writer
        /// </summary>
        private readonly ILogWriter logWriter;

        /// <summary>
        ///     The modify URL redirect request factory
        /// </summary>
        private readonly IModifyUrlRedirectRequestFactory modifyUrlRedirectRequestFactory;

        /// <summary>
        ///     The page service
        /// </summary>
        private readonly IPageService pageService;

        /// <summary>
        ///     The site builder service
        /// </summary>
        private readonly ISiteBuilderService siteBuilderService;

        /// <summary>
        ///     The url301 redirect repository
        /// </summary>
        private readonly IURL301RedirectRepository url301RedirectRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RedirectService" /> class.
        /// </summary>
        /// <param name="logWriter">The log writer.</param>
        /// <param name="url301RedirectRepository">The url301 redirect repository.</param>
        /// <param name="addUrlRedirectRequestFactory">The add URL redirect request factory.</param>
        /// <param name="connectRequestFactory">The connect request factory.</param>
        /// <param name="modifyUrlRedirectRequestFactory">The modify URL redirect request factory.</param>
        /// <param name="deleteUrlRedirectRequestFactory">The delete URL redirect request factory.</param>
        /// <param name="siteBuilderService">The site builder service.</param>
        /// <param name="pageService">The page service.</param>
        public RedirectService(
            ILogWriter logWriter,
            IURL301RedirectRepository url301RedirectRepository,
            IAddUrlRedirectRequestFactory addUrlRedirectRequestFactory,
            IIVectorConnectRequestFactory connectRequestFactory,
            IModifyUrlRedirectRequestFactory modifyUrlRedirectRequestFactory,
            IDeleteUrlRedirectRequestFactory deleteUrlRedirectRequestFactory,
            ISiteBuilderService siteBuilderService, 
            IPageService pageService)
        {
            this.logWriter = logWriter;
            this.url301RedirectRepository = url301RedirectRepository;
            this.addUrlRedirectRequestFactory = addUrlRedirectRequestFactory;
            this.connectRequestFactory = connectRequestFactory;
            this.modifyUrlRedirectRequestFactory = modifyUrlRedirectRequestFactory;
            this.deleteUrlRedirectRequestFactory = deleteUrlRedirectRequestFactory;
            this.siteBuilderService = siteBuilderService;
            this.pageService = pageService;
        }

        /// <summary>
        ///     Gets the page meta.
        /// </summary>
        /// <returns>
        ///     an object containing the page meta information
        /// </returns>
        public List<IRedirect> GetRedirects()
        {
            var redirects = new List<IRedirect>();

            try
            {
                IEnumerable<URL301Redirect> url301s = this.url301RedirectRepository.GetAll();

                foreach (URL301Redirect url301Redirect in url301s)
                {
                    redirects.Add(new Redirect { RedirectUrl = url301Redirect.RedirectURL, Url = url301Redirect.URL, StatusCode = 301, RedirectId = url301Redirect.Id });  
                }
            }
            catch (Exception ex)
            {
                this.logWriter.Write("RedirectService", "Error getting redirects", ex.ToString());
            }

            return redirects;
        }

        /// <summary>
        ///     Adds the redirect.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <param name="site">The site.</param>
        /// <returns>
        ///     whether adding the redirect was successful or not.
        /// </returns>
        public RedirectReturn AddRedirect(string url, string redirectUrl, string site)
        {
            var redirectReturn = new RedirectReturn();
            try
            {
                List<ContextBridgeRequest> redirectcontexts = this.GetContextsFromUrls(url, redirectUrl, redirectReturn);

                if (redirectReturn.Success)
                {
                    foreach (ContextBridgeRequest contextBridgeModel in redirectcontexts)
                    {
                        this.siteBuilderService.AddContextBridge(contextBridgeModel, site);
                    }
                }

                if (redirectReturn.Success)
                {
                    this.SendAddConnectRequest(url, redirectUrl, redirectReturn);
                }

                if (!redirectReturn.Success)
                {
                    this.logWriter.Write("RedirectService", "Error adding redirects", Serializer.Serialize(redirectReturn).OuterXml);
                }
            }
            catch (Exception ex)
            {
                this.logWriter.Write("RedirectService", "Error adding redirects", ex.ToString());
            }

            return redirectReturn;
        }

        /// <summary>
        /// Adds the redirect.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <param name="redirectId">The redirect identifier.</param>
        /// <param name="site">The site.</param>
        /// <returns>
        /// Whether or not adding the redirect was successful.
        /// </returns>
        public RedirectReturn ModifyRedirect(string url, string redirectUrl, int redirectId, string site)
        {
            var redirectReturn = new RedirectReturn();
            try
            {
                List<ContextBridgeRequest> contexts = this.GetContextsFromUrls(url, redirectUrl, redirectReturn);

                foreach (ContextBridgeRequest bridgeModel in contexts)
                {
                    this.siteBuilderService.ModifyContextBridge(bridgeModel, site);
                }

                if (redirectReturn.Success)
                {
                    iVectorConnectRequest requestBody = this.modifyUrlRedirectRequestFactory.Create(url, redirectUrl, redirectId);

                    List<string> warnings = requestBody.Validate();
                    if (warnings.Count == 0)
                    {
                        IIVectorConnectRequest ivcRequest = this.connectRequestFactory.Create(requestBody, HttpContext.Current);
                        var response = ivcRequest.Go<ivci.ModifyURLRedirectResponse>();

                        redirectReturn.Success = response.ReturnStatus.Success;
                        this.url301RedirectRepository.GetAll(clearcache: true);
                    }
                }

                if (!redirectReturn.Success)
                {
                    this.logWriter.Write("RedirectService", "Error modifying redirects", Serializer.Serialize(redirectReturn).OuterXml);
                }
            }
            catch (Exception ex)
            {
                this.logWriter.Write("RedirectService", "Error modifying redirects", ex.ToString());
            }

            return redirectReturn;
        }

        /// <summary>
        ///     Adds the redirect.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <param name="redirectId">The redirect identifier.</param>
        /// <param name="site">The site.</param>
        /// <returns>
        ///     Whether or not adding the redirect was successful.
        /// </returns>
        public RedirectReturn DeleteRedirect(string url, string redirectUrl, int redirectId, string site)
        {
            var redirectReturn = new RedirectReturn();
            try
            {
                List<ContextBridgeRequest> contexts = this.GetContextsFromUrls(url, redirectUrl, redirectReturn);

                foreach (ContextBridgeRequest bridgeModel in contexts)
                {
                    this.siteBuilderService.DeleteContextBridge(bridgeModel, site);
                }

                iVectorConnectRequest requestBody = this.deleteUrlRedirectRequestFactory.Create(redirectId);

                List<string> warnings = requestBody.Validate();
                if (warnings.Count == 0)
                {
                    IIVectorConnectRequest ivcRequest = this.connectRequestFactory.Create(requestBody, HttpContext.Current);
                    var response = ivcRequest.Go<ivci.DeleteURLRedirectResponse>();

                    redirectReturn.Success = response.ReturnStatus.Success;
                    this.url301RedirectRepository.GetAll(clearcache: true);
                }

                if (!redirectReturn.Success)
                {
                    this.logWriter.Write("RedirectService", "Error deleting redirects", Serializer.Serialize(redirectReturn).OuterXml);
                }
            }
            catch (Exception ex)
            {
                this.logWriter.Write("RedirectService", "Error deleting redirects", ex.ToString());
            }

            return redirectReturn;
        }

        /// <summary>
        /// Gets the contexts from url.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <param name="redirectReturn">The redirect return.</param>
        /// <returns>
        /// a list of context bridge requests for the given url
        /// </returns>
        private List<ContextBridgeRequest> GetContextsFromUrls(string url, string redirectUrl, RedirectReturn redirectReturn)
        {
            var redirectcontexts = new List<ContextBridgeRequest>();
            string trimmedUrl = url;
            string trimmedRedirectUrl = redirectUrl;

            trimmedUrl = this.RemoveLeadingSlash(trimmedUrl);
            trimmedRedirectUrl = this.RemoveLeadingSlash(trimmedRedirectUrl);

            Page page = this.GetPage(trimmedUrl, trimmedRedirectUrl, redirectReturn);

            if (redirectReturn.Success)
            {
                string[] urlParts = trimmedUrl.Split('/');
                string[] redirectUrlParts = trimmedRedirectUrl.Split('/');

                this.ValidateEntityInformation(urlParts, redirectUrlParts, redirectReturn, page);

                if (redirectReturn.Success)
                {
                    var urlEntityInfos = new List<PageEntityInformation>();
                    var redirectUrlEntityInfos = new List<PageEntityInformation>();
                    var counter = 0;

                    foreach (PageEntityInformation entityInfo in page.EntityInformations.Where(ef => !ef.Hide))
                    {
                        urlEntityInfos.Add(this.SetupEntityInfo(entityInfo, urlParts[counter]));
                        redirectUrlEntityInfos.Add(this.SetupEntityInfo(entityInfo, redirectUrlParts[counter]));
                        counter++;
                    }

                    foreach (Section section in page.Sections)
                    {
                        foreach (Widget widget in section.Widgets.Where(w => w.EntitySpecific))
                        {
                            redirectcontexts.Add(this.SetupRedirectContext(widget, page, urlEntityInfos, redirectUrlEntityInfos));
                        }
                    }
                }
            }

            return redirectcontexts;
        }

        /// <summary>
        /// Sends the add connect request.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <param name="redirectReturn">The redirect return.</param>
        private void SendAddConnectRequest(string url, string redirectUrl, RedirectReturn redirectReturn)
        {
            iVectorConnectRequest requestBody = this.addUrlRedirectRequestFactory.Create(url, redirectUrl);

            List<string> warnings = requestBody.Validate();
            if (warnings.Count == 0)
            {
                IIVectorConnectRequest ivcRequest = this.connectRequestFactory.Create(requestBody, HttpContext.Current);
                var response = ivcRequest.Go<ivci.AddURLRedirectResponse>();

                redirectReturn.Success = response.ReturnStatus.Success;
                this.url301RedirectRepository.GetAll(clearcache: true);
            }
        }

        /// <summary>
        /// Setups the redirect context.
        /// </summary>
        /// <param name="widget">The widget.</param>
        /// <param name="page">The page.</param>
        /// <param name="urlEntityInfos">The URL entity info.</param>
        /// <param name="redirectUrlEntityInfos">The redirect URL entity info.</param>
        /// <returns>the context bridge request</returns>
        private ContextBridgeRequest SetupRedirectContext(Widget widget, Page page, List<PageEntityInformation> urlEntityInfos, List<PageEntityInformation> redirectUrlEntityInfos)
        {
            widget.Page = new PageViewModel
            {
                EntityType = page.EntityType
            };

            widget.Page.EntityInformations = urlEntityInfos;
            string oldContext = widget.GetContext();

            widget.Page.EntityInformations = redirectUrlEntityInfos;
            string currentContext = widget.GetContext();

            return new ContextBridgeRequest { CurrentContext = currentContext, OldContext = oldContext, Entity = widget.Name };
        }

        /// <summary>
        /// Setups the entity information.
        /// </summary>
        /// <param name="entityInfo">The entity information.</param>
        /// <param name="urlValue">The URL value.</param>
        /// <returns>
        /// a page entity information
        /// </returns>
        private PageEntityInformation SetupEntityInfo(PageEntityInformation entityInfo, string urlValue)
        {
            return new PageEntityInformation
            {
                Hide = false,
                Id = entityInfo.Id,
                Name = entityInfo.Name,
                Value = entityInfo.Value,
                UrlSafeValue = urlValue
            };
        }

        /// <summary>
        /// Removes the leading slash.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>the url with the first slash removed</returns>
        private string RemoveLeadingSlash(string url)
        {
            if (url.StartsWith("/"))
            {
                url = url.TrimStart('/');
            }

            return url;
        }

        /// <summary>
        /// Validates the entity information.
        /// </summary>
        /// <param name="urlParts">The URL parts.</param>
        /// <param name="redirectUrlParts">The redirect URL parts.</param>
        /// <param name="redirectReturn">The redirect return.</param>
        /// <param name="page">The page.</param>
        private void ValidateEntityInformation(string[] urlParts, string[] redirectUrlParts, RedirectReturn redirectReturn, Page page)
        {
            if (urlParts.Length != redirectUrlParts.Length)
            {
                redirectReturn.WarningList.Add("The URLs do not contain the same number of forward slashes");
                redirectReturn.Success = false;
            }
            else if (page.EntityInformations.Count(ef => !ef.Hide) != urlParts.Length)
            {
                redirectReturn.WarningList.Add("The URLs do not contain the same number of forward slashes as the page expects");
                redirectReturn.Success = false;
            }
        }

        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <param name="trimmedUrl">The trimmed URL.</param>
        /// <param name="trimmedRedirectUrl">The trimmed redirect URL.</param>
        /// <param name="redirectReturn">The redirect return.</param>
        /// <returns>the page</returns>
        private Page GetPage(string trimmedUrl, string trimmedRedirectUrl, RedirectReturn redirectReturn)
        {
            Page page = this.pageService.GetPageByURL(trimmedUrl);
            if (page == null || page.Name == "404")
            {
                page = this.pageService.GetPageByURL(trimmedRedirectUrl);

                if (page == null || page.Name == "404")
                {
                    redirectReturn.WarningList.Add("No page with either the current or the previous URLs exist");
                    redirectReturn.Success = false;
                }
            }

            return page;
        }
    }
}