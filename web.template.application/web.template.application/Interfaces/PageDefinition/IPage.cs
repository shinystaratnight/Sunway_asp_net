namespace Web.Template.Application.Interfaces.PageDefinition
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Tracking;
    using Web.Template.Application.PageDefinition;
    using Web.Template.Application.PageDefinition.Enums;

    /// <summary>
    /// Interface representing a web page.
    /// </summary>
    public interface IPage
    {
        /// <summary>
        /// Gets or sets the access.
        /// </summary>
        /// <value>
        /// The access.
        /// </value>
        AccessLevel Access { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [admin mode].
        /// </summary>
        /// <value><c>true</c> if [admin mode]; otherwise, <c>false</c>.</value>
        bool AdminMode { get; set; }

        /// <summary>
        /// Gets or sets the entity information.
        /// </summary>
        /// <value>The entity information.</value>
        List<PageEntityInformation> EntityInformations { get; set; }

        /// <summary>
        /// Gets or sets the type of the entity.
        /// </summary>
        /// <value>The type of the entity.</value>
        string EntityType { get; set; }

        /// <summary>
        /// Gets or sets the font script.
        /// </summary>
        /// <value>The font script.</value>
        string FontScript { get; set; }

        /// <summary>
        /// Gets or sets the font source.
        /// </summary>
        /// <value>The font source.</value>
        string FontSource { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        int Id { get; set; }

        /// <summary>
        /// Gets or sets the meta information.
        /// </summary>
        /// <value>The meta information.</value>
        PageMetaInformation MetaInformation { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets the state of the pre loaded.
        /// </summary>
        /// <value>The state of the pre loaded.</value>
        string PreLoadedState { get; set; }

        /// <summary>
        /// Gets or sets the sections.
        /// </summary>
        /// <value>
        /// The sections.
        /// </value>
        ICollection<Section> Sections { get; set; }
        
        /// <summary>
        /// Gets or sets the site base URL.
        /// </summary>
        /// <value>The site base URL.</value>
        string SiteBaseUrl { get; set; }

        /// <summary>
        /// Gets or sets the name of the site.
        /// </summary>
        /// <value>The name of the site.</value>
        string SiteName { get; set; }

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>
        /// The template.
        /// </value>
        string Template { get; set; }

        /// <summary>
        /// Gets or sets the tracking affiliates.
        /// </summary>
        /// <value>
        /// The tracking affiliates.
        /// </value>
        List<ITrackingAffiliate> TrackingAffiliates { get; set; }

        /// <summary>
        /// Gets or sets the user handle.
        /// </summary>
        /// <value>
        /// The user handle.
        /// </value>
        string TwitterHandle { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        string Url { get; set; }

		/// <summary>
		/// Gets or sets the additional HTML.
		/// </summary>
		/// <value>
		/// The additional HTML.
		/// </value>
		string AdditionalHTML { get; set; }
    }
}