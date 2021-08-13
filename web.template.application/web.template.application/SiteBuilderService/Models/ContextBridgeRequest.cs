namespace Web.Template.Application.SiteBuilderService.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// the request sent to the sitebuilder when adding, modifying or deleting a context bridge,
    /// </summary>
    public class ContextBridgeRequest
    {
        /// <summary>
        /// Gets or sets the context bridge identifier.
        /// </summary>
        /// <value>
        /// The context bridge identifier.
        /// </value>
        public int ContextBridgeID { get; set; }

        /// <summary>
        /// Gets or sets the current context.
        /// </summary>
        /// <value>
        /// The current context.
        /// </value>
        public string CurrentContext { get; set; }

        /// <summary>
        /// Gets or sets the entity.
        /// </summary>
        /// <value>
        /// The entity.
        /// </value>
        [JsonIgnore]
        public string Entity { get; set; }

        /// <summary>
        /// Gets or sets the old context.
        /// </summary>
        /// <value>
        /// The old context.
        /// </value>
        public string OldContext { get; set; }
    }
}