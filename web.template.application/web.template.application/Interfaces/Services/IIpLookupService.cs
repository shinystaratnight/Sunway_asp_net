namespace Web.Template.Application.Interfaces.Services
{
    /// <summary>
    /// interface defining a service to look up the country from their IP
    /// </summary>
    public interface IIpLookupService
    {
        /// <summary>
        /// Gets the client country code.
        /// </summary>
        /// <returns>a country code that the user is from</returns>
        string GetClientCountryCode();
    }
}