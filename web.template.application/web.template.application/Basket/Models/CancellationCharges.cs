namespace Web.Template.Application.Basket.Models
{
    using System;

    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Class representing a cancellation charge.
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Models.ICancellationCharge" />
    public class CancellationCharge : ICancellationCharge
    {
        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        /// <value>
        /// The amount.
        /// </value>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>
        /// The end date.
        /// </value>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>
        /// The start date.
        /// </value>
        public DateTime StartDate { get; set; }
    }
}