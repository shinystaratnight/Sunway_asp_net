namespace Web.Template.Application.Interfaces.Configuration
{
    using System.Collections.Generic;

    using Web.Template.Application.Enum;

    /// <summary>
    /// Interface IBookingJourneyConfiguration
    /// </summary>
    public interface IBookingJourneyConfiguration
    {
        /// <summary>
        /// Gets or sets the change flight pages.
        /// </summary>
        /// <value>The change flight pages.</value>
        List<string> ChangeFlightPages { get; set; }

        /// <summary>
        /// Gets or sets the search modes.
        /// </summary>
        /// <value>The search modes.</value>
        List<ISearchModeConfiguration> SearchModes { get; set; }

        /// <summary>
        /// Gets or sets the OnRequestShowOptions
        /// </summary>
        /// <value>The OnRequestShowOptions.</value>
        OnRequestDisplay OnRequestDisplay { get; set; }

        /// <summary>
        /// Gets or sets the three d secure provider.
        /// </summary>
        /// <value>The three d secure provider.</value>
        ThreeDSecureProvider ThreeDSecureProvider { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [hide cancellation charges].
        /// </summary>
        /// <value>
        /// <c>true</c> if [hide cancellation charges]; otherwise, <c>false</c>.
        /// </value>
        bool HideCancellationCharges { get; set; }

        /// <summary>
        /// Gets or sets the payment mode.
        /// </summary>
        /// <value>
        /// The payment mode.
        /// </value>
        PaymentMode PaymentMode { get; set; }


        /// <summary>
        /// Gets or sets the payment page path.
        /// </summary>
        /// <value>
        /// The payment page path.
        /// </value>
        string PaymentUrl { get; set; }
    }
}