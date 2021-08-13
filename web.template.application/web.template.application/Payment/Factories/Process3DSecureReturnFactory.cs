namespace Web.Template.Application.Payment.Factories
{
    using Web.Template.Application.Interfaces.Payment;
    using Web.Template.Application.Payment.Models;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Class Process3DSecureReturnFactory.
    /// </summary>
    public class Process3DSecureReturnFactory : IProcess3DSecureReturnFactory
    {

        /// <summary>
        /// Creates the specified response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>The ProcessThreeDSecureReturn.</returns>
        public IProcessThreeDSecureReturn Create(ivci.Process3DSecureReturnResponse response)
        {

            var tdsReturn = new ProcessThreeDSecureReturn()
                                {
                                    PaymentToken = response.PaymentToken,
                                    Success = response.ReturnStatus.Success,
                                    ThreeDSecureCode = response.ThreeDSecureCode,
                                    Warnings = response.ReturnStatus.Exceptions
                                };
            return tdsReturn;
        }
    }
}
