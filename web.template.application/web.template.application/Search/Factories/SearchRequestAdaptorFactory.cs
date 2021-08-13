namespace Web.Template.Application.Search.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.Http;

    using Web.Template.Application.Interfaces.Search;

    /// <summary>
    /// A class responsible for choosing which Request Adaptor you want to use based on the request's response type.
    /// </summary>
    /// <seealso cref="ISearchRequestAdaptorFactory" />
    public class SearchRequestAdaptorFactory : ISearchRequestAdaptorFactory
    {
        /// <summary>
        /// The list of adaptors that can be returned from this class.
        /// </summary>
        private List<Type> adaptors;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchRequestAdaptorFactory"/> class.
        /// </summary>
        public SearchRequestAdaptorFactory()
        {
            this.LoadAdaptors();
        }

        /// <summary>
        /// Creates the adaptor by the type of the response.
        /// </summary>
        /// <param name="responseType">Type of the response.</param>
        /// <returns>The correct adaptor for the response type</returns>
        /// <exception cref="System.NotImplementedException">Thrown if an adaptor does not exist for the specified response type.</exception>
        public ISearchRequestAdapter CreateAdaptorByResponseType(Type responseType)
        {
            ISearchRequestAdapter adaptor = this.CreateConnectAdaptor(responseType);

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
        /// <returns>A Request Adaptor</returns>
        private ISearchRequestAdapter CreateConnectAdaptor(Type responseType)
        {
            ISearchRequestAdapter adaptor = null;

            foreach (Type adaptorType in this.adaptors)
            {
                try
                {
                    adaptor = GlobalConfiguration.Configuration.DependencyResolver.GetService(adaptorType) as ISearchRequestAdapter;
                    if (adaptor != null && adaptor.ResponseType == responseType)
                    {
                        break;
                    }
                    else
                    {
                        adaptor = null;
                    }
                }
                catch (Exception ex)
                {
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

            if (this.adaptors.Count == 0)
            {
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (this.adaptors.Count == 0)
                    {
                        try
                        {
                            this.adaptors.AddRange(assembly.GetTypes().Where(type => type.GetInterfaces().Contains(typeof(ISearchRequestAdapter))).ToList());
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
        }
    }
}