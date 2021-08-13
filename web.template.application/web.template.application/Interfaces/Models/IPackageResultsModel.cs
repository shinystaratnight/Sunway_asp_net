namespace Web.Template.Application.Interfaces.Models
{
    using System.Collections.Generic;

    /// <summary>
    ///     PackageResultsCollection model interface
    /// </summary>
    public interface IPackageResultsModel
    {
        /// <summary>
        ///     Gets or sets the Property results.
        /// </summary>
        /// <value>
        ///     The results.
        /// </value>
        List<IResultsModel> ResultsCollection { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="IResultsModel" /> is success.
        /// </summary>
        /// <value>
        ///     <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        bool Success { get; set; }

        /// <summary>
        ///     Gets or sets the warning.
        /// </summary>
        /// <value>
        ///     The warning.
        /// </value>
        List<string> WarningList { get; set; }
    }
}