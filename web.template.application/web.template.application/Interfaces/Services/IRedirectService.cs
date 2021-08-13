namespace Web.Template.Application.Interfaces.Services
{
    using System.Collections.Generic;
    using Application.Site;
    using Site;

    /// <summary>
    ///     Interface ISiteService
    /// </summary>
    public interface IRedirectService
    {
        /// <summary>
        ///     Gets a list of all redirects.
        /// </summary>
        /// <returns>The site</returns>
        List<IRedirect> GetRedirects();

        /// <summary>
        /// Adds the redirect.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <param name="site">The site.</param>
        /// <returns>
        /// Whether or not adding the redirect was succesful.
        /// </returns>
        RedirectReturn AddRedirect(string url, string redirectUrl, string site);

        /// <summary>
        /// Adds the redirect.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <param name="redirectId">The redirect identifier.</param>
        /// <param name="site">The site.</param>
        /// <returns>
        /// Whether or not adding the redirect was succesful.
        /// </returns>
        RedirectReturn ModifyRedirect(string url, string redirectUrl, int redirectId, string site);

        /// <summary>
        /// Adds the redirect.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <param name="redirectId">The redirect identifier.</param>
        /// <param name="site">The site.</param>
        /// <returns>
        /// Whether or not adding the redirect was succesful.
        /// </returns>
        RedirectReturn DeleteRedirect(string url, string redirectUrl, int redirectId, string site);
    }
}