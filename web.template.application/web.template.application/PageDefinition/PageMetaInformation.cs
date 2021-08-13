namespace Web.Template.Application.PageDefinition
{
    /// <summary>
    /// Class representing the meta information of a page. e.g. the title and description that go in the header.
    /// </summary>
    public class PageMetaInformation
    {
        /// <summary>
        /// Gets or sets the canonical URL.
        /// </summary>
        /// <value>
        /// The canonical URL.
        /// </value>
        public string CanonicalUrl { get; set; }

        /// <summary>
        /// Gets or sets the meta description.
        /// </summary>
        /// <value>
        /// The meta description.
        /// </value>
        public string MetaDescription { get; set; }

        /// <summary>
        /// Gets or sets the page image.
        /// </summary>
        /// <value>
        /// The page image.
        /// </value>
        public string PageImage { get; set; }

        /// <summary>
        /// Gets or sets the page title.
        /// </summary>
        /// <value>
        /// The page title.
        /// </value>
        public string PageTitle { get; set; }

        /// <summary>
        /// Gets or sets the social media title.
        /// </summary>
        /// <value>
        /// The social media title.
        /// </value>
        public string SocialMediaDescription { get; set; }

        /// <summary>
        /// Gets or sets the social media title.
        /// </summary>
        /// <value>
        /// The social media title.
        /// </value>
        public string SocialMediaTitle { get; set; }
    }
}