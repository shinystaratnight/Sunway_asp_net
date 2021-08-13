namespace Web.Template.Application.Interfaces.Payment
{
    using iVectorConnectInterface.Interfaces;

    /// <summary>
    /// Interface IGet3DSecureRedirectRequestFactory
    /// </summary>
    public interface IGet3DSecureRedirectRequestFactory
    {
        /// <summary>
        /// Creates the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>iVectorConnectRequest.</returns>
        iVectorConnectRequest Create(IThreeDSecureRedirectModel model);
    }
}