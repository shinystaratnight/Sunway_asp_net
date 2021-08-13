namespace Web.Template.Data.Lookup.Repositories.Geography
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Geography;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Geography;

    /// <summary>
    /// Repository responsible for accessing relating relating to regions.
    /// </summary>
    /// <seealso cref="Web.Template.Data.Repositories.LookupRepository{Web.Template.Domain.Entities.Geography.Region}" />
    /// <seealso cref="IRegionRepository" />
    public class RegionRepository : LookupRepository<Region>, IRegionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegionRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public RegionRepository(DbContext dbContext)
            : base(dbContext)
        {
        }

        /// <summary>
        /// Gets the regions with resorts.
        /// </summary>
        /// <returns>All Regions and their resorts included</returns>
        public IEnumerable<Region> GetRegionsWithResorts()
        {
            var regions = this.GetAll(region => region.Include(r => r.Resorts));

            if (regions == null)
            {
                throw new Exception("Region not found");
            }

            return regions;
        }

        /// <summary>
        /// Gets the region with resort.
        /// </summary>
        /// <param name="regionId">The region identifier.</param>
        /// <returns>A region with its resorts included</returns>
        public Region GetRegionWithResorts(int regionId)
        {
            var region = this.DbSet.Include(r => r.Resorts).FirstOrDefault(r => r.Id == regionId);

            if (region == null)
            {
                throw new Exception("Region not found");
            }

            return region;
        }
    }
}