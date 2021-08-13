using Web.Template.Application.Results.ResultModels;

namespace Web.Template.Application.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// The Offer Model
    /// A list of string parameters required for the model
    /// </summary>
    public class QuoteModel
    {
        /// <summary>
        /// Gets or sets the parameter list.
        /// </summary>
        /// <value>
        /// The parameter list.
        /// </value>
        public int PropertyToken { get; set; }

        /// <summary>
        /// Gets or sets the parameter list.
        /// </summary>
        /// <value>
        /// The parameter list.
        /// </value>
        public int FlightToken { get; set; }

        /// <summary>
        /// Gets or sets the parameter list.
        /// </summary>
        /// <value>
        /// The parameter list.
        /// </value>
        public string PropertySearchToken { get; set; }

        /// <summary>
        /// Gets or sets the parameter list.
        /// </summary>
        /// <value>
        /// The parameter list.
        /// </value>
        public string FlightSearchToken { get; set; }
        public string ToEmail { get; set; }
        public string ToName { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public string CCEmail { get; set; }
        [JsonProperty("RoomOptions")]
        public List<RoomOption> RoomOptions { get; set; }
    }
}