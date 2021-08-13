namespace Web.Template.Application.Enum
{
    /// <summary>
    /// enum representing the types of payment.
    /// </summary>
    public enum PaymentType
    {
        /// <summary>
        /// The unset
        /// </summary>
        Unset,

        /// <summary>
        /// The credit card
        /// </summary>
        CreditCard,

        /// <summary>
        /// The custom with make payment
        /// </summary>
        CustomWithMakePayment,

        /// <summary>
        /// The offsite make payment
        /// </summary>
        OffsiteMakePayment,

        /// <summary>
        /// The offsite payment taken
        /// </summary>
        OffsitePaymentTaken,

        /// <summary>
        /// The custom
        /// </summary>
        Custom,
    }
}