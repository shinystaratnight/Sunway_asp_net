namespace Web.Booking.API.Content
{
    using System.Collections.Generic;
    using System.Web.Http;

    using Newtonsoft.Json.Linq;

    using SiteBuilder.Domain.Poco.Return;

    using Web.Booking.Models.Application;
    using Web.Template.Application.Interfaces.SiteBuilder;
    using Web.Template.Application.SiteBuilderService.Models;

    using ContentReturn = SiteBuilder.Domain.Poco.Return.ContentReturn;
    using EntitiesReturn = SiteBuilder.Domain.Poco.Return.EntitiesReturn;

    /// <summary>
    /// Sitebuilder controller managing requests to the sitebuilder
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class SiteBuilderController : ApiController
    {
        /// <summary>
        /// The site builder service
        /// </summary>
        private readonly ISiteBuilderService siteBuilderService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteBuilderController"/> class.
        /// </summary>
        /// <param name="siteBuilderService">The site builder service.</param>
        public SiteBuilderController(ISiteBuilderService siteBuilderService)
        {
            this.siteBuilderService = siteBuilderService;
        }

        /// <summary>
        /// Updates the model.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="context">The context.</param>
        /// <returns>The response from the sitebuilder</returns>
        [Route("api/sitebuilder/model/{site}/{entityName}/{context}")]
        [HttpDelete]
        public string DeleteContent(string site, string entityName, string context)
        {
            return this.siteBuilderService.DeleteContent(site, entityName, context);
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="context">The context.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>An entity</returns>
        [Route("api/sitebuilder/entity/{site}/{entityName}/{context}/{mode}")]
        [HttpGet]
        public EntityModel GetEntity(string site, string entityName, string context, string mode)
        {
             return this.siteBuilderService.GetEntity(site, entityName, context, mode);
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="type">The entity type</param>
        /// <returns>An entity</returns>
        [Route("api/sitebuilder/entities/{site}")]
        [HttpGet]
        public EntitiesReturn GetEntities(string site, [FromUri] string type = "")
        {
            return this.siteBuilderService.GetEntities(site, type);
        }

        /// <summary>
        /// Gets the images.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>the images</returns>
        [Route("api/sitebuilder/images/{search?}")]
        [HttpGet]
        public string GetImages(string search = "", [FromUri] int width = 0, [FromUri] int height = 0)
        {
            return this.siteBuilderService.ImageSearch(search, width, height);
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="context">The context.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>A model</returns>
        [Route("api/sitebuilder/model/{site}/{entityName}/{context}/{mode}")]
        [HttpGet]
        public string GetModel(string site, string entityName, string context, string mode)
        {
            ContentDetailsReturn contentDetailsReturn = this.siteBuilderService.GetModel(site, entityName, context, mode);

            if (contentDetailsReturn == null && mode == "draft")
            {
                contentDetailsReturn = this.siteBuilderService.GetModel(site, entityName, context, "live");
            }

            return contentDetailsReturn != null ? contentDetailsReturn.Content : string.Empty;
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <returns>A model</returns>
        [Route("api/sitebuilder/models/{site}/{entityName}")]
        [HttpGet]
        public Dictionary<string, string> GetModels(string site, string entityName)
        {
            var models = new Dictionary<string, string>();
            ContentReturn contentReturn = this.siteBuilderService.GetModels(site, entityName);
            foreach (var context in contentReturn.Contexts)
            {
                models.Add(context.Name, context.ContentValue);
            }
            return models;
        }

        /// <summary>
        /// Gets the contexts.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <returns>A List of contexts</returns>
        [Route("api/sitebuilder/{site}/{entityName}/contexts")]
        [HttpGet]
        public List<string> GetContexts(string site, string entityName)
        {
            var contexts = new List<string>();
            ContentReturn contentReturn = this.siteBuilderService.GetModels(site, entityName);
            foreach (var context in contentReturn.Contexts)
            {
                contexts.Add(context.Name);
            }
            return contexts;
        }


        /// <summary>
        /// Updates the model.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="context">The context.</param>
        /// <param name="model">The model.</param>
        /// <param name="publish">if set to <c>true</c> [publish].</param>
        /// <returns>The response from the sitebuilder</returns>
        [Route("api/sitebuilder/model/{site}/{entityName}/{context}")]
        [HttpPut]
        public string UpdateModel(string site, string entityName, string context, [FromBody] JToken model, [FromUri] bool publish = false)
        {
            return this.siteBuilderService.UpdateModel(site, entityName, context, model.ToString(), publish);
        }

        /// <summary>
        /// Updates the model.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="context">The context.</param>
        /// <returns>The response from the sitebuilder</returns>
        [Route("api/sitebuilder/model/{site}/{entityName}/{context}/publish")]
        [HttpPut]
        public string PublishModel(string site, string entityName, string context)
        {
            return this.siteBuilderService.PublishDraftContent(site, entityName, context);
        }

        /// <summary>
        /// logs the user in as an admin
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>The login result.</returns>
        [Route("api/sitebuilder/login")]
        [HttpGet]
        public SessionViewModel AdminLogin([FromUri] string username = "", [FromUri] string password = "")
        {
            var session = this.siteBuilderService.UserLogin(username, password);
            var viewModel = new SessionViewModel()
                                {
                                    UserSession = session.UserSession,
                                    Warnings = session.Warnings,
                                    Success = session.UserSession.AdminSession.LoggedIn
                                };


            return viewModel;
        }
    }
}