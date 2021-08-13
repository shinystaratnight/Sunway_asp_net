namespace Web.Template.Application.Interfaces.Payment
{
    using Web.Template.Application.Interfaces.Payment;

    /// <summary>
    /// Interface IProcess3DSecureReturnFactory
    /// </summary>
    public interface IProcess3DSecureReturnFactory
    {
        /// <summary>
        /// Creates the specified response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>The ProcessThreeDSecureReturn.</returns>
        IProcessThreeDSecureReturn Create(iVectorConnectInterface.Process3DSecureReturnResponse response);
    }
}