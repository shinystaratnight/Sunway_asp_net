namespace Web.Template.Application.Book.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.Http;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Book;
    using Web.Template.Application.Interfaces.Search;

    /// <summary>
    /// A class responsible for choosing which Request Adaptor you want to use based on the request's response type.
    /// </summary>
    /// <seealso cref="ISearchRequestAdaptorFactory" />
    public class BookAdaptorFactory : IBookRequestAdaptorFactory
    {
        /// <summary>
        /// The list of adaptors that can be returned from this class.
        /// </summary>
        private List<Type> adaptors;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookAdaptorFactory"/> class.
        /// </summary>
        public BookAdaptorFactory()
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
        public IBookRequestAdaptor CreateAdaptorByComponentType(ComponentType componentType)
        {
            IBookRequestAdaptor adaptor = this.CreateConnectAdaptor(componentType);

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
        private IBookRequestAdaptor CreateConnectAdaptor(ComponentType componentType)
        {
            IBookRequestAdaptor adaptor = null;

            foreach (Type adaptorType in this.adaptors)
            {
                adaptor = GlobalConfiguration.Configuration.DependencyResolver.GetService(adaptorType) as IBookRequestAdaptor;
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
                if (this.adaptors.Count == 0)
                {
                    try
                    {
                        this.adaptors.AddRange(assembly.GetTypes().Where(type => type.GetInterfaces().Contains(typeof(IBookRequestAdaptor))).ToList());
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }
    }
}