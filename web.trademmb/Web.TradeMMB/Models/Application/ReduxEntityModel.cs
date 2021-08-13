namespace Web.TradeMMB.Models.Application
{
    using System;

    /// <summary>
    /// The Entity Model used in Redux.
    /// </summary>
    public class ReduxEntityModel
    {
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public string context { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is fetching.
        /// </summary>
        /// <value><c>true</c> if this instance is fetching; otherwise, <c>false</c>.</value>
        public bool isFetching { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is loaded.
        /// </summary>
        /// <value><c>true</c> if this instance is loaded; otherwise, <c>false</c>.</value>
        public bool isLoaded { get; set; }

        /// <summary>
        /// Gets or sets the json schema.
        /// </summary>
        /// <value>The json schema.</value>
        public string jsonSchema { get; set; }

        /// <summary>
        /// Gets or sets the last modified date.
        /// </summary>
        /// <value>The last modified date.</value>
        public DateTime lastModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the last modified user.
        /// </summary>
        /// <value>The last modified user.</value>
        public string lastModifiedUser { get; set; }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public string model { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string name { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string status { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string type { get; set; }
    }
}