namespace Web.Template.Application.PageDefinition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using SiteBuilder.Domain.Poco.Return;

    using Tweetinvi.Core.Extensions;

    using Web.Template.Application.Content;
    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Interfaces.User;
    using Web.Template.Application.PageDefinition.Enums;

    /// <summary>
    /// A class representing a widget, a discreet, reusable control.
    /// </summary>
    public class Widget
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Widget" /> class.
        /// </summary>
        /// <param name="contentService">The content service.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="siteService">The site service.</param>
        public Widget(IContentService contentService, IUserService userService, ISiteService siteService)
        {
            this.ContentService = contentService;
            this.UserService = userService;
            this.SiteService = siteService;
        }

        /// <summary>
        /// Gets or sets the access.
        /// </summary>
        /// <value>
        /// The Access level that a user must have for this widget to appear on a page.
        /// </value>
        public AccessLevel Access { get; set; }

        /// <summary>
        /// Gets the access display.
        /// </summary>
        /// <value>
        /// The access display.
        /// </value>
        public string AccessDisplay => this.Access.ToString();

        /// <summary>
        /// Gets or sets a value indicating whether [client side render].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [client side render]; otherwise, <c>false</c>.
        /// </value>
        public bool ClientSideRender { get; set; }

        /// <summary>
        /// Gets or sets the container namespace.
        /// </summary>
        /// <value>The container namespace.</value>
        public string ContainerNamespace { get; set; }

        /// <summary>
        /// Gets or sets the content JSON.
        /// </summary>
        /// <value>The content JSON.</value>
        public string ContentJSON { get; set; }

        /// <summary>
        /// Gets or sets the content service.
        /// </summary>
        /// <value>The content service.</value>
        public IContentService ContentService { get; set; }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public string Context { get; set; }

        /// <summary>
        /// Gets the content context.
        /// </summary>
        /// <value>
        /// The content context, which will include e.g. entity specific information.
        /// </value>
        public string ContentContext { get; set; }

        /// <summary>
        /// Gets or sets the entities.
        /// </summary>
        /// <value>
        /// The entities.
        /// </value>
        public List<string> Entities { get; set; }

        /// <summary>
        /// Gets or sets the content of the entities.
        /// </summary>
        /// <value>
        /// The content of the entities.
        /// </value>
        public Dictionary<string, List<ContentReturnContext>> EntitiesContent { get; set; }

        /// <summary>
        /// Gets or sets the entity.
        /// </summary>
        /// <value>
        /// The entity.
        /// </value>
        public List<PageDefEntity> EntityList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [entity specific].
        /// </summary>
        /// <value><c>true</c> if [entity specific]; otherwise, <c>false</c>.</value>
        public bool EntitySpecific { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Widget"/> is overbranded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if overbranded; otherwise, <c>false</c>.
        /// </value>
        public bool Overbranded { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The Name of the widget E.g. SearchTool
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Widget"/> is editable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if editable; otherwise, <c>false</c>.
        /// </value>
        public bool NotEditable { get; set; }

        /// <summary>
        /// Gets or sets the page object.
        /// </summary>
        /// <value>
        /// The page object.
        /// </value>
        public PageViewModel Page { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [server side render].
        /// </summary>
        /// <value><c>true</c> if [server side render]; otherwise, <c>false</c>.</value>
        public bool ServerSideRender { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the widget is shared.
        /// </summary>
        /// <value><c>true</c> if shared; otherwise, <c>false</c>.</value>
        public bool Shared { get; set; }

        /// <summary>
        /// Gets or sets the site.
        /// </summary>
        /// <value>The site.</value>
        public ISite Site { get; set; }

        /// <summary>
        /// Gets or sets the site service.
        /// </summary>
        /// <value>The site service.</value>
        public ISiteService SiteService { get; set; }

        /// <summary>
        /// Gets or sets the content of the specific entities.
        /// </summary>
        /// <value>
        /// The content of the specific entities.
        /// </value>
        public Dictionary<string, IContentModel> SpecificEntitiesContent { get; set; }

        /// <summary>
        /// Gets or sets the sub widgets.
        /// </summary>
        /// <value>
        /// A collection of all subwidgets that will be part of this widget.
        /// </value>
        public ICollection<SubWidget> SubWidgets { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>The user.</value>
        public IUserSession User { get; set; }

        /// <summary>
        /// Gets or sets the user service.
        /// </summary>
        /// <value>The user service.</value>
        public IUserService UserService { get; set; }

        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>
        /// The view that this instance of the widget will be using e.g. default.
        /// </value>
        public string View { get; set; }

        /// <summary>
        /// Setups the content.
        /// </summary>
        public void SetupContent()
        {
            this.User = this.UserService.GetUser(HttpContext.Current);
            this.Site = this.SiteService.GetSite(HttpContext.Current);

            if (!string.IsNullOrEmpty(this.Name) && !string.IsNullOrEmpty(this.Context))
            {
                var context = this.GetContext();
                var siteName = this.Shared ? "Shared" : this.Site.Name;
                var widgetName = this.Overbranded ? "Trade" : this.Name;

                IContentModel contentModel = this.ContentService.GetContentForContext(siteName, widgetName, context);
                if (contentModel.Success)
                {
                    this.ContentJSON = contentModel.ContentJSON;
                }

                var contents = new Dictionary<string, List<ContentReturnContext>>();
                if (this.Entities != null)
                {
                    foreach (var entity in this.Entities)
                    {
                        contents.Add(entity, this.ContentService.GetModels(this.Site.Name, entity).Contexts.ToList());
                    }

                    this.EntitiesContent = contents;
                }

                var entities = new Dictionary<string, IContentModel>();
                if (this.EntityList != null)
                {
                    this.SetUpEntityContent(entities);

                    this.SpecificEntitiesContent = entities;
                }
            }
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <returns>the context</returns>
        public string GetContext()
        {
            string context = this.Context;

            if (this.EntitySpecific && !string.IsNullOrEmpty(this.Page.EntityType) && this.Page.EntityInformations != null)
            {
                context = $"{context}_";

                foreach (PageEntityInformation pageEntityInformation in this.Page.EntityInformations.Where(o => !o.Hide))
                {
                    context = $"{context}{pageEntityInformation.UrlSafeValue}-";
                }

                context = context.Remove(context.Length - 1);
            }

            if (this.Shared)
            {
                context = $"{context}-{this.Site.Name.ToLower()}";
            }

            if (this.Overbranded)
            {
                var trade = this.User.TradeSession.Trade;
                context = this.UrlSafeString($"{trade.Name} {trade.ABTAATOLNumber}");
            }

            this.ContentContext = context;
            return context;
        }

        /// <summary>
        /// Sets the content of up entity.
        /// </summary>
        /// <param name="entities">The entities.</param>
        private void SetUpEntityContent(Dictionary<string, IContentModel> entities)
        {
            var siteName = this.Shared ? "Shared" : this.Site.Name;

            foreach (var entity in this.EntityList)
            {
                var entityInformation = this.Page.EntityInformations.FirstOrDefault(entityInfo => string.Equals(entityInfo.Name, entity.Type, StringComparison.CurrentCultureIgnoreCase));

                if (entityInformation != null)
                {
                    IContentModel entityModel = new ContentModel();
                    switch (entity.Source)
                    {
                        case PageDefEntitySource.SiteBuilder:
                            {
                                entityModel = this.ContentService.GetContentForContext(siteName, entityInformation.Name, entityInformation.Value);
                                break;
                            }

                        case PageDefEntitySource.iVector:
                            {
                                string type = entity.SourceType;
                                if (entity.SourceType.IsNullOrEmpty())
                                {
                                    type = entity.Type;
                                }

                                entityModel = this.ContentService.GetCMSContentForObjectType(type, entityInformation.Id);
                                break;
                            }
                    }

                    entities.Add(entityInformation.Name + entity.Source, entityModel);
                }
            }
        }

        /// <summary>
        /// URLs the safe string.
        /// </summary>
        /// <param name="stringToEncode">The string to encode.</param>
        /// <returns>
        /// a string tat has been made url safe.
        /// </returns>
        private string UrlSafeString(string stringToEncode)
        {
            return stringToEncode.ToLower()
                .Replace(" ", "-")
                .Replace("'", string.Empty)
                .Replace("*", string.Empty)
                .Replace(",", string.Empty)
                .Replace(".", string.Empty)
                .Replace(":", string.Empty)
                .Replace("&", "and")
                .Replace("é", "e");
        }
    }
}