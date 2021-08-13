namespace Web.Template.Application.Interfaces.BookingAdjustment
{
    using System.Collections.Generic;

    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Interface IBookingAdjustmentService
    /// </summary>
    public interface IBookingAdjustmentService
    {

        /// <summary>
        /// Retrieves the result.
        /// </summary>
        /// <param name="searchToken">The search token.</param>
        /// <returns>The adjustments</returns>
        List<IAdjustment> RetrieveResult(string searchToken);

        /// <summary>
        /// Searches this instance.
        /// </summary>
        /// <param name="bookingAdjustmentSearchModel">The booking adjustment search model.</param>
        /// <returns>The search return</returns>
        IBookingAdjustmentSearchReturn Search(IBookingAdjustmentSearchModel bookingAdjustmentSearchModel);
    }
}
