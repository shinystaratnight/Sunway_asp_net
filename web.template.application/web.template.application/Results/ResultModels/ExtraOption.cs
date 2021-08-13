namespace Web.Template.Application.Results.ResultModels
{
    using System;

    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Class ExtraOption.
    /// </summary>
    public class ExtraOption : ISubResult
    {
        /// <summary>
        /// Gets or sets the adult price.
        /// </summary>
        /// <value>The adult price.</value>
        public decimal AdultPrice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [age required].
        /// </summary>
        /// <value><c>true</c> if [age required]; otherwise, <c>false</c>.</value>
        public bool AgeRequired { get; set; }

        /// <summary>
        /// Gets or sets the booking token.
        /// </summary>
        /// <value>The booking token, the unique identifier connect uses for the result</value>
        public string BookingToken { get; set; }

        /// <summary>
        /// Gets or sets the child price.
        /// </summary>
        /// <value>The child price.</value>
        public decimal ChildPrice { get; set; }

        /// <summary>
        /// Gets or sets the component token.
        /// </summary>
        /// <value>The component token.</value>
        public int ComponentToken { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [date required].
        /// </summary>
        /// <value><c>true</c> if [date required]; otherwise, <c>false</c>.</value>
        public bool DateRequired { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>The duration.</value>
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>The end date.</value>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        /// <value>The end time.</value>
        public string EndTime { get; set; }

        /// <summary>
        /// Gets or sets the extra category.
        /// </summary>
        /// <value>The extra category.</value>
        public string ExtraCategory { get; set; }

        /// <summary>
        /// Gets or sets the extra category group.
        /// </summary>
        /// <value>The extra category group.</value>
        public string ExtraCategoryGroup { get; set; }

        /// <summary>
        /// Gets or sets the extra category identifier.
        /// </summary>
        /// <value>The extra category identifier.</value>
        public int ExtraCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the extra price.
        /// </summary>
        /// <value>The extra price.</value>
        public decimal ExtraPrice { get; set; }
        
        /// <summary>
        /// Gets or sets the infant price.
        /// </summary>
        /// <value>The infant price.</value>
        public decimal InfantPrice { get; set; }

        /// <summary>
        /// Gets or sets the maximum adults.
        /// </summary>
        /// <value>The maximum adults.</value>
        public int MaxAdults { get; set; }

        /// <summary>
        /// Gets or sets the maximum child age.
        /// </summary>
        /// <value>The maximum child age.</value>
        public int MaxChildAge { get; set; }

        /// <summary>
        /// Gets or sets the maximum children.
        /// </summary>
        /// <value>The maximum children.</value>
        public int MaxChildren { get; set; }

        /// <summary>
        /// Gets or sets the maximum age.
        /// </summary>
        /// <value>The maximum age.</value>
        public int MaximumAge { get; set; }

        /// <summary>
        /// Gets or sets the maximum quantity.
        /// </summary>
        /// <value>The maximum quantity.</value>
        public int MaximumQuantity { get; set; }

        /// <summary>
        /// Gets or sets the maximum passengers.
        /// </summary>
        /// <value>The maximum passengers.</value>
        public int MaxPassengers { get; set; }

        /// <summary>
        /// Gets or sets the minimum adults.
        /// </summary>
        /// <value>The minimum adults.</value>
        public int MinAdults { get; set; }

        /// <summary>
        /// Gets or sets the minimum child age.
        /// </summary>
        /// <value>The minimum child age.</value>
        public int MinChildAge { get; set; }

        /// <summary>
        /// Gets or sets the minimum children.
        /// </summary>
        /// <value>The minimum children.</value>
        public int MinChildren { get; set; }

        /// <summary>
        /// Gets or sets the minimum age.
        /// </summary>
        /// <value>The minimum age.</value>
        public int MinimumAge { get; set; }

        /// <summary>
        /// Gets or sets the minimum passengers.
        /// </summary>
        /// <value>The minimum passengers.</value>
        public int MinPassengers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [multi book].
        /// </summary>
        /// <value><c>true</c> if [multi book]; otherwise, <c>false</c>.</value>
        public bool MultiBook { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [occupancy rules].
        /// </summary>
        /// <value><c>true</c> if [occupancy rules]; otherwise, <c>false</c>.</value>
        public bool OccupancyRules { get; set; }

        /// <summary>
        /// Gets or sets the type of the pricing.
        /// </summary>
        /// <value>The type of the pricing.</value>
        public string PricingType { get; set; }

        /// <summary>
        /// Gets or sets the senior age.
        /// </summary>
        /// <value>The senior age.</value>
        public int SeniorAge { get; set; }

        /// <summary>
        /// Gets or sets the senior price.
        /// </summary>
        /// <value>The senior price.</value>
        public decimal SeniorPrice { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>The start time.</value>
        public string StartTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [time required].
        /// </summary>
        /// <value><c>true</c> if [time required]; otherwise, <c>false</c>.</value>
        public bool TimeRequired { get; set; }

        /// <summary>
        /// Gets or sets the total commission.
        /// </summary>
        /// <value>The total commission.</value>
        public decimal TotalCommission { get; set; }

        /// <summary>
        /// Gets or sets the total price.
        /// </summary>
        /// <value>The total price.</value>
        public decimal TotalPrice { get; set; }
    }
}