namespace Web.Template.Application.Basket.Models.Components
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Base class used for basket components
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Models.IBasketComponent" />
    public abstract class BasketCompontentBase : IBasketComponent
    {
        /// <summary>
        /// Gets or sets the adults.
        /// </summary>
        /// <value>
        /// The adults.
        /// </value>
        public int Adults { get; set; }

        /// <summary>
        /// Gets or sets the arrival date.
        /// </summary>
        /// <value>
        /// The arrival date.
        /// </value>
        public DateTime ArrivalDate { get; set; }

        /// <summary>
        /// Gets or sets the basket token.
        /// </summary>
        /// <value>The basket token.</value>
        public string BasketToken { get; set; }

        /// <summary>
        /// Gets or sets the booking token.
        /// </summary>
        /// <value>
        /// The booking token.
        /// </value>
        public string BookingToken { get; set; }

        /// <summary>
        /// Gets or sets the cancellation.
        /// </summary>
        /// <value>
        /// The cancellation.
        /// </value>
        public List<ICancellationCharge> CancellationCharges { get; set; }

        /// <summary>
        /// Gets or sets the child ages.
        /// </summary>
        /// <value>
        /// The child ages.
        /// </value>
        public List<int> ChildAges { get; set; }

        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public int Children { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [component booked].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [component booked]; otherwise, <c>false</c>.
        /// </value>
        public bool ComponentBooked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [component pre booked].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [component pre booked]; otherwise, <c>false</c>.
        /// </value>
        public bool ComponentPreBooked { get; set; }

        /// <summary>
        /// Gets or sets the component token.
        /// </summary>
        /// <value>
        /// The component token.
        /// </value>
        public int ComponentToken { get; set; }

        /// <summary>
        /// Gets the type of the component.
        /// </summary>
        /// <value>
        /// The type of the component.
        /// </value>
        [JsonConverter(typeof(StringEnumConverter))]
        public virtual ComponentType ComponentType { get; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the guest i ds.
        /// </summary>
        /// <value>
        /// The guest i ds.
        /// </value>
        public List<int> GuestIDs { get; set; }

        /// <summary>
        /// Gets or sets the infants.
        /// </summary>
        /// <value>
        /// The infants.
        /// </value>
        public int Infants { get; set; }

        /// <summary>
        /// Gets or sets the payments.
        /// </summary>
        /// <value>
        /// The payments.
        /// </value>
        public List<IPayment> Payments { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        /// <value>
        /// The price.
        /// </value>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the search mode.
        /// </summary>
        /// <value>
        /// The search mode.
        /// </value>
        public SearchMode SearchMode { get; set; }

        /// <summary>
        /// Gets or sets the search token.
        /// </summary>
        /// <value>The search token.</value>
        public string SearchToken { get; set; }

        /// <summary>
        /// Gets or sets the sub components.
        /// </summary>
        /// <value>
        /// The sub components.
        /// </value>
        public List<ISubComponent> SubComponents { get; set; }

        /// <summary>
        /// Gets or sets the terms and conditions.
        /// </summary>
        /// <value>
        /// The terms and conditions.
        /// </value>
        public string TermsAndConditions { get; set; }

        /// <summary>
        /// Gets or sets the terms and conditions URL.
        /// </summary>
        /// <value>
        /// The terms and conditions URL.
        /// </value>
        public string TermsAndConditionsUrl { get; set; }

        /// <summary>
        /// Gets or sets the commission.
        /// </summary>
        /// <value>
        /// The commission.
        /// </value>
        public decimal TotalCommission { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        /// <value>
        /// The price.
        /// </value>
        public abstract decimal TotalPrice { get; set; }

        /// <summary>
        /// Gets or sets the vat on commission.
        /// </summary>
        /// <value>The vat on commission.</value>
        public decimal VATOnCommission { get; set; }

        /// <summary>
        /// Gets or sets the setup component search details.
        /// </summary>
        /// <param name="searchModel">The search model.</param>
        /// <value>
        /// The setup component search details.
        /// </value>
        public virtual void SetupComponentSearchDetails(ISearchModel searchModel)
        {
            this.ArrivalDate = searchModel.DepartureDate;
            this.Duration = searchModel.Duration;
            this.Adults = searchModel.Rooms.Sum(s => s.Adults);
            this.Children = searchModel.Rooms.Sum(s => s.Children);
            this.Infants = searchModel.Rooms.Sum(s => s.Infants);
            this.ChildAges = searchModel.Rooms.SelectMany(r => r.ChildAges).ToList();
        }

        /// <summary>
        /// Setups the component extra search details.
        /// </summary>
        /// <param name="extraSearchModel">The extra search model.</param>
        public virtual void SetupComponentExtraSearchDetails(IExtraSearchModel extraSearchModel)
        {
            this.Adults = extraSearchModel.Adults;
            this.Children = extraSearchModel.Children;
            this.Infants = extraSearchModel.Infants;
            this.ChildAges = extraSearchModel.ChildAges;

            this.TotalPrice = this.SubComponents.Sum(s => s.TotalPrice);
        }

        /// <summary>
        /// Setups the meta data.
        /// </summary>
        /// <param name="metaData">The meta data.</param>
        public virtual void SetupMetaData(Dictionary<string, string> metaData)
        {
        }
    }
}