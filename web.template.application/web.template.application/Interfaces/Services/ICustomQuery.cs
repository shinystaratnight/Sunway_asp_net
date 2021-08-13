namespace Web.Template.Application.Services
{
    using System.Collections.Generic;
    using System.Xml;

    /// <summary>
    /// custom query interface
    /// </summary>
    public interface ICustomQuery
    {
        /// <summary>
        /// Gets the custom query XML.
        /// </summary>
        /// <param name="params">The parameters.</param>
        /// <param name="customQueryName">Name of the custom query.</param>
        /// <returns>
        /// an XML doc
        /// </returns>
        XmlDocument GetCustomQueryXml(List<string> @params, string customQueryName);
    }
}