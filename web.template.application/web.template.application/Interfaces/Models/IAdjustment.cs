namespace Web.Template.Application.Interfaces.Models
{
    /// <summary>
    /// Adjustment interface
    /// </summary>
    public interface IAdjustment
    {
        /// <summary>
        /// Gets or sets the adjustment amount.
        /// </summary>
        /// <value>
        /// The adjustment amount.
        /// </value>
        decimal AdjustmentAmount { get; set; }

        /// <summary>
        /// Gets or sets the type of the adjustment.
        /// </summary>
        /// <value>
        /// The type of the adjustment.
        /// </value>
        string AdjustmentType { get; set; }

        /// <summary>
        /// Gets or sets the calculation basis.
        /// </summary>
        /// <value>
        /// The calculation basis.
        /// </value>
        string CalculationBasis { get; set; }

        /// <summary>
        /// Gets or sets the type of the parent.
        /// </summary>
        /// <value>
        /// The type of the parent.
        /// </value>
        string ParentType { get; set; }
    }
}