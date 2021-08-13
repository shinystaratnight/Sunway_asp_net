namespace Web.Template.Application.Site
{
    using System.Collections.Generic;

    /// <summary>
    /// A class returned from the redirect controller that reports on the outcome of adding, deleting or modifying a redirect.
    /// </summary>
    public class RedirectReturn
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RedirectReturn"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public bool Success { get; set; } = true;

        /// <summary>
        /// Gets or sets the warning list.
        /// </summary>
        /// <value>
        /// The warning list.
        /// </value>
        public List<string> WarningList { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectReturn"/> class.
        /// </summary>
        public RedirectReturn()
        {
            WarningList = new List<string>();
        }
    }
}