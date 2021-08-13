namespace Web.Template.Application.Interfaces.Models
{
    using System.Collections.Generic;

    using Web.Template.Application.Enum;

    /// <summary>
    ///     ResultsCollection model interface
    /// </summary>
    public interface IResultsModel
    {
        /// <summary>
        /// Gets or sets the extra search model.
        /// </summary>
        /// <value>The extra search model.</value>
        IExtraSearchModel ExtraSearchModel { get; set; }

        /// <summary>
        ///     Gets or sets the Property results.
        /// </summary>
        /// <value>
        ///     The results.
        /// </value>
        List<IResult> ResultsCollection { get; set; }

        /// <summary>
        /// Gets or sets the result token.
        /// </summary>
        /// <value>
        /// The result token.
        /// </value>
        string ResultToken { get; set; }

        /// <summary>
        ///     Gets or sets the search mode.
        /// </summary>
        /// <value>
        ///     The search mode.
        /// </value>
        SearchMode SearchMode { get; set; }

        /// <summary>
        /// Gets or sets the search model.
        /// </summary>
        /// <value>
        /// The search model.
        /// </value>
        ISearchModel SearchModel { get; set; }

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