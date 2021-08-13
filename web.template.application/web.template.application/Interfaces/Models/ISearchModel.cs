namespace Web.Template.Application.Interfaces.Models
{
    using System;
    using System.Collections.Generic;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Search.SearchModels;

    /// <summary>
    ///     Search Model
    /// </summary>
    public interface ISearchModel
    {
        /// <summary>
        ///     Gets or sets The arrival identifier
        /// </summary>
        /// <value>
        ///     The arrival identifier.
        /// </value>
        int ArrivalID { get; set; }

        /// <summary>
        /// A list of secondary arrival ids for when a search needs extra information e.g. in an Airportgroup search we need the resort ids
        /// </summary>
        /// <value>
        /// The secondary arrival i ds.
        /// </value>
        List<int> SecondaryArrivalIDs { get; set; }

        /// <summary>
        ///     Gets or sets the arrival latitude.
        /// </summary>
        /// <value>
        ///     The arrival latitude.
        /// </value>
        decimal ArrivalLatitude { get; set; }

        /// <summary>
        ///     Gets or sets the arrival longitude.
        /// </summary>
        /// <value>
        ///     The arrival longitude.
        /// </value>
        decimal ArrivalLongitude { get; set; }

        /// <summary>
        ///     Gets or sets the arrival radius.
        /// </summary>
        /// <value>
        ///     The arrival radius.
        /// </value>
        decimal ArrivalRadius { get; set; }

        /// <summary>
        ///     Gets or sets  The arrival type
        /// </summary>
        /// <value>
        ///     The type of the arrival.
        /// </value>
        LocationType ArrivalType { get; set; }

        /// <summary>
        ///     Gets or sets  The departure date
        /// </summary>
        /// <value>
        ///     The departure date.
        /// </value>
        DateTime DepartureDate { get; set; }

        /// <summary>
        ///     Gets or sets  The departure identifier
        /// </summary>
        /// <value>
        ///     The departure identifier.
        /// </value>
        int DepartureID { get; set; }

        /// <summary>
        /// Gets or sets the departure time.
        /// </summary>
        /// <value>
        /// The departure time.
        /// </value>
        string DepartureTime { get; set; }

        /// <summary>
        ///     Gets or sets  The departure type
        /// </summary>
        /// <value>
        ///     The type of the departure.
        /// </value>
        LocationType DepartureType { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="SearchModel" /> is direct.
        /// </summary>
        /// <value>
        ///     <c>true</c> if direct; otherwise, <c>false</c>.
        /// </value>
        bool Direct { get; set; }

        /// <summary>
        ///     Gets or sets  The duration
        /// </summary>
        /// <value>
        ///     The duration.
        /// </value>
        int Duration { get; set; }

        /// <summary>
        /// Gets or sets the flight class identifier.
        /// </summary>
        /// <value>The flight class identifier.</value>
        int FlightClassId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is package search.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is package search; otherwise, <c>false</c>.
        /// </value>
        bool IsPackageSearch { get; set; }

        /// <summary>
        ///     Gets or sets The meal basis identifier
        /// </summary>
        /// <value>
        ///     The meal basis identifier.
        /// </value>
        int MealBasisID { get; set; }

        /// <summary>
        ///     Gets or sets  The minimum rating
        /// </summary>
        /// <value>
        ///     The minimum rating.
        /// </value>
        int MinRating { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [one way].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [one way]; otherwise, <c>false</c>.
        /// </value>
        bool OneWay { get; set; }

        /// <summary>
        /// Gets or sets the return time.
        /// </summary>
        /// <value>
        /// The return time.
        /// </value>
        string ReturnTime { get; set; }

        /// <summary>
        ///     Gets or sets the rooms.
        /// </summary>
        /// <value>
        ///     The rooms.
        /// </value>
        List<Room> Rooms { get; set; }

        /// <summary>
        ///     Gets or sets  The search mode
        /// </summary>
        /// <value>
        ///     The search mode.
        /// </value>
        SearchMode SearchMode { get; set; }
    }
}