namespace Web.Template.Application.Interfaces.Configuration
{
    using Web.Template.Application.Enum;

    /// <summary>
    /// Date configuration interface.
    /// </summary>
    public interface IDateConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether [date picker dropdowns].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [date picker dropdowns]; otherwise, <c>false</c>.
        /// </value>
        bool DatePickerDropdowns { get; set; }

        /// <summary>
        /// Gets or sets the date picker first day.
        /// </summary>
        /// <value>
        /// The date picker first day.
        /// </value>
        DatePickerFirstDay DatePickerFirstDay { get; set; }

        /// <summary>
        /// Gets or sets the date picker months.
        /// </summary>
        /// <value>
        /// The date picker months.
        /// </value>
        int DatePickerMonths { get; set; }
    }
}