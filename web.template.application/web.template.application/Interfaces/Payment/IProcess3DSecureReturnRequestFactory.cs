namespace Web.Template.Application.Interfaces.Payment
{
    using iVectorConnectInterface.Interfaces;

    /// <summary>
    /// Interface IProcess3DSecureReturnRequestFactory
    /// </summary>
    public interface IProcess3DSecureReturnRequestFactory
    {
        /// <summary>
        /// Creates the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The iVectorConnectRequest.</returns>
        iVectorConnectRequest Create(IProcessThreeDSecureModel model);
    }
}
