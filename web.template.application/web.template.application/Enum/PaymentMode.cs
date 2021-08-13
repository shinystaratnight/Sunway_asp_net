namespace Web.Template.Application.Enum
{
    /// <summary>
    /// Enum of potential payment modes
    /// </summary>
    public enum PaymentMode
    {
        /// <summary>
        /// The standard payment method
        /// </summary>
        Standard,

        /// <summary>
        /// Payment taken in i frame from payment provider
        /// </summary>
        OffsiteIFrame,

        /// <summary>
        /// Payment taken after redirecting to offsite payment provider
        /// </summary>
        OffsiteRedirect
    }
}
