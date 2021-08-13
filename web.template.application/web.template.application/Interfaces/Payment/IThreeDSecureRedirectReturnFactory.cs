namespace Web.Template.Application.Interfaces.Payment
{
    /// <summary>
    /// Interface IThreeDSecureRedirectReturnFactory
    /// </summary>
    public interface IThreeDSecureRedirectReturnFactory
    {
        /// <summary>
        /// Creates the specified response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>The ThreeDSecureRedirectReturn.</returns>
        IThreeDSecureRedirectReturn Create(iVectorConnectInterface.Get3DSecureRedirectResponse response);
    }
}