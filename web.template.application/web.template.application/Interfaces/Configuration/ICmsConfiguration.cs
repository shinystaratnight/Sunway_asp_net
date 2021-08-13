namespace Web.Template.Application.Interfaces.Configuration
{
    /// <summary>
    /// Date configuration interface.
    /// </summary>
    public interface ICmsConfiguration
    {
        /// <summary>
        /// Gets or sets the base url for Cms content from iVector
        /// </summary>
        /// <value>
        ///   the url path
        /// </value>
        string BaseUrl { get; set; }
    }
}