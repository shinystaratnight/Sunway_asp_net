namespace Web.Template.Application.Interfaces.Booking.Factories
{
    using System.Web;

    using iVectorConnectInterface;
    using iVectorConnectInterface.Interfaces;
    using iVectorConnectInterface.Modules;

    using Web.Template.Application.Booking.Factories;
    using Web.Template.Application.IVectorConnect.Requests;

    /// <summary>
    /// a factory for building a booking details request
    /// </summary>
    public interface IModuleRequestFactory
    {
        /// <summary>
        /// Creates the specified booking reference.
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        /// <param name="route">The route.</param>
        /// <param name="method">The method.</param>
        /// <returns>
        /// a connect request
        /// </returns>
        iVectorConnectRequest Create(string moduleName, string route, string method);
    }

    /// <summary>
    /// Factory to make requests to modules in ivector connect
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Booking.Factories.IModuleRequestFactory" />
    public class ModuleRequestFactory : IModuleRequestFactory
    {
        /// <summary>
        /// The login details factory
        /// </summary>
        private readonly IConnectLoginDetailsFactory loginDetailsFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleRequestFactory"/> class.
        /// </summary>
        /// <param name="loginDetailsFactory">The login details factory.</param>
        public ModuleRequestFactory(IConnectLoginDetailsFactory loginDetailsFactory)
        {
            this.loginDetailsFactory = loginDetailsFactory;
        }

        /// <summary>
        /// Creates the specified booking reference.
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        /// <param name="route">The route.</param>
        /// <param name="method">The method.</param>
        /// <returns>
        /// a connect request
        /// </returns>
        public iVectorConnectRequest Create(string moduleName, string route, string method)
        {
            iVectorConnectRequest moduleRequest = new ModuleRequest()
            {
                LoginDetails = this.loginDetailsFactory.Create(HttpContext.Current),
                ModuleName = moduleName,
                Route = route,
                Method = method
            };
            return moduleRequest;
        }
    }
}