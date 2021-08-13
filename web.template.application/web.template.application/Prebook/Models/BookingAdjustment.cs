namespace Web.Template.Application.Prebook.Models
{
    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Class representing a booking adjustment
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Models.IAdjustment" />
    public class BookingAdjustment : IAdjustment
    {
        /// <summary>
        /// Gets or sets the adjustment amount.
        /// </summary>
        /// <value>
        /// The adjustment amount.
        /// </value>
        public decimal AdjustmentAmount { get; set; }

        /// <summary>
        /// Gets or sets the type of the adjustment.
        /// </summary>
        /// <value>
        /// The type of the adjustment.
        /// </value>
        public string AdjustmentType { get; set; }

        /// <summary>
        /// Gets or sets the calculation basis.
        /// </summary>
        /// <value>
        /// The calculation basis.
        /// </value>
        public string CalculationBasis { get; set; }

        /// <summary>
        /// Gets or sets the type of the parent.
        /// </summary>
        /// <value>
        /// The type of the parent.
        /// </value>
        public string ParentType { get; set; }
    }
}