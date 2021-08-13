namespace Web.Template.Data.Lookup.Repositories.Geography
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    using Web.Template.Data.Lookup.Repositories.Generic;
    using Web.Template.Domain.Entities.Geography;
    using Web.Template.Domain.Interfaces.Lookup.Repositories.Geography;

    /// <summary>
    /// Repository that is responsible for managing access to country data
    /// </summary>
    /// <seealso cref="Country" />
    /// <seealso cref="ICountryRepository" />
    public class CountryRepository : LookupRepository<Country>, ICountryRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CountryRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public CountryRepository(DbContext dbContext)
            : base(dbContext)
        {
        }

        /// <summary>
        /// Gets the countries with regions and resort.
        /// </summary>
        /// <returns>All countries with their regions and resorts filled in.</returns>
        public IEnumerable<Country> GetCountriesWithRegionsAndResort()
        {
            return this.GetAll(country => country.Include(c => c.Regions.Select(region => region.Resorts)));
        }

        /// <summary>
        /// Gets the country with regions and resorts.
        /// </summary>
        /// <param name="countryId">The country identifier.</param>
        /// <returns>A Country with its regions and resorts filled in.</returns>
        public Country GetCountryWithRegionsAndResorts(int countryId)
        {
            return this.GetSingle(
                countryId, 
                country => country.Include(c => c.Regions.Select(region => region.Resorts)));
        }
    }
}