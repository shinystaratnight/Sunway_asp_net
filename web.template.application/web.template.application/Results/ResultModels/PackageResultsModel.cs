namespace Web.Template.Application.Results.ResultModels
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Concrete package results model
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Models.IPackageResultsModel" />
    public class PackageResultsModel : IPackageResultsModel
    {
        /// <summary>
        /// Gets or sets the Property results.
        /// </summary>
        /// <value>
        /// The results.
        /// </value>
        public List<IResultsModel> ResultsCollection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IResultsModel" /> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the warning.
        /// </summary>
        /// <value>
        /// The warning.
        /// </value>
        public List<string> WarningList { get; set; }
    }
}