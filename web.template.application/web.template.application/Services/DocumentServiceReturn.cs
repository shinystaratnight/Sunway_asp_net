namespace Web.Template.Application.Services
{
    using System.Collections.Generic;

    /// <summary>
    /// The DocumentService Return.
    /// </summary>
    public class DocumentServiceReturn
    {
        /// <summary>
        /// Gets or sets the document url.
        /// </summary>
        public string DocumentUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the document generation succeeded.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        public List<string> Warnings { get; set; }
    }
}