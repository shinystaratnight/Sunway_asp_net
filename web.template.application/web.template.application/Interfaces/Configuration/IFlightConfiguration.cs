namespace Web.Template.Application.Interfaces.Configuration
{
    /// <summary>
    /// Flight configuration interface.
    /// </summary>
    public interface IFlightConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether [show flight extras].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show flight extras]; otherwise, <c>false</c>.
        /// </value>
        bool ShowFlightExtras { get; set; }
    }
}