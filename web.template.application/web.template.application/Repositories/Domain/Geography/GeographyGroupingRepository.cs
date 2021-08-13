namespace Web.Template.Application.Repositories.Domain.Geography
{
    using System.Collections.Generic;

    using Web.Template.Application.IVectorConnect.Lookups.Factories;
    using Web.Template.Domain.Entities.Geography;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Geography;

    /// <summary>
    ///     A repository used to look up GeographyGrouping, this instance uses connect async lookups to populate it.
    /// </summary>
    /// <seealso cref="IGeographyGroupingRepository" />
    public class GeographyGroupingRepository : IGeographyGroupingRepository
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GeographyGroupingRepository" /> class.
        /// </summary>
        /// <param name="geographyGroupFactory">The geography group factory.</param>
        public GeographyGroupingRepository(GeographyGroupFactory geographyGroupFactory)
        {
            using (geographyGroupFactory)
            {
                ////    this.GeographyGroupings = geographyGroupFactory.Create();
            }
        }

        /// <summary>
        ///     Gets The geography groupings
        /// </summary>
        /// <value>
        ///     The geography groupings.
        /// </value>
        private List<GeographyGrouping> GeographyGroupings { get; }

        /// <summary>
        ///     Gets the geography by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        ///     A geography grouping
        /// </returns>
        public GeographyGrouping GetGeographyGroupById(int id)
        {
            foreach (GeographyGrouping gg in this.GeographyGroupings)
            {
                if (gg.Id == id)
                {
                    return gg;
                }
            }

            return null;
        }
    }
}