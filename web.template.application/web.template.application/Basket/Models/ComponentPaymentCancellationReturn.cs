using System.Collections.Generic;

namespace Web.Template.Application.Basket.Models
{
	using Web.Template.Application.Interfaces.Models;
	/// <summary>
	/// 
	/// </summary>
	/// <seealso cref="Web.Template.Application.Interfaces.Models.IComponentPaymentCancellationReturn" />
	public class ComponentPaymentCancellationReturn : IComponentPaymentCancellationReturn
	{
        /// <summary>
        /// Gets or sets the component token.
        /// </summary>
        /// <value>
        /// The component token.
        /// </value>
        public int ComponentToken { get; set; }

	    /// <summary>
		/// Gets or sets the payments.
		/// </summary>
		/// <value>
		/// The payments.
		/// </value>
		public List<IPayment> Payments { get; set; }

		/// <summary>
		/// Gets or sets the cancellation charges.
		/// </summary>
		/// <value>
		/// The cancellation charges.
		/// </value>
		public List<ICancellationCharge> CancellationCharges { get; set; }

		/// <summary>
		/// Gets or sets the warnings.
		/// </summary>
		/// <value>
		/// The warnings.
		/// </value>
		public List<string> Warnings { get; set; }
	}
}
