namespace Web.Template.Application.Interfaces.Models
{
    using System;

    /// <summary>
    /// Payments due
    /// </summary>
    public interface IPayment
    {
        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        /// <value>
        /// The amount.
        /// </value>
        decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the date due.
        /// </summary>
        /// <value>
        /// The date due.
        /// </value>
        DateTime DateDue { get; set; }

        /// <summary>
        /// Gets or sets the display amount.
        /// </summary>
        /// <value>
        /// The display amount.
        /// </value>
        decimal DisplayAmount { get; set; }
    }
}