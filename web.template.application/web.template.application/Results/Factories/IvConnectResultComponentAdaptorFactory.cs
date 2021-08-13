namespace Web.Template.Application.Results.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.Http;

    using Web.Template.Application.Interfaces.Results;

    /// <summary>
    /// A class responsible for deciding which connect results adaptor we want to used based on the response type.
    /// </summary>
    /// <seealso cref="IIVConnectResultsAdaptorFactory" />
    public class IvConnectResultComponentAdaptorFactory : IIvConnectResultComponentAdaptorFactory
    {
        /// <summary>
        /// The adaptors
        /// </summary>
        private List<Type> adaptors;

        /// <summary>
        /// Initializes a new instance of the <see cref="IvConnectResultComponentAdaptorFactory"/> class.
        /// </summary>
        public IvConnectResultComponentAdaptorFactory()
        {
            this.LoadAdaptors();
        }

        /// <summary>
        /// Creates the type of the adaptor by response.
        /// </summary>
        /// <param name="componentType">Type of the response.</param>
        /// <returns>A connect results adaptor</returns>
        /// <exception cref="System.NotImplementedException">Thrown if no adaptor exists for the specified response type</exception>
        public IConnectResultComponentAdaptor CreateAdaptorByComponentType(Type componentType)
        {
            IConnectResultComponentAdaptor adaptor = this.CreateConnectAdaptor(componentType);

            if (adaptor == null)
            {
                throw new NotImplementedException();
            }

            return adaptor;
        }

        /// <summary>
        /// Creates the connect adaptor.
        /// </summary>
        /// <param name="responseType">Type of the response.</param>
        /// <returns>A connect results adaptor</returns>
        private IConnectResultComponentAdaptor CreateConnectAdaptor(Type responseType)
        {
            IConnectResultComponentAdaptor adaptor = null;

            foreach (Type adaptorType in this.adaptors)
            {
                adaptor = GlobalConfiguration.Configuration.DependencyResolver.GetService(adaptorType) as IConnectResultComponentAdaptor;
                if (adaptor != null && adaptor.ComponentType == responseType)
                {
                    break;
                }
            }

            return adaptor;
        }

        /// <summary>
        /// Finds all classes that implement the interface and adds them to the collection.
        /// </summary>
        private void LoadAdaptors()
        {
            this.adaptors = new List<Type>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    this.adaptors.AddRange(assembly.GetTypes().Where(type => type.GetInterfaces().Contains(typeof(IConnectResultComponentAdaptor))).ToList());
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}