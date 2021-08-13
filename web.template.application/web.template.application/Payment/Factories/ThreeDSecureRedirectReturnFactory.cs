namespace Web.Template.Application.Payment.Factories
{
    using Web.Template.Application.Interfaces.Payment;
    using Web.Template.Application.Payment.Models;

    using ivci = iVectorConnectInterface;

    /// <summary>
    /// Class ThreeDSecureRedirectReturnFactory.
    /// </summary>
    public class ThreeDSecureRedirectReturnFactory : IThreeDSecureRedirectReturnFactory
    {

        /// <summary>
        /// Creates the specified response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>The ThreeDSecureRedirectReturn.</returns>
        public IThreeDSecureRedirectReturn Create(ivci.Get3DSecureRedirectResponse response)
        {
            if (response != null)
            {
                var tdsReturn = new ThreeDSecureRedirectReturn()
                                    {
                                        Html = response.HTMLData,
                                        Success = response.ReturnStatus.Success,
                                        Warnings = response.ReturnStatus.Exceptions,
                                        Enrollment = response.Enrollment,
                                        PaymentToken = response.PaymentToken
                                    };
                return tdsReturn;
            }
            
            return new ThreeDSecureRedirectReturn();
        }
    }
}
