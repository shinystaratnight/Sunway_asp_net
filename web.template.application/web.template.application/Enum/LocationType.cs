namespace Web.Template.Application.Enum
{
    /// <summary>
    ///     Location types are the types of locations we can search to and from
    /// </summary>
    public enum LocationType
    {
        /// <summary>
        ///     The default value, will only be this if it has not been set
        /// </summary>
        None = 0, 

        /// <summary>
        ///     The airport
        /// </summary>
        Airport, 

        /// <summary>
        ///     The airport group
        /// </summary>
        AirportGroup, 

        /// <summary>
        ///     The port
        /// </summary>
        Port, 

        /// <summary>
        ///     The resort
        /// </summary>
        Resort, 

        /// <summary>
        ///     The region
        /// </summary>
        Region, 

        /// <summary>
        ///     The country
        /// </summary>
        Country, 

        /// <summary>
        ///     Property Reference ID
        /// </summary>
        Property, 

        /// <summary>
        ///     Geo code (requires longitude, latitude and radius)
        /// </summary>
        GeoCode, 

        /// <summary>
        ///     geography Group
        /// </summary>
        GeographyGroup
    }
}