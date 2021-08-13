namespace Web.Template.Application.Prebook.Builders
{
    using System.Collections.Generic;

    using iVectorConnectInterface.Property;

    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Prebook;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Builds a prebook return
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Prebook.IPrebookReturnBuilder" />
    public class PrebookReturnBuilder : IPrebookReturnBuilder
    {
        /// <summary>
        /// The prebook return
        /// </summary>
        private readonly IPrebookReturn prebookReturn;

        /// <summary>
        /// The post prebook amount
        /// </summary>
        private decimal postPrebookAmount;

        /// <summary>
        /// The pre prebook amount
        /// </summary>
        private decimal prePrebookAmount;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrebookReturnBuilder"/> class.
        /// </summary>
        /// <param name="prebookReturn">The prebook return.</param>
        public PrebookReturnBuilder(IPrebookReturn prebookReturn)
        {
            this.prebookReturn = prebookReturn;
            this.prebookReturn.Warnings = new List<string>();
            this.prebookReturn.Success = true;
        }

        /// <summary>
        /// Gets a value indicating whether [currently successful].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [currently successful]; otherwise, <c>false</c>.
        /// </value>
        public bool CurrentlySuccessful => this.prebookReturn.Success;

        /// <summary>
        /// Adds the response.
        /// </summary>
        /// <param name="preBookResponse">The pre book response.</param>
        public void AddResponse(iVectorConnectInterface.Basket.PreBookResponse preBookResponse)
        {
            this.SetupReturnBookingErrors(preBookResponse);
        }

        /// <summary>
        /// Adds the warning.
        /// </summary>
        /// <param name="warning">The warning.</param>
        public void AddWarning(string warning)
        {
            this.prebookReturn.Warnings.Add(warning);
            this.prebookReturn.Success = false;
        }

        /// <summary>
        /// Adds the warnings.
        /// </summary>
        /// <param name="warnings">The warnings.</param>
        public void AddWarnings(List<string> warnings)
        {
            this.prebookReturn.Warnings.AddRange(warnings);
            if (warnings.Count > 0)
            {
                this.prebookReturn.Success = false;
            }
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns>
        /// A prebook return
        /// </returns>
        public IPrebookReturn Build()
        {
            this.prebookReturn.PriceChange = this.postPrebookAmount - this.prePrebookAmount;
            return this.prebookReturn;
        }

        /// <summary>
        /// Sets the basket.
        /// </summary>
        /// <param name="basket">The basket.</param>
        public void SetBasket(IBasket basket)
        {
            this.prebookReturn.Basket = basket;
        }

        /// <summary>
        /// Sets the post pre book price.
        /// </summary>
        /// <param name="price">The price.</param>
        public void SetPostPreBookPrice(decimal price)
        {
            this.postPrebookAmount = price;
        }

        /// <summary>
        /// Sets the pre pre book price.
        /// </summary>
        /// <param name="price">The price.</param>
        public void SetPrePreBookPrice(decimal price)
        {
            this.prePrebookAmount = price;
        }

        /// <summary>
        /// Sets to failure.
        /// </summary>
        public void SetToFailure()
        {
            this.prebookReturn.Success = false;
        }

        /// <summary>
        /// Checks the bookings for errors.
        /// </summary>
        /// <param name="preBookResponse">The pre book response.</param>
        private void SetupReturnBookingErrors(iVectorConnectInterface.Basket.PreBookResponse preBookResponse)
        {
            foreach (PreBookResponse propertyBooking in preBookResponse.PropertyBookings)
            {
                if (!propertyBooking.ReturnStatus.Success)
                {
                    this.prebookReturn.Success = false;
                    this.prebookReturn.Warnings.Add("unable to reserve hotel");
                }
            }

            foreach (iVectorConnectInterface.Flight.PreBookResponse flightBooking in preBookResponse.FlightBookings)
            {
                if (!flightBooking.ReturnStatus.Success)
                {
                    this.prebookReturn.Success = false;
                    this.prebookReturn.Warnings.Add("unable to reserve Flight");
                }
            }

            foreach (iVectorConnectInterface.Transfer.PreBookResponse transferbooking in preBookResponse.TransferBookings)
            {
                if (!transferbooking.ReturnStatus.Success)
                {
                    this.prebookReturn.Success = false;
                    this.prebookReturn.Warnings.Add("unable to reserve transfer");
                }
            }
        }
    }
}