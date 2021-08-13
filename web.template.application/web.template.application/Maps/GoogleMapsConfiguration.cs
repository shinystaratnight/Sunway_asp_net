namespace Web.Template.Application.Maps
{
    using Web.Template.Application.Interfaces.Map;

    /// <summary>
    /// Google map configuration class, used to store information about setting up google maps such as the key.
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Map.IMapConfiguration" />
    public class GoogleMapsConfiguration : IMapConfiguration
    {
        /// <summary>
        /// Gets or sets the key
        /// </summary>
        /// <value>
        /// The access key.
        /// </value>
        public string Key { get; set; }
    }
}