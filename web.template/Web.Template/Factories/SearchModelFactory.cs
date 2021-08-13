namespace Web.Template.Factories
{
    using System;
    using System.Collections.Generic;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Search.SearchModels;
    using Web.Template.Interfaces;

    using SearchModel = Web.Template.Models.Application.SearchModel;

    /// <summary>
    ///     Factory class for producing search models
    /// </summary>
    /// <seealso cref="Web.Template.Interfaces.ISearchModelFactory" />
    public class SearchModelFactory : ISearchModelFactory
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
        /// <param name="childAges">The child ages.</param>
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
        public ISearchModel Create(
            int arrivalId, 
            LocationType arrivalType, 
            DateTime departureDate, 
            int duration, 
            SearchMode searchMode, 
            int rooms, 
            string adults, 
            string children, 
            string infants, 
            string childAges, 
            int departureId = 0, 
            LocationType departureType = LocationType.None, 
            bool direct = false, 
            int mealBasisId = 0, 
            int minRating = 0, 
            bool oneWay = false, 
            decimal arrivalLongitude = 0, 
            decimal arrivalRadius = 0, 
            decimal arrivalLatitude = 0,
            int flightClassId = 0)
        {
            List<Room> roomsList = this.BuildRoomList(rooms, adults, children, infants, childAges);

            return new SearchModel(arrivalId, arrivalLatitude, arrivalLongitude, arrivalRadius, arrivalType, departureDate, departureId, departureType, direct, duration, mealBasisId, minRating, oneWay, roomsList, searchMode, flightClassId);
        }

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
        /// <param name="childAges">The child ages.</param>
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
        /// <returns>A search model populated by the specified values</returns>
        public ISearchModel Create(
            int arrivalId, 
            LocationType arrivalType, 
            DateTime departureDate, 
            int duration, 
            SearchMode searchMode, 
            int adults, 
            int children, 
            int infants, 
            string childAges, 
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
            int flightClassId = 0)
        {
            var roomsList = new List<Room>();
            var room = new Room { Adults = adults, Children = children, Infants = infants, ChildAges = new List<int>() };
            this.SplitChildAgeString(childAges, room);
            roomsList.Add(room);

            return new SearchModel(
                arrivalId, 
                arrivalLatitude, 
                arrivalLongitude, 
                arrivalRadius, 
                arrivalType, 
                departureDate, 
                departureId, 
                departureType, 
                direct, 
                duration, 
                mealBasisId, 
                minRating, 
                oneWay, 
                roomsList, 
                searchMode, 
                flightClassId,
                departureTime, 
                returnTime);
        }

        /// <summary>
        ///     The url will contain multiples rooms of content in the format {room1}_{room2}_{room3} so for example if there are 3
        ///     rooms each with
        ///     two adults it might look like 2_2_2 we need to split this out and build up rooms from this.
        /// </summary>
        /// <param name="rooms">The rooms.</param>
        /// <param name="adults">The adults.</param>
        /// <param name="children">The children.</param>
        /// <param name="infants">The infants.</param>
        /// <param name="childAges">The child ages.</param>
        /// <returns>A list of rooms populated from the specified values</returns>
        private List<Room> BuildRoomList(int rooms, string adults, string children, string infants, string childAges)
        {
            var roomsList = new List<Room>();

            for (var i = 0; i < rooms; i++)
            {
                var room = new Room { ChildAges = new List<int>() };

                int totalAdults = this.SplitStringByDeliminatorAndGetValueAtIndex(adults, i, '_');
                int totalChildren = this.SplitStringByDeliminatorAndGetValueAtIndex(children, i, '_');
                int totalInfants = this.SplitStringByDeliminatorAndGetValueAtIndex(infants, i, '_');

                string[] childAgeArray = childAges?.Split('_');
                if (childAgeArray?.Length > i)
                {
                    this.SplitChildAgeString(childAgeArray[i], room);
                }

                room.Adults = totalAdults;
                room.Children = totalChildren;
                room.Infants = totalInfants;

                roomsList.Add(room);
            }

            return roomsList;
        }

        /// <summary>
        ///     Splits the child age string.
        /// </summary>
        /// <param name="childAges">The child ages.</param>
        /// <param name="room">The room.</param>
        private void SplitChildAgeString(string childAges, Room room)
        {
            foreach (string childAgeList in childAges.Split('-'))
            {
                if (childAgeList == "0")
                {
                    continue;
                }

                int age;
                if (int.TryParse(childAgeList, out age))
                {
                    room.ChildAges.Add(age);
                }
            }
        }

        /// <summary>
        ///     Passenger string is an array split on _
        /// </summary>
        /// <param name="passengerString">The passenger string.</param>
        /// <param name="index">The index.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns>
        ///     An integer
        /// </returns>
        private int SplitStringByDeliminatorAndGetValueAtIndex(string passengerString, int index, char delimiter)
        {
            var passengerTotal = 0;
            string[] adultsArray = passengerString?.Split(delimiter);
            if (adultsArray?.Length > index)
            {
                int.TryParse(adultsArray[index], out passengerTotal);
            }

            return passengerTotal;
        }
    }
}