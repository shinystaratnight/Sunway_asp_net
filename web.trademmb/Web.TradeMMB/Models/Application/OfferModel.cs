namespace Web.TradeMMB.Models.Application
{
    using System.Collections.Generic;

    /// <summary>
    /// The Offer Model
    /// A list of string parameters required for the model
    /// </summary>
    public class OfferModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OfferModel"/> class.
        /// </summary>
        public OfferModel()
        {
            this.ParamList = new List<string>();
        }

        /// <summary>
        /// Gets or sets the parameter list.
        /// </summary>
        /// <value>
        /// The parameter list.
        /// </value>
        public List<string> ParamList { get; set; }
    }
}