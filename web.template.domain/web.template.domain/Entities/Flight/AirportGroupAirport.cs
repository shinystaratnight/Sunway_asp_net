namespace Web.Template.Domain.Entities.Flight
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations.Schema;
	using Web.Template.Domain.Interfaces.Entity;

	/// <summary>
	/// Entity representing Airports in a group of Airports
	/// </summary>
	[Table("AirportGroupAirport")]
    public partial class AirportGroupAirport : ILookup
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Column("AirportGroupAirportID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the airport group identifier.
        /// </summary>
        /// <value>
        /// The airport group identifier.
        /// </value>
        [Column("AirportGroupID")]
        public int AirportGroupID { get; set; }

		/// <summary>
		/// Gets or sets the airport identifier.
		/// </summary>
		/// <value>
		/// The airport identifier.
		/// </value>
		[Column("AirportID")]
		public int AirportID { get; set; }
    }
}