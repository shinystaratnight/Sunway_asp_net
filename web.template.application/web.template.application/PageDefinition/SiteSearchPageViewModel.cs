using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Template.Application.PageDefinition
{
    public class SiteSearchPageViewModel
    {
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the page title.
        /// </summary>
        /// <value>
        /// The page title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the page entity informations.
        /// </summary>
        /// <value>
        /// The page entity informations.
        /// </value>
        public List<PageEntityInformation> PageEntityInformations { get; set; }
    }
}
