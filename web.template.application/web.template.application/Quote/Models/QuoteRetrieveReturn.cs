namespace Web.Template.Application.Quote.Models
{
    using System.CodeDom;
    using System.Collections.Generic;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Quote.Models;

    /// <summary>
    /// Class QuoteRetrieveReturn.
    /// </summary>
    public class QuoteRetrieveReturn : IQuoteRetrieveReturn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuoteRetrieveReturn"/> class.
        /// </summary>
        public QuoteRetrieveReturn()
        {
            this.ResultCounts = new Dictionary<string, int>();
            this.ResultTokens = new Dictionary<string, string>();
            this.QuoteComponentTypes = new List<ComponentType>();
            this.RepricedComponentTypes = new List<ComponentType>();
            this.Warnings = new List<string>();
        }

        /// <summary>
        /// Gets or sets the basket token.
        /// </summary>
        /// <value>The basket token.</value>
        public string BasketToken { get; set; }

        /// <summary>
        /// Gets or sets the price change.
        /// </summary>
        /// <value>The price change.</value>
        public decimal PriceChange { get; set; }

        /// <summary>
        /// Gets or sets the property identifier.
        /// </summary>
        /// <value>The property identifier.</value>
        public int PropertyId { get; set; }

        /// <summary>
        /// Gets or sets the quote component types.
        /// </summary>
        /// <value>The quote component types.</value>
        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public List<ComponentType> QuoteComponentTypes { get; set; }

        /// <summary>
        /// Gets or sets the re-priced component types.
        /// </summary>
        /// <value>The re-priced component types.</value>
        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public List<ComponentType> RepricedComponentTypes { get; set; }

        /// <summary>
        /// Gets or sets the result counts.
        /// </summary>
        /// <value>The result counts.</value>
        public Dictionary<string, int> ResultCounts { get; set; }

        /// <summary>
        /// Gets or sets the result tokens.
        /// </summary>
        /// <value>The result tokens.</value>
        public Dictionary<string, string> ResultTokens { get; set; }

        /// <summary>
        /// Gets or sets the search model.
        /// </summary>
        /// <value>The search model.</value>
        public ISearchModel SearchModel { get; set; }

        /// <summary>
        /// Gets or sets the selected flight token.
        /// </summary>
        /// <value>The selected flight token.</value>
        public int SelectedFlightToken { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether success.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>The warnings.</value>
        public List<string> Warnings { get; set; }
    }
}
