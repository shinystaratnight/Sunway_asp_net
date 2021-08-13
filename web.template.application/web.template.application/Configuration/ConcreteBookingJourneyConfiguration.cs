namespace Web.Template.Application.Configuration
{
    using System.Collections.Generic;

    /// <summary>
    /// concrete booking journey config used for serialization
    /// </summary>
    public class ConcreteBookingJourneyConfiguration
    {
        /// <summary>
        /// Gets or sets the change flight pages.
        /// </summary>
        /// <value>The change flight pages.</value>
        public List<string> ChangeFlightPages { get; set; }

        /// <summary>
        /// Gets or sets the search modes.
        /// </summary>
        /// <value>The search modes.</value>
        public List<SearchModeConfiguration> SearchModes { get; set; }
    }
}