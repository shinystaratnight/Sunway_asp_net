namespace Web.Template.Application.Configuration
{
    using Web.Template.Application.Interfaces.Configuration;

    /// <summary>
    /// Class responsible for setting flight information on a site. 
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Configuration.IFlightConfiguration" />
    public class FlightConfiguration : IFlightConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether [show flight extras].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show flight extras]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowFlightExtras { get; set; }
    }
}