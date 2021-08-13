namespace Web.Template.Application.PageDefinition
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.PageDefinition;
    using Web.Template.Application.Interfaces.Tracking;
    using Web.Template.Application.PageDefinition.Enums;

    /// <summary>
    /// A class representing a page that has been constructed by the site builder
    /// </summary>
    public class Page : IPage
    {
        /// <summary>
        /// Gets or sets the access.
        /// </summary>
        /// <value>
        /// The Access level that a user must have to access the page.
        /// </value>
        public AccessLevel Access { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [admin mode].
        /// </summary>
        /// <value><c>true</c> if [admin mode]; otherwise, <c>false</c>.</value>
        public bool AdminMode { get; set; }

        /// <summary>
        /// Gets or sets the URL information.
        /// </summary>
        /// <value>
        /// The URL information.
        /// </value>
        public List<PageEntityInformation> EntityInformations { get; set; }

        /// <summary>
        /// Gets or sets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        public string EntityType { get; set; }

        /// <summary>
        /// Gets or sets the font script.
        /// </summary>
        /// <value>The font script.</value>
        public string FontScript { get; set; }

        /// <summary>
        /// Gets or sets the font source.
        /// </summary>
        /// <value>The font source.</value>
        public string FontSource { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the meta information.
        /// </summary>
        /// <value>
        /// The meta information.
        /// </value>
        public PageMetaInformation MetaInformation { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name of the page e.g. Homepage
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the page title.
        /// </summary>
        /// <value>
        /// The page title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the state of the pre loaded.
        /// </summary>
        /// <value>The state of the pre loaded.</value>
        public string PreLoadedState { get; set; }

        /// <summary>
        /// Gets or sets the sections.
        /// </summary>
        /// <value>
        /// A collection of sections that the page has e.g. sidebar
        /// </value>
        public ICollection<Section> Sections { get; set; }

        /// <summary>
        /// Gets or sets the site base URL.
        /// </summary>
        /// <value>The site base URL.</value>
        public string SiteBaseUrl { get; set; }

        /// <summary>
        /// Gets or sets the name of the site.
        /// </summary>
        /// <value>The name of the site.</value>
        public string SiteName { get; set; }

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>
        /// The template that the page will use for layout e.g. Standard Sidebar Left
        /// </value>
        public string Template { get; set; }

        /// <summary>
        /// Gets or sets the tracking affiliates.
        /// </summary>
        /// <value>
        /// The tracking affiliates.
        /// </value>
        public List<ITrackingAffiliate> TrackingAffiliates { get; set; }

        /// <summary>
        /// Gets or sets the user handle.
        /// </summary>
        /// <value>
        /// The user handle.
        /// </value>
        public string TwitterHandle { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL to which the page will refer
        /// </value>
        public string Url { get; set; }

		/// <summary>
		/// Gets or sets the additional HTML.
		/// </summary>
		/// <value>
		/// The additional HTML.
		/// </value>
		public string AdditionalHTML { get; set; }

	    /// <summary>
        /// Gets or sets the published.
        /// </summary>
        /// <value>
        /// The published.
        /// </value>
        public bool Published { get; set; }
    }
}