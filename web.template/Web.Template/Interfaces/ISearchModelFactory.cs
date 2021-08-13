namespace Web.Template.Interfaces
{
    using System;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    ///     A factory that builds up a search model with the specified values
    /// </summary>
    public interface ISearchModelFactory
    {
        /// <summary>
        /// Creates the specified arrival identifier.
        /// </summary>
        /// <param name="arrivalId">The arrival identifier.</param>
        /// <param name="arrivalType">Type of the arrival.</param>
        /// <param name="departureDate">The departure date.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="searchMode">The search mode.</param>
        /// <param name="rooms">The rooms.</param>
        /// <param name="adults">The adults.</param>
        /// <param name="children">The children.</param>
        /// <param name="infants">The infants.</param>
        /// <param name="childages">The child ages.</param>
        /// <param name="departureId">The departure identifier.</param>
        /// <param name="departureType">Type of the departure.</param>
        /// <param name="direct">if set to <c>true</c> [direct].</param>
        /// <param name="mealBasisId">The meal basis identifier.</param>
        /// <param name="minRating">The minimum rating.</param>
        /// <param name="oneWay">if set to <c>true</c> [one way].</param>
        /// <param name="arrivalLongitude">The arrival longitude.</param>
        /// <param name="arrivalRadius">The arrival radius.</param>
        /// <param name="arrivalLatitude">The arrival latitude.</param>
        /// <param name="flightClassId">The flight class identifier.</param>
        /// <returns>A search model populated with the specified values</returns>
        ISearchModel Create(
            int arrivalId, 
            LocationType arrivalType, 
            DateTime departureDate, 
            int duration, 
            SearchMode searchMode, 
            int rooms, 
            string adults, 
            string children, 
            string infants, 
            string childages, 
            int departureId = 0, 
            LocationType departureType = LocationType.None, 
            bool direct = false, 
            int mealBasisId = 0, 
            int minRating = 0, 
            bool oneWay = false, 
            decimal arrivalLongitude = 0, 
            decimal arrivalRadius = 0, 
            decimal arrivalLatitude = 0,
            int flightClassId = 0);

        /// <summary>
        /// Creates the specified arrival identifier.
        /// </summary>
        /// <param name="arrivalId">The arrival identifier.</param>
        /// <param name="arrivalType">Type of the arrival.</param>
        /// <param name="departureDate">The departure date.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="searchMode">The search mode.</param>
        /// <param name="adults">The adults.</param>
        /// <param name="children">The children.</param>
        /// <param name="infants">The infants.</param>
        /// <param name="childages">The child ages.</param>
        /// <param name="departureId">The departure identifier.</param>
        /// <param name="departureType">Type of the departure.</param>
        /// <param name="direct">if set to <c>true</c> [direct].</param>
        /// <param name="mealBasisId">The meal basis identifier.</param>
        /// <param name="minRating">The minimum rating.</param>
        /// <param name="oneWay">if set to <c>true</c> [one way].</param>
        /// <param name="arrivalLongitude">The arrival longitude.</param>
        /// <param name="arrivalRadius">The arrival radius.</param>
        /// <param name="arrivalLatitude">The arrival latitude.</param>
        /// <param name="departureTime">The departure time.</param>
        /// <param name="returnTime">The return time.</param>
        /// <param name="flightClassId">The flight class identifier.</param>
        /// <returns>Creates a search model</returns>
        ISearchModel Create(
            int arrivalId, 
            LocationType arrivalType, 
            DateTime departureDate, 
            int duration, 
            SearchMode searchMode, 
            int adults, 
            int children, 
            int infants, 
            string childages, 
            int departureId = 0, 
            LocationType departureType = LocationType.None, 
            bool direct = false, 
            int mealBasisId = 0, 
            int minRating = 0, 
            bool oneWay = false, 
            decimal arrivalLongitude = 0, 
            decimal arrivalRadius = 0, 
            decimal arrivalLatitude = 0, 
            string departureTime = "00:00", 
            string returnTime = "00:00",
            int flightClassId = 0);
    }
}