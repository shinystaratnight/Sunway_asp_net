namespace Web.Template.Application.Book.Models
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Book;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Prebook.Models;

    /// <summary>
    /// Model returned from the prebook to the UI.
    /// </summary>
    public class BookReturn : IBookReturn
    {
        /// <summary>
        /// Gets or sets the basket.
        /// </summary>
        /// <value>
        /// The basket.
        /// </value>
        public IBasket Basket { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [component failed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [component failed]; otherwise, <c>false</c>.
        /// </value>
        public bool ComponentFailed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PrebookReturn"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [three d secure enrolled].
        /// </summary>
        /// <value><c>true</c> if [three d secure enrolled]; otherwise, <c>false</c>.</value>
        public bool ThreeDSecureEnrollment { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>
        /// The warnings.
        /// </value>
        public List<string> Warnings { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [offsite redirect].
		/// </summary>
		/// <value>
		///   <c>true</c> if [offsite redirect]; otherwise, <c>false</c>.
		/// </value>
		public bool OffsiteRedirect { get; set; }

		/// <summary>
		/// Gets or sets the offsite redirect HTML.
		/// </summary>
		/// <value>
		/// The offsite redirect HTML.
		/// </value>
		public string OffsiteRedirectHTML { get; set; }
    }
}