namespace Web.Template.Application.Interfaces.Quote.Adaptors
{
    using Web.Template.Application.Enum;

    /// <summary>
    /// A Request Adaptor factory decides which request adaptor we want to used based on the component type.
    /// </summary>
    public interface IQuoteCreateRequestAdaptorFactory
    {
        /// <summary>
        /// Creates the type of the adaptor by component.
        /// </summary>
        /// <param name="componentType">Type of the component.</param>
        /// <returns>A request adaptor.</returns>
        IQuoteCreateRequestAdaptor CreateAdaptorByComponentType(ComponentType componentType);
    }
}
