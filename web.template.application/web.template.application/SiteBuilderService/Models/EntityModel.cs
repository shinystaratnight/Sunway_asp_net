namespace Web.Template.Application.SiteBuilderService.Models
{
    using System;

    /// <summary>
    /// The model returned by the entity call by the sitebuilder user interface, concerning all information about an entity.
    /// </summary>
    public class EntityModel
    {
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public string Context { get; set; }

        /// <summary>
        /// Gets or sets the json schema.
        /// </summary>
        /// <value>
        /// The json schema.
        /// </value>
        public string JsonSchema { get; set; }

        /// <summary>
        /// Gets or sets the languages.
        /// </summary>
        /// <value>
        /// The languages.
        /// </value>
        public string[] Langauges { get; set; }

        /// <summary>
        /// Gets or sets the last modified date.
        /// </summary>
        /// <value>
        /// The last modified date.
        /// </value>
        public DateTime LastModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the last modified user.
        /// </summary>
        /// <value>
        /// The last modified user.
        /// </value>
        public string LastModifiedUser { get; set; }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string Type { get; set; }
    }
}