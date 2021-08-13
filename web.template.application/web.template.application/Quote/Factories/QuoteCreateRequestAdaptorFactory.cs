namespace Web.Template.Application.Quote.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.Http;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Quote;
    using Web.Template.Application.Interfaces.Quote.Adaptors;

    /// <summary>
    /// Class QuoteCreateRequestAdaptorFactory.
    /// </summary>
    public class QuoteCreateRequestAdaptorFactory : IQuoteCreateRequestAdaptorFactory
    {
        /// <summary>
        /// The adaptors
        /// </summary>
        private List<Type> adaptors;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuoteCreateRequestAdaptorFactory"/> class.
        /// </summary>
        public QuoteCreateRequestAdaptorFactory()
        {
            this.LoadAdaptors();
        }

        /// <summary>
        /// Creates the type of the adaptor by component.
        /// </summary>
        /// <param name="componentType">Type of the component.</param>
        /// <returns>A request adaptor.</returns>
        /// <exception cref="System.NotImplementedException">Thrown if an adaptor does not exist for the specified response type.</exception>
        public IQuoteCreateRequestAdaptor CreateAdaptorByComponentType(ComponentType componentType)
        {
            IQuoteCreateRequestAdaptor adaptor = this.CreateConnectAdaptor(componentType);
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
        /// <returns>The quote create request adaptor.</returns>
        private IQuoteCreateRequestAdaptor CreateConnectAdaptor(ComponentType componentType)
        {
            IQuoteCreateRequestAdaptor adaptor = null;

            foreach (Type adaptorType in this.adaptors)
            {
                adaptor = GlobalConfiguration.Configuration.DependencyResolver.GetService(adaptorType) as IQuoteCreateRequestAdaptor;
                if (adaptor != null && adaptor.ComponentType == componentType)
                {
                    break;
                }
            }
            return adaptor;
        }

        /// <summary>
        /// Loads the adaptors.
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
                        this.adaptors.AddRange(assembly.GetTypes().Where(type => type.GetInterfaces().Contains(typeof(IQuoteCreateRequestAdaptor))).ToList());
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
    }
}
