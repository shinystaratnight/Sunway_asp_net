namespace Web.Template.Application.Basket.Models
{
    using System;

    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Class representing a payment.
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Models.IPayment" />
    public class Payment : IPayment
    {
        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        /// <value>
        /// The amount.
        /// </value>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the date due.
        /// </summary>
        /// <value>
        /// The date due.
        /// </value>
        public DateTime DateDue { get; set; }

        /// <summary>
        /// Gets or sets the display amount.
        /// </summary>
        /// <value>
        /// The display amount.
        /// </value>
        public decimal DisplayAmount { get; set; }
    }
}