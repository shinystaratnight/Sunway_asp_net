namespace Web.Template.Data.Lookup.Repositories.CMS.Flight
{
    using System.Collections.Generic;
    using System.Xml;

    using Web.Template.Data.Connect;
    using Web.Template.Domain.Entities.Flight;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Flight;

    /// <summary>
    /// Class CMSFlightCacheRouteRepository.
    /// </summary>
    public class CMSFlightCacheRouteRepository : CMSLookupBase<FlightCacheRoute>, IFlightCacheRouteRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CMSFlightCacheRouteRepository" /> class.
        /// </summary>
        /// <param name="asyncLookup">The asynchronous lookup.</param>
        public CMSFlightCacheRouteRepository(IAsyncLookup asyncLookup)
            : base(asyncLookup)
        {
        }

        /// <summary>
        /// Returns all members.
        /// </summary>
        /// <returns>
        /// All items in the repository
        /// </returns>
        public IEnumerable<FlightCacheRoute> GetAll() => base.GetAll();

        /// <summary>
        /// Setups this instance.
        /// </summary>
        /// <returns>A list of flight cache routes</returns>
        protected override List<FlightCacheRoute> Setup()
        {
            XmlDocument xml = this.GetLookupsXml("FlightCacheRoutes", 1);
            var flightCacheRoutes = Utillity.XMLFunctions.XMLToGenericList<FlightCacheRoute>(xml);
            return flightCacheRoutes;
        }
    }
}