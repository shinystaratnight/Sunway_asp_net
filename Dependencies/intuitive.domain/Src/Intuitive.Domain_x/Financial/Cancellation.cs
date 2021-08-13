namespace Intuitive.Domain.Financial
{
    using System;

    using Intuitive.Domain.Financial.Interfaces;

    /// <summary>
    ///     Represents a single cancellation.
    /// </summary>
    /// <seealso cref="ICancellation"/>
    public class Cancellation : ICancellation
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Cancellation"/> class.
        /// </summary>
        public Cancellation()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cancellation"/> class.
        /// </summary>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
        public Cancellation(DateTime startDate, DateTime endDate, decimal amount)
        {
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Amount = amount;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cancellation"/> class.
        /// </summary>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <param name="roundDecimals">
        /// The round decimals.
        /// </param>
        public Cancellation(DateTime startDate, DateTime endDate, decimal amount, int roundDecimals)
        {
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Amount = decimal.Round(amount, roundDecimals);
        }

        /// <summary>
        ///     Gets or sets the end date.
        /// </summary>
        /// <value>
        ///     The end date.
        /// </value>
        public DateTime EndDate { get; set; }

        /// <summary>
        ///     Gets or sets the start date.
        /// </summary>
        /// <value>
        ///     The start date.
        /// </value>
        public DateTime StartDate { get; set; }

        /// <summary>
        ///     Gets or sets the amount.
        /// </summary>
        /// <value>
        ///     The amount.
        /// </value>
        public decimal Amount { get; set; }

        /// <summary>
        /// Returns true if the object equals the parameter
        /// </summary>
        /// <param name="obj">The object to check equality with</param>
        /// <returns>True if equal, false otherwise</returns>
        public override bool Equals(object obj)
        {
            Cancellation other = obj as Cancellation;

            if (other == null)
            {
                return false;
            }

            return this.StartDate == other.StartDate &&
                   this.EndDate == other.EndDate &&
                   this.Amount == other.Amount;
        }

        /// <summary>
        /// Overrides the get hash code method
        /// </summary>
        /// <returns>Hash code based on the equality operator</returns>
        public override int GetHashCode()
        {
            return string.Join("|", this.StartDate, this.EndDate, this.Amount).GetHashCode();
        }
    }
}