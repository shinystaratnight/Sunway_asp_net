namespace Web.Template.Models.Application
{
    using System;
    using System.Collections.Generic;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Search.SearchModels;

    /// <summary>
    ///     Model containing the details required to carry out a search
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Models.ISearchModel" />
    public class SearchModel : ISearchModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchModel" /> class.
        /// </summary>
        /// <param name="arrivalID">The arrival identifier.</param>
        /// <param name="arrivalLatitude">The arrival latitude.</param>
        /// <param name="arrivalLongitude">The arrival longitude.</param>
        /// <param name="arrivalRadius">The arrival radius.</param>
        /// <param name="arrivalType">Type of the arrival.</param>
        /// <param name="departureDate">The departure date.</param>
        /// <param name="departureID">The departure identifier.</param>
        /// <param name="departureType">Type of the departure.</param>
        /// <param name="direct">if set to <c>true</c> [direct].</param>
        /// <param name="duration">The duration.</param>
        /// <param name="mealBasisID">The meal basis identifier.</param>
        /// <param name="minRating">The minimum rating.</param>
        /// <param name="oneWay">if set to <c>true</c> [one way].</param>
        /// <param name="rooms">The rooms.</param>
        /// <param name="searchMode">The search mode.</param>
        /// <param name="flightClassId">The flight class identifier.</param>
        /// <param name="departureTime">The departure time.</param>
        /// <param name="returnTime">The return time.</param>
        public SearchModel(
            int arrivalID, 
            decimal arrivalLatitude, 
            decimal arrivalLongitude, 
            decimal arrivalRadius, 
            LocationType arrivalType, 
            DateTime departureDate, 
            int departureID, 
            LocationType departureType, 
            bool direct, 
            int duration, 
            int mealBasisID, 
            int minRating, 
            bool oneWay, 
            List<Room> rooms, 
            SearchMode searchMode, 
            int flightClassId,
            string departureTime = "00:00", 
            string returnTime = "00:00")
        {
            this.ArrivalID = arrivalID;
            this.ArrivalLatitude = arrivalLatitude;
            this.ArrivalLongitude = arrivalLongitude;
            this.ArrivalRadius = arrivalRadius;
            this.ArrivalType = arrivalType;
            this.DepartureDate = departureDate;
            this.DepartureID = departureID;
            this.DepartureType = departureType;
            this.Direct = direct;
            this.Duration = duration;
            this.MealBasisID = mealBasisID;
            this.MinRating = minRating;
            this.OneWay = oneWay;
            this.Rooms = rooms;
            this.SearchMode = searchMode;
            this.FlightClassId = flightClassId;

            if (departureTime.Length == 4)
            {
                this.DepartureTime = $"{departureTime.Substring(0, 2)}:{departureTime.Substring(2, 2)}";
            }
            else
            {
                this.DepartureTime = departureTime;
            }

            if (returnTime.Length == 4)
            {
                this.ReturnTime = $"{returnTime.Substring(0, 2)}:{returnTime.Substring(2, 2)}";
            }
            else
            {
                this.ReturnTime = returnTime;
            }
        }

        /// <summary>
        ///     Gets or sets The arrival identifier
        /// </summary>
        /// <value>
        ///     The arrival identifier.
        /// </value>
        public int ArrivalID { get; set; }

        /// <summary>
        /// A list of secondary arrival ids for when a search needs extra information e.g. in an Airportgroup search we need the resort ids
        /// </summary>
        /// <value>
        /// The secondary arrival i ds.
        /// </value>
        public List<int> SecondaryArrivalIDs { get; set; }

        /// <summary>
        ///     Gets or sets the arrival latitude.
        /// </summary>
        /// <value>
        ///     The arrival latitude.
        /// </value>
        public decimal ArrivalLatitude { get; set; }

        /// <summary>
        ///     Gets or sets the arrival longitude.
        /// </summary>
        /// <value>
        ///     The arrival longitude.
        /// </value>
        public decimal ArrivalLongitude { get; set; }

        /// <summary>
        ///     Gets or sets the arrival radius.
        /// </summary>
        /// <value>
        ///     The arrival radius.
        /// </value>
        public decimal ArrivalRadius { get; set; }

        /// <summary>
        ///     Gets or sets  The arrival type
        /// </summary>
        /// <value>
        ///     The type of the arrival.
        /// </value>
        public LocationType ArrivalType { get; set; }

        /// <summary>
        ///     Gets or sets  The departure date
        /// </summary>
        /// <value>
        ///     The departure date.
        /// </value>
        public DateTime DepartureDate { get; set; }

        /// <summary>
        ///     Gets or sets  The departure identifier
        /// </summary>
        /// <value>
        ///     The departure identifier.
        /// </value>
        public int DepartureID { get; set; }

        /// <summary>
        /// Gets or sets the departure time.
        /// </summary>
        /// <value>
        /// The departure time.
        /// </value>
        public string DepartureTime { get; set; }

        /// <summary>
        ///     Gets or sets  The departure type
        /// </summary>
        /// <value>
        ///     The type of the departure.
        /// </value>
        public LocationType DepartureType { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="SearchModel" /> is direct.
        /// </summary>
        /// <value>
        ///     <c>true</c> if direct; otherwise, <c>false</c>.
        /// </value>
        public bool Direct { get; set; }

        /// <summary>
        ///     Gets or sets  The duration
        /// </summary>
        /// <value>
        ///     The duration.
        /// </value>
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the flight class identifier.
        /// </summary>
        /// <value>
        /// The flight class identifier.
        /// </value>
        public int FlightClassId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is package search.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is package search; otherwise, <c>false</c>.
        /// </value>
        public bool IsPackageSearch { get; set; }

        /// <summary>
        ///     Gets or sets The meal basis identifier
        /// </summary>
        /// <value>
        ///     The meal basis identifier.
        /// </value>
        public int MealBasisID { get; set; }

        /// <summary>
        ///     Gets or sets  The minimum rating
        /// </summary>
        /// <value>
        ///     The minimum rating.
        /// </value>
        public int MinRating { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [one way].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [one way]; otherwise, <c>false</c>.
        /// </value>
        public bool OneWay { get; set; }

        /// <summary>
        /// Gets or sets the return time.
        /// </summary>
        /// <value>
        /// The return time.
        /// </value>
        public string ReturnTime { get; set; }

        /// <summary>
        ///     Gets or sets the rooms.
        /// </summary>
        /// <value>
        ///     The rooms.
        /// </value>
        public List<Room> Rooms { get; set; }

        /// <summary>
        ///     Gets or sets  The search mode
        /// </summary>
        /// <value>
        ///     The search mode.
        /// </value>
        public SearchMode SearchMode { get; set; }
    }
}