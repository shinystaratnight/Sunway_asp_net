namespace Web.Template.Data.Connect
{
    using System.Xml;

    /// <summary>
    /// Using intuitive async cache to get a lookup
    /// </summary>
    public interface IAsyncLookup
    {
        /// <summary>
        /// Gets the asynchronous CMS lookup.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>an xml document containing the lookup</returns>
        XmlDocument GetAsyncCMSLookup(string objectType, int id);

        /// <summary>
        /// Gets the asynchronous lookup.
        /// </summary>
        /// <param name="lookup">The lookup.</param>
        /// <param name="clearCache">if set to <c>true</c> [clear cache].</param>
        /// <returns>
        /// an xml document containing the lookup
        /// </returns>
        XmlDocument GetAsyncLookup(string lookup, bool clearCache = false);
    }
}