namespace Web.Template.Application.Configuration
{
    using System.Collections.Generic;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Configuration;

    /// <summary>
    /// Class BookingJourneyConfiguration.
    /// </summary>
    public class BookingJourneyConfiguration : IBookingJourneyConfiguration
    {
        /// <summary>
        /// Gets or sets the change flight pages.
        /// </summary>
        /// <value>The change flight pages.</value>
        public List<string> ChangeFlightPages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [default deposit payment].
        /// </summary>
        /// <value><c>true</c> if [default deposit payment]; otherwise, <c>false</c>.</value>
        public bool DefaultDepositPayment { get; set; }

        /// <summary>
        /// Gets or sets the search modes.
        /// </summary>
        /// <value>The search modes.</value>
        public List<ISearchModeConfiguration> SearchModes { get; set; }

        /// <summary>
        /// Gets or sets the OnRequestShowOptions
        /// </summary>
        /// <value>The OnRequestShowOptions.</value>
        public OnRequestDisplay OnRequestDisplay { get; set; }

        /// <summary>
        /// Gets or sets the three d secure provider.
        /// </summary>
        /// <value>The three d secure provider.</value>
        public ThreeDSecureProvider ThreeDSecureProvider { get; set; }

        /// <summary>
        /// Gets or sets the hide cancellation charges.
        /// </summary>
        /// <value>
        /// The hide cancellation charges.
        /// </value>
        public bool HideCancellationCharges { get; set; }

        /// <summary>
        /// Gets or sets the payment mode.
        /// </summary>
        /// <value>
        /// The payment mode.
        /// </value>
        [JsonConverter(typeof(StringEnumConverter))]
        public PaymentMode PaymentMode { get; set; }

        /// <summary>
        /// Gets or sets the option to validate child and infant ages in the booking journey.
        /// </summary>
        /// <value>
        /// The validate child infant ages option.
        /// </value>
        public bool ValidateChildInfantAges { get; set; }

        /// Gets or sets the searchtool location.
        /// </summary>
        /// <value>
        /// The searchtool location.
        /// </value>
        [JsonConverter(typeof(StringEnumConverter))]
        public SearchToolLocation SearchToolLocation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [render mobile summary].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render mobile summary]; otherwise, <c>false</c>.
        /// </value>
        public bool RenderMobileSummary { get; set; }

        /// <summary>
        /// Gets or sets the payment url to use in the booking journey and breadcrumb.
        /// </summary>
        /// <value>The payment url to use.</value>
        public string PaymentUrl { get; set; }
    }
}