namespace Web.Template.Application.SiteBuilderService
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface ISiteBuilderRequest
    /// </summary>
    public interface ISiteBuilderRequest
    {
        /// <summary>
        /// Simple web request wrapper, should probably centralize.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="url">The URL.</param>
        /// <param name="body">The body.</param>
        /// <param name="headers">The headers.</param>
        /// <returns>The response body as a string</returns>
        string Send(string method, string url, string body = null, Dictionary<string, string> headers = null);
    }
}