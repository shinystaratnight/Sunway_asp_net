namespace Web.Template.Application.Book
{
    using iVectorConnectInterface.Property;

    using Web.Template.Application.Interfaces.Book;
    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Updates basket with values from Prebook response, could potentially make a basket visitor instead.
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Prebook.IPrebookResponseProcessor" />
    public class BookResponseProcessor : IBookResponseProcessor
    {
        /// <summary>
        /// Processes the specified book response.
        /// </summary>
        /// <param name="bookResponse">The book response.</param>
        /// <param name="basket">The basket.</param>
        public void Process(iVectorConnectInterface.Basket.BookResponse bookResponse, IBasket basket)
        {
            this.UpdateBasketWithResponseValues(bookResponse, basket);
        }

        /// <summary>
        /// Updates the basket with response values.
        /// </summary>
        /// <param name="bookResponse">The book response.</param>
        /// <param name="basket">The basket.</param>
        private void UpdateBasketWithResponseValues(iVectorConnectInterface.Basket.BookResponse bookResponse, IBasket basket)
        {
            basket.AllComponentsBooked = true;

            basket.BookingReference = bookResponse.BookingReference;

            // This is where we get valid creditCardTypes
            if (basket.Components != null)
            {
                foreach (BookResponse property in bookResponse.PropertyBookings)
                {
                    if (!property.ReturnStatus.Success)
                    {
                        basket.ComponentFailedToBook = true;
                    }
                }

                foreach (iVectorConnectInterface.Flight.BookResponse flight in bookResponse.FlightBookings)
                {
                    if (!flight.ReturnStatus.Success)
                    {
                        basket.ComponentFailedToBook = true;
                    }
                }

                foreach (iVectorConnectInterface.Transfer.BookResponse transfer in bookResponse.TransferBookings)
                {
                    if (!transfer.ReturnStatus.Success)
                    {
                        basket.ComponentFailedToBook = true;
                    }
                }
            }

            // If oIVCReturn.Timeout Then

            // oBookReturn.Warnings.Add(BookWarning.Timeout)

            // End If
        }
    }
}