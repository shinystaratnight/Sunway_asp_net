namespace Web.Template.Application.Interfaces.Payment
{
    /// <summary>
    /// Interface IThreeDSecureService
    /// </summary>
    public interface IThreeDSecureService
    {
        /// <summary>
        /// Get3s the d secure redirect.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The ThreeDSecureRedirectReturn.</returns>
        IThreeDSecureRedirectReturn Get3DSecureRedirect(IThreeDSecureRedirectModel model);

        /// <summary>
        /// Process3s the d secure.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The ProcessThreeDSecureReturn.</returns>
        IProcessThreeDSecureReturn Process3DSecure(IProcessThreeDSecureModel model);
    }
}