namespace Web.Template.Application.Prebook.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using Web.Template.Application.Enum;

    /// <summary>
    /// class defining a single piece of Errata
    /// </summary>
    /// <seealso cref="Web.Template.Application.Prebook.Models.IErratum" />
    public class Erratum : IErratum
    {
        /// <summary>
        /// Gets or sets the type of the component.
        /// </summary>
        /// <value>The type of the component.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public ComponentType ComponentType { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        public string Subject { get; set; }
    }
}