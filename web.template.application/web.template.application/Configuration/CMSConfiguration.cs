namespace Web.Template.Application.Configuration
{
    using Web.Template.Application.Interfaces.Configuration;

    /// <summary>
    /// Class configuring the display of iVector CMS content on the site.
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Configuration.IDateConfiguration" />
    public class CmsConfiguration : ICmsConfiguration
    {
        /// <summary>
        /// Gets or sets the base url for Cms content from iVector
        /// </summary>
        /// <value>
        /// the url path
        /// </value>
        public string BaseUrl { get; set; }
    }
}