namespace Web.Template.Application.Book.Builders
{
    using System.Collections.Generic;

    using iVectorConnectInterface.Property;

    using Web.Template.Application.Interfaces.Book;
    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Builds a prebook return
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Book.IBookReturnBuilder" />
    /// <seealso cref="Web.Template.Application.Interfaces.Prebook.IPrebookReturnBuilder" />
    public class BookReturnBuilder : IBookReturnBuilder
    {
        /// <summary>
        /// The prebook return
        /// </summary>
        private IBookReturn bookReturn;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookReturnBuilder" /> class.
        /// </summary>
        /// <param name="bookReturn">The prebook return.</param>
        public BookReturnBuilder(IBookReturn bookReturn)
        {
            this.bookReturn = bookReturn;
            this.bookReturn.Warnings = new List<string>();
            this.bookReturn.Success = true;
        }

        /// <summary>
        /// Gets a value indicating whether [currently successful].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [currently successful]; otherwise, <c>false</c>.
        /// </value>
        public bool CurrentlySuccessful => this.bookReturn.Success;

        /// <summary>
        /// Adds the response.
        /// </summary>
        /// <param name="bookResponse">The book response.</param>
        public void AddResponse(iVectorConnectInterface.Basket.BookResponse bookResponse)
        {
            this.SetupReturnBookingErrors(bookResponse);
        }

        /// <summary>
        /// Adds the warning.
        /// </summary>
        /// <param name="warning">The warning.</param>
        public void AddWarning(string warning)
        {
            this.bookReturn.Warnings.Add(warning);
            this.bookReturn.Success = false;
        }

        /// <summary>
        /// Adds the warnings.
        /// </summary>
        /// <param name="warnings">The warnings.</param>
        public void AddWarnings(List<string> warnings)
        {
            this.bookReturn.Warnings.AddRange(warnings);
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns>
        /// A prebook return
        /// </returns>
        public IBookReturn Build()
        {
            return this.bookReturn;
        }

        /// <summary>
        /// Sets the basket.
        /// </summary>
        /// <param name="basket">The basket.</param>
        public void SetBasket(IBasket basket)
        {
            this.bookReturn.Basket = basket;
        }

        /// <summary>
        /// Sets to failure.
        /// </summary>
        public void SetToFailure()
        {
            this.bookReturn.Success = false;
        }

        /// <summary>
        /// Checks the bookings for errors.
        /// </summary>
        /// <param name="bookResponse">The book response.</param>
        private void SetupReturnBookingErrors(iVectorConnectInterface.Basket.BookResponse bookResponse)
        {
            if (!bookResponse.ReturnStatus.Success)
            {
                foreach (BookResponse propertyBooking in bookResponse.PropertyBookings)
                {
                    if (!propertyBooking.ReturnStatus.Success)
                    {
                        this.bookReturn.Success = false;
                        this.bookReturn.Warnings.Add("unable to reserve hotel");
                        this.bookReturn.ComponentFailed = true;
                    }
                }

                foreach (iVectorConnectInterface.Flight.BookResponse flightBooking in bookResponse.FlightBookings)
                {
                    if (!flightBooking.ReturnStatus.Success)
                    {
                        this.bookReturn.Success = false;
                        this.bookReturn.Warnings.Add("unable to reserve Flight");
                        this.bookReturn.ComponentFailed = true;
                    }
                }

                foreach (iVectorConnectInterface.Transfer.BookResponse transferbooking in bookResponse.TransferBookings)
                {
                    if (!transferbooking.ReturnStatus.Success)
                    {
                        this.bookReturn.Success = false;
                        this.bookReturn.Warnings.Add("unable to reserve transfer");
                        this.bookReturn.ComponentFailed = true;
                    }
                }
            }
        }
    }
}