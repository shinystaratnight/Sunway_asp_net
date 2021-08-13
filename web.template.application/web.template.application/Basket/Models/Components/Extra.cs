namespace Web.Template.Application.Basket.Models.Components
{
    using System.Collections.Generic;
    using System.Linq;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Extra basket component
    /// </summary>
    public class Extra : BasketCompontentBase
    {
        /// <summary>
        /// Gets or sets the adult ages.
        /// </summary>
        /// <value>The adult ages.</value>
        public List<int> AdultAges { get; set; }

        /// <summary>
        /// Gets or sets the type of the component.
        /// </summary>
        /// <value>The type of the component.</value>
        public override ComponentType ComponentType => ComponentType.Extra;

        /// <summary>
        /// Gets or sets the extra identifier.
        /// </summary>
        /// <value>The extra identifier.</value>
        public int ExtraId { get; set; }

        /// <summary>
        /// Gets or sets the name of the extra.
        /// </summary>
        /// <value>The name of the extra.</value>
        public string ExtraName { get; set; }

        /// <summary>
        /// Gets or sets the type of the extra.
        /// </summary>
        /// <value>The type of the extra.</value>
        public string ExtraType { get; set; }

        /// <summary>
        /// Gets or sets the extra type identifier.
        /// </summary>
        /// <value>The extra type identifier.</value>
        public int ExtraTypeId { get; set; }

        /// <summary>
        /// Gets or sets the include options.
        /// </summary>
        /// <value>The include options.</value>
        public bool IncludeOptions { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        /// <value>The price.</value>
        public override decimal TotalPrice { get; set; }

        /// <summary>
        /// Setups the meta data.
        /// </summary>
        /// <param name="metaData">The meta data.</param>
        public override void SetupMetaData(Dictionary<string, string> metaData)
        {
            if (metaData != null)
            {
                this.IncludeOptions = bool.Parse(metaData["IncludeOptions"]);
            }
        }
    }
}
