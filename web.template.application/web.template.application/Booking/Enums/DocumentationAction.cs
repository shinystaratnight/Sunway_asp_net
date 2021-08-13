namespace Web.Template.Application.Booking.Enums
{
    /// <summary>
    /// Defines actions regarding the interaction with booking documentation.
    /// </summary>
    public enum DocumentationAction
    {
        /// <summary>
        /// We can view documentation, getting a list of paths to the files
        /// </summary>
        View,

        /// <summary>
        /// We can send documentation sending it to a given email address or the one on the booking
        /// </summary>
        Send
    }
}