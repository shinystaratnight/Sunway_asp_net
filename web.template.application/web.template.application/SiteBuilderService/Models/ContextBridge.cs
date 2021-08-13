namespace Web.Template.Application.SiteBuilderService.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// a class serialized to from sitebuilder
    /// </summary>
    public class ContextBridge
    {
        /// <summary>
        /// Gets or sets the context bridge identifier.
        /// </summary>
        /// <value>
        /// The context bridge identifier.
        /// </value>
        [JsonProperty("ID")]
        public int ContextBridgeID { get; set; }

        /// <summary>
        /// Gets or sets the old context.
        /// </summary>
        /// <value>
        /// The old context.
        /// </value>
        public string OldContext { get; set; }

        /// <summary>
        /// Gets or sets the current context.
        /// </summary>
        /// <value>
        /// The current context.
        /// </value>
        public string CurrentContext { get; set; }

        /// <summary>
        /// Gets or sets the site identifier.
        /// </summary>
        /// <value>
        /// The site identifier.
        /// </value>
        public int SiteID { get; set; }

        /// <summary>
        /// Gets or sets the instance identifier.
        /// </summary>
        /// <value>
        /// The instance identifier.
        /// </value>
        public int InstanceID { get; set; }

        /// <summary>
        /// Gets or sets the entity identifier.
        /// </summary>
        /// <value>
        /// The entity identifier.
        /// </value>
        public int EntityID { get; set; }
    }
}