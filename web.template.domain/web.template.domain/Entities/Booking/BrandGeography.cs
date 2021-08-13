namespace Web.Template.Domain.Entities.Booking
{
    using Web.Template.Domain.Interfaces.Entity;

    /// <summary>
    /// Brand Geography
    /// </summary>
    /// <seealso cref="Web.Template.Domain.Interfaces.Entity.ILookup" />
    public class BrandGeography : ILookup
    {
        public int BrandID { get; set; }

        /// <summary>
        /// Gets or sets the geographylevel3 identifier.
        /// </summary>
        /// <value>
        /// The geographylevel3 identifier.
        /// </value>
        public int Geographylevel3Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }
    }
}