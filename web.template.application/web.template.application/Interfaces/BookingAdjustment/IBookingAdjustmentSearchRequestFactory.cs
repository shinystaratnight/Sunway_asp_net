namespace Web.Template.Application.Interfaces.BookingAdjustment
{
    using iVectorConnectInterface.Interfaces;

    /// <summary>
    /// Interface IBookingAdjustmentSearchRequestFactory
    /// </summary>
    public interface IBookingAdjustmentSearchRequestFactory
    {
        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <param name="searchModel">The booking adjustment search model.</param>
        /// <returns>The iVectorConnectRequest.</returns>
        iVectorConnectRequest Create(IBookingAdjustmentSearchModel searchModel);
    }
}
