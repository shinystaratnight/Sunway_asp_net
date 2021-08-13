namespace Web.Template.Application.Interfaces.Book
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Prebook;

    /// <summary>
    /// Interface defining the prebook return model
    /// </summary>
    public interface IBookReturn
    {
        /// <summary>
        /// Gets or sets the basket.
        /// </summary>
        /// <value>
        /// The basket.
        /// </value>
        IBasket Basket { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [component failed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [component failed]; otherwise, <c>false</c>.
        /// </value>
        bool ComponentFailed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IPrebookReturn"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        bool Success { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [three d secure enrolled].
        /// </summary>
        /// <value><c>true</c> if [three d secure enrolled]; otherwise, <c>false</c>.</value>
        bool ThreeDSecureEnrollment { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>
        /// The warnings.
        /// </value>
        List<string> Warnings { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [offsite redirect].
		/// </summary>
		/// <value>
		///   <c>true</c> if [offsite redirect]; otherwise, <c>false</c>.
		/// </value>
		bool OffsiteRedirect { get; set; }

		/// <summary>
		/// Gets or sets the offsite redirect HTML.
		/// </summary>
		/// <value>
		/// The offsite redirect HTML.
		/// </value>
		string OffsiteRedirectHTML { get; set; }
    }
}