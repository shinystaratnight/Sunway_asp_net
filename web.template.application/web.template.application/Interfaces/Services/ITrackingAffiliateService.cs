namespace Web.Template.Application.Interfaces.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Web.Template.Application.Interfaces.Tracking;

    /// <summary>
    /// Interface for the tracking affiliate Service
    /// </summary>
    public interface ITrackingAffiliateService
    {
        /// <summary>
        /// Setups the tracking affiliates.
        /// </summary>
        /// <returns>
        /// Returns the tracking affiliates
        /// </returns>
        Task<List<ITrackingAffiliate>> SetupTrackingAffiliates();
    }
}