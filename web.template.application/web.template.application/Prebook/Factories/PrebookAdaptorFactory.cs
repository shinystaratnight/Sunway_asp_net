namespace Web.Template.Application.Prebook.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.Http;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Prebook;
    using Web.Template.Application.Interfaces.Search;

    /// <summary>
    /// A class responsible for choosing which Request Adaptor you want to use based on the request's response type.
    /// </summary>
    /// <seealso cref="ISearchRequestAdaptorFactory" />
    public class PrebookAdaptorFactory : IPrebookRequestAdaptorFactory
    {
        /// <summary>
        /// The list of adaptors that can be returned from this class.
        /// </summary>
        private List<Type> adaptors;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrebookAdaptorFactory" /> class.
        /// </summary>
        public PrebookAdaptorFactory()
        {
            this.LoadAdaptors();
        }

        /// <summary>
        /// Creates the adaptor by the type of the response.
        /// </summary>
        /// <param name="componentType">The type of component e.g. Flight</param>
        /// <returns>
        /// The correct adaptor for the response type
        /// </returns>
        /// <exception cref="System.NotImplementedException">Thrown if an adaptor does not exist for the specified response type.</exception>
        public IPrebookRequestAdaptor CreateAdaptorByComponentType(ComponentType componentType)
        {
            IPrebookRequestAdaptor adaptor = this.CreateConnectAdaptor(componentType);

            if (adaptor == null)
            {
                throw new NotImplementedException();
            }

            return adaptor;
        }

        /// <summary>
        /// Creates the connect adaptor.
        /// </summary>
        /// <param name="componentType">Type of the component.</param>
        /// <returns>
        /// A Request Adaptor
        /// </returns>
        private IPrebookRequestAdaptor CreateConnectAdaptor(ComponentType componentType)
        {
            IPrebookRequestAdaptor adaptor = null;

            foreach (Type adaptorType in this.adaptors)
            {
                adaptor = GlobalConfiguration.Configuration.DependencyResolver.GetService(adaptorType) as IPrebookRequestAdaptor;
                if (adaptor != null && adaptor.ComponentType == componentType)
                {
                    break;
                }
            }

            return adaptor;
        }

        /// <summary>
        /// Gets all classes that implement the IRequest Adaptor Interface
        /// </summary>
        private void LoadAdaptors()
        {
            this.adaptors = new List<Type>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (this.adaptors == null || this.adaptors.Count == 0)
                {
                    try
                    {
                        this.adaptors.AddRange(assembly.GetTypes().Where(type => type.GetInterfaces().Contains(typeof(IPrebookRequestAdaptor))).ToList());
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }
    }
}