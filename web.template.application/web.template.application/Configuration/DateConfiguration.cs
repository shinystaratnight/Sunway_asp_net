namespace Web.Template.Application.Configuration
{
    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Configuration;

    /// <summary>
    /// Class configuring the display of dates on the site.
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Configuration.IDateConfiguration" />
    public class DateConfiguration : IDateConfiguration
    {
        /// <summary>
        /// The number of months displayed by a date picker
        /// </summary>
        private int datepickerMonths;

        /// <summary>
        /// Gets or sets a value indicating whether [date picker dropdowns].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [date picker dropdowns]; otherwise, <c>false</c>.
        /// </value>
        public bool DatePickerDropdowns { get; set; }

        /// <summary>
        /// Gets or sets the date picker first day.
        /// </summary>
        /// <value>
        /// The date picker first day.
        /// </value>
        public DatePickerFirstDay DatePickerFirstDay { get; set; }

        /// <summary>
        /// Gets or sets the date picker months.
        /// </summary>
        /// <value>
        /// The date picker months.
        /// </value>
        public int DatePickerMonths
        {
            get
            {
                return this.datepickerMonths;
            }

            set
            {
                if (value >= 1 && value <= 2)
                {
                    this.datepickerMonths = value;
                }
            }
        }
    }
}