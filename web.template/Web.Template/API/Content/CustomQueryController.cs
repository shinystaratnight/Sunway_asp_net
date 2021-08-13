namespace Web.Template.API.Content
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Http;
    using System.Xml;

    using Intuitive.WebControls;

    using Microsoft.Ajax.Utilities;

    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Services;

    /// <summary>
    ///     The API to custom query content to be called from a front end widget when custom query content is needed
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class CustomQueryController : ApiController
    {
        /// <summary>
        /// The page service
        /// </summary>
        private readonly ICustomQuery customQuery;

        public CustomQueryController(ICustomQuery customQuery)
        {
            this.customQuery = customQuery;
        }

        /// <summary>
        /// Gets the page by URL.
        /// </summary>
        /// <param name="queryName">The custom query to use.</param>
        /// <param name="parameterCsv">The custom parameters.</param>
        /// <returns>
        /// Returns the offers model of given URL
        /// </returns>
        [Route("api/customquery")]
        [HttpGet]
        public XmlDocument GetCustomQueryContentFromQueryString(
            [FromUri] string queryName = "",
            [FromUri] string parameterCsv = ""
            )
        {
            string[] parameterArray = parameterCsv.Split(new char[] { ',' });
            List<string> paramList = new List<string>();
            foreach (var p in parameterArray)
            {
                if (p.Trim() != "")
                {
                    paramList.Add(p);
                }
            }
            var xml = this.customQuery.GetCustomQueryXml(paramList, queryName);
            return xml;
        }
    }
}