namespace Web.Template.Application.PageDefinition
{
    /// <summary>
    /// Class representing entity information relating to a page.
    /// </summary>
    public class PageEntityInformation
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PageEntityInformation"/> is hide.
        /// </summary>
        /// <value>
        ///   <c>true</c> if hide; otherwise, <c>false</c>.
        /// </value>
        public bool Hide { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the URL safe value.
        /// </summary>
        /// <value>
        /// The URL safe value.
        /// </value>
        public string UrlSafeValue { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }
    }
}