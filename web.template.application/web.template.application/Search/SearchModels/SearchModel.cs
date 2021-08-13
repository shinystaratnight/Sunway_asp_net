namespace Web.Template.Application.Search.SearchModels
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    ///     Search model containing the details required to do a search.
    /// </summary>
    public class SearchModel : ISearchModel
    {
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
        [JsonConverter(typeof(StringEnumConverter))]
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
        [JsonConverter(typeof(StringEnumConverter))]
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
        /// <value>The flight class identifier.</value>
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
        ///     Gets or sets a value indicating whether the search is one way one way
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
        [JsonConverter(typeof(StringEnumConverter))]
        public SearchMode SearchMode { get; set; }

        /// <summary>
        ///     Gets or sets  The user token
        /// </summary>
        /// <value>
        ///     The user token.
        /// </value>
        public string UserToken { get; set; }
    }
}