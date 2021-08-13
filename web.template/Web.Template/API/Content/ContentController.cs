namespace Web.Template.API.Content
{
    using System.Web.Http;

    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Services;

    /// <summary>
    ///     The API to CMS Content to be called from a front end widget when additional content is needed
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class ContentController : ApiController
    {
        /// <summary>
        ///     The content service
        /// </summary>
        private readonly IContentService contentService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContentController" /> class.
        /// </summary>
        /// <param name="contentService">The content service.</param>
        public ContentController(IContentService contentService)
        {
            this.contentService = contentService;
        }

        /// <summary>
        /// Gets the CMS content by object.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>A content model</returns>
        [Route("api/cms/{objectType}/{id}")]
        [HttpGet]
        public IContentModel GetCMSContentByObjectType(string objectType, int id)
        {
            return this.contentService.GetCMSContentForObjectType(objectType, id);
        }

        /// <summary>
        /// Gets the content associated with the provided widget and context.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="widget">The widget.</param>
        /// <param name="context">The context.</param>
        /// <returns>A content model</returns>
        [Route("api/content/{site}/{widget}/{context}")]
        [HttpGet]
        public IContentModel GetContentByContext(string site, string widget, string context)
        {
            return this.contentService.GetContentForContext(site, widget, context);
        }
    }
}