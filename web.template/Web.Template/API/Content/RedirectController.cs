namespace Web.Template.API.Content
{
    using System.Collections.Generic;
    using System.Web.Http;
    using Application.Interfaces.Services;
    using Application.Interfaces.Site;
    using Application.Site;

    /// <summary>
    /// Controller for managing redirects.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class RedirectController : ApiController
    {
        /// <summary>
        ///     The redirect service
        /// </summary>
        private readonly IRedirectService redirectService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RedirectController" /> class.
        /// </summary>
        /// <param name="redirectService">The redirect service.</param>
        public RedirectController(IRedirectService redirectService)
        {
            this.redirectService = redirectService;
        }

        /// <summary>
        ///     Gets all redirects.
        /// </summary>
        /// <returns> a list of redirects</returns>
        [Route("api/redirect/all")]
        public List<IRedirect> GetAllRedirects()
        {
            return this.redirectService.GetRedirects();
        }

        /// <summary>
        ///     Adds the redirect.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>a redirect return</returns>
        [HttpPut]
        [Route("api/redirect/add")]
        public RedirectReturn AddRedirect([FromBody] RedirectRequestModel model)
        {
            return this.redirectService.AddRedirect(model.Url, model.RedirectUrl, model.SiteName);
        }

        /// <summary>
        ///     Modifies the redirect.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>a redirect return</returns>
        [HttpPost]
        [Route("api/redirect/modify")]
        public RedirectReturn ModifyRedirect([FromBody] RedirectRequestModel model)
        {
            return this.redirectService.ModifyRedirect(model.Url, model.RedirectUrl, model.RedirectId, model.SiteName);
        }

        /// <summary>
        ///     Deletes the redirect.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>a redirect return</returns>
        [HttpDelete]
        [Route("api/redirect/delete")]
        public RedirectReturn DeleteRedirect([FromBody] RedirectRequestModel model)
        {
            return this.redirectService.DeleteRedirect(model.Url, model.RedirectUrl, model.RedirectId, model.SiteName);
        }
    }
}