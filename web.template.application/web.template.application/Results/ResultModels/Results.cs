namespace Web.Template.Application.Results.ResultModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    ///     Class representing hotel results returned from search
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Models.IResultsModel" />
    [DebuggerDisplay("{SearchMode} search with {ResultsCollection.Count} results, {WarningList.Count} warnings")]
    public class Results : IResultsModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Results"/> class.
        /// </summary>
        public Results()
        {
            this.WarningList = new List<string>();
            this.ResultsCollection = new List<IResult>();
            this.ResultToken = SetupToken();
        }
        
        /// <summary>
        /// Gets or sets the extra search model.
        /// </summary>
        /// <value>The extra search model.</value>
        public IExtraSearchModel ExtraSearchModel { get; set; }

        /// <summary>
        ///     Gets or sets the Property results.
        /// </summary>
        /// <value>
        ///     The results.
        /// </value>
        public List<IResult> ResultsCollection { get; set; }

        /// <summary>
        /// Gets or sets the result token.
        /// </summary>
        /// <value>
        /// The result token.
        /// </value>
        public string ResultToken { get; set; }

        /// <summary>
        ///     Gets or sets the search mode.
        /// </summary>
        /// <value>
        ///     The search mode.
        /// </value>
        public SearchMode SearchMode { get; set; }

        /// <summary>
        /// Gets or sets the search model.
        /// </summary>
        /// <value>
        /// The search model.
        /// </value>
        public ISearchModel SearchModel { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="IResultsModel" /> is success.
        /// </summary>
        /// <value>
        ///     <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public bool Success { get; set; }

        /// <summary>
        ///     Gets or sets the warning.
        /// </summary>
        /// <value>
        ///     The warning.
        /// </value>
        public List<string> WarningList { get; set; }

        /// <summary>
        /// Setups the token.
        /// </summary>
        /// <returns>A token unique to the search</returns>
        private static string SetupToken()
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            string token = Convert.ToBase64String(time.Concat(key).ToArray());
            return token;
        }
    }
}