namespace Web.Template.Data.Context
{
    using System.Data.Entity;

    using Web.Template.Domain.Entities.Booking;
    using Web.Template.Domain.Entities.Flight;
    using Web.Template.Domain.Entities.Geography;
    using Web.Template.Domain.Entities.Payment;
    using Web.Template.Domain.Entities.Property;

    /// <summary>
    /// Database context for lookups.
    /// </summary>
    /// <seealso cref="System.Data.Entity.DbContext" />
    public class LookupContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LookupContext"/> class.
        /// </summary>
        public LookupContext()
            : base("name=LookupContext")
        {
        }

        /// <summary>
        /// Gets or sets the airport groups.
        /// </summary>
        /// <value>
        /// The airport groups.
        /// </value>
        public virtual DbSet<AirportGroup> AirportGroups { get; set; }

        /// <summary>
        /// Gets or sets the airport lookup collection
        /// </summary>
        /// <value>
        /// The airports.
        /// </value>
        public virtual DbSet<Airport> Airports { get; set; }

        /// <summary>
        /// Gets or sets the airport terminals.
        /// </summary>
        /// <value>
        /// The airport terminals.
        /// </value>
        public virtual DbSet<AirportTerminal> AirportTerminals { get; set; }

        /// <summary>
        /// Gets or sets the booking countries.
        /// </summary>
        /// <value>
        /// The booking countries.
        /// </value>
        public virtual DbSet<BookingCountry> BookingCountries { get; set; }

        /// <summary>
        /// Gets or sets the booking documentations.
        /// </summary>
        /// <value>
        /// The booking documentations.
        /// </value>
        public virtual DbSet<BookingDocumentation> BookingDocumentations { get; set; }

        /// <summary>
        /// Gets or sets the brands.
        /// </summary>
        /// <value>
        /// The brands.
        /// </value>
        public virtual DbSet<Brand> Brands { get; set; }

        /// <summary>
        /// Gets or sets the countries.
        /// </summary>
        /// <value>
        /// The countries.
        /// </value>
        public virtual DbSet<Country> Countries { get; set; }

        /// <summary>
        /// Gets or sets the credit card surcharges.
        /// </summary>
        /// <value>
        /// The credit card surcharges.
        /// </value>
        public virtual DbSet<CreditCardSurcharge> CreditCardSurcharges { get; set; }

        /// <summary>
        /// Gets or sets the credit card types.
        /// </summary>
        /// <value>
        /// The credit card types.
        /// </value>
        public virtual DbSet<CreditCardType> CreditCardTypes { get; set; }

        /// <summary>
        /// Gets or sets the currencies.
        /// </summary>
        /// <value>
        /// The currencies.
        /// </value>
        public virtual DbSet<Currency> Currencies { get; set; }

        /// <summary>
        /// Gets or sets the exchange rates.
        /// </summary>
        /// <value>
        /// The exchange rates.
        /// </value>
        public virtual DbSet<ExchangeRate> ExchangeRates { get; set; }

        /// <summary>
        /// Gets or sets the filter facilities.
        /// </summary>
        /// <value>
        /// The filter facilities.
        /// </value>
        public virtual DbSet<FilterFacility> FilterFacilities { get; set; }

        /// <summary>
        /// Gets or sets the flight carrier lookup collection.
        /// </summary>
        /// <value>
        /// The flight carriers.
        /// </value>
        public virtual DbSet<FlightCarrier> FlightCarriers { get; set; }

        /// <summary>
        /// Gets or sets the flight classes.
        /// </summary>
        /// <value>
        /// The flight classes.
        /// </value>
        public virtual DbSet<FlightClass> FlightClasses { get; set; }

        /// <summary>
        /// Gets or sets the landmarks.
        /// </summary>
        /// <value>
        /// The landmarks.
        /// </value>
        public virtual DbSet<Landmark> Landmarks { get; set; }

        /// <summary>
        /// Gets or sets the languages.
        /// </summary>
        /// <value>
        /// The languages.
        /// </value>
        public virtual DbSet<Language> Languages { get; set; }

        /// <summary>
        /// Gets or sets the marketing codes.
        /// </summary>
        /// <value>
        /// The marketing codes.
        /// </value>
        public virtual DbSet<MarketingCode> MarketingCodes { get; set; }

        /// <summary>
        /// Gets or sets the meal basis.
        /// </summary>
        /// <value>
        /// The meal basis.
        /// </value>
        public virtual DbSet<MealBasis> MealBasis { get; set; }

        /// <summary>
        /// Gets or sets the nationalities.
        /// </summary>
        /// <value>
        /// The nationalities.
        /// </value>
        public virtual DbSet<Nationality> Nationalities { get; set; }

        /// <summary>
        /// Gets or sets the ports.
        /// </summary>
        /// <value>
        /// The ports.
        /// </value>
        public virtual DbSet<Port> Ports { get; set; }

        /// <summary>
        /// Gets or sets the product attributes.
        /// </summary>
        /// <value>
        /// The product attributes.
        /// </value>
        public virtual DbSet<ProductAttribute> ProductAttributes { get; set; }

        /// <summary>
        /// Gets or sets the property references.
        /// </summary>
        /// <value>
        /// The property references.
        /// </value>
        public virtual DbSet<PropertyReference> PropertyReferences { get; set; }

        /// <summary>
        /// Gets or sets the regions.
        /// </summary>
        /// <value>
        /// The regions.
        /// </value>
        public virtual DbSet<Region> Regions { get; set; }

        /// <summary>
        /// Gets or sets the resorts.
        /// </summary>
        /// <value>
        /// The resorts.
        /// </value>
        public virtual DbSet<Resort> Resorts { get; set; }

        /// <summary>
        /// Gets or sets the route availabilities.
        /// </summary>
        /// <value>
        /// The route availabilities.
        /// </value>
        public virtual DbSet<RouteAvailability> RouteAvailabilities { get; set; }

        /// <summary>
        /// Gets or sets the sales channels.
        /// </summary>
        /// <value>
        /// The sales channels.
        /// </value>
        public virtual DbSet<SalesChannel> SalesChannels { get; set; }

        /// <summary>
        /// Gets or sets the trade contacts.
        /// </summary>
        /// <value>
        /// The trade contacts.
        /// </value>
        public virtual DbSet<TradeContactGroup> TradeContactGroups { get; set; }

        /// <summary>
        /// Gets or sets the trade contacts.
        /// </summary>
        /// <value>
        /// The trade contacts.
        /// </value>
        public virtual DbSet<TradeContact> TradeContacts { get; set; }

        /// <summary>
        /// Gets or sets the trade contacts.
        /// </summary>
        /// <value>
        /// The trade contacts.
        /// </value>
        public virtual DbSet<Trade> Trades { get; set; }

        /// <summary>
        /// This method is called when the model for a derived context has been initialized, but
        /// before the model has been locked down and used to initialize the context.  The default
        /// implementation of this method does nothing, but it can be overridden in a derived class
        /// such that the model can be further configured before it is locked down.
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created.</param>
        /// <remarks>
        /// Typically, this method is called only once when the first instance of a derived context
        /// is created.  The model for that context is then cached and is for all further instances of
        /// the context in the app domain.  This caching can be disabled by setting the ModelCaching
        /// property on the given ModelBuilder, but note that this can seriously degrade performance.
        /// More control over caching is provided through use of the ModelBuilder and ContextFactory
        /// classes directly.
        /// </remarks>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Brand>().Property(e => e.BrandCode).IsUnicode(false);

            modelBuilder.Entity<Brand>().Property(e => e.Name).IsUnicode(false);
            modelBuilder.Entity<CreditCardSurcharge>().Property(e => e.SurchargeType).IsUnicode(false);

            modelBuilder.Entity<CreditCardSurcharge>().Property(e => e.SurchargePercentage).HasPrecision(14, 2);

            modelBuilder.Entity<FlightClass>().Property(e => e.Name).IsUnicode(false);

            modelBuilder.Entity<Country>().Property(e => e.Code).IsUnicode(false);

            modelBuilder.Entity<Country>().Property(e => e.Name).IsUnicode(false);

            modelBuilder.Entity<Country>().Property(e => e.ISOCode).IsUnicode(false);

            modelBuilder.Entity<Country>()
                .HasMany<Region>(r => r.Regions)
                .WithRequired(c => c.Country)
                .HasForeignKey(r => r.CountryId);

            modelBuilder.Entity<Region>()
                .HasMany<Resort>(r => r.Resorts)
                .WithRequired(r => r.Region)
                .HasForeignKey(r => r.RegionID);

            modelBuilder.Entity<Resort>().HasKey(x => x.Id);

            modelBuilder.Entity<Brand>().HasMany(b => b.BrandGeography).WithMany(r => r.Brands).Map(
                m =>
                    {
                        m.MapLeftKey("BrandID");
                        m.MapRightKey("GeographyLevel3ID");
                        m.ToTable("BrandGeographyLevel3");
                    });

            modelBuilder.Entity<MarketingCode>().Property(e => e.Name).IsUnicode(false);

            modelBuilder.Entity<Nationality>().Property(e => e.Name).IsUnicode(false);

            modelBuilder.Entity<Nationality>().Property(e => e.ISOCode).IsUnicode(false);

            modelBuilder.Entity<SalesChannel>().Property(e => e.Name).IsUnicode(false);
        }
    }
}