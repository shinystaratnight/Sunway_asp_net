namespace Web.Template.Application.Interfaces.Prebook
{
    using Web.Template.Application.Enum;

    /// <summary>
    /// A Request Adaptor factory decides which request adaptor we want to used based on the response type.
    /// </summary>
    public interface IPrebookRequestAdaptorFactory
    {
        /// <summary>
        /// Creates the type of the adaptor by response.
        /// </summary>
        /// <param name="componentType">The type of component e.g. Flight</param>
        /// <returns>
        /// A Request Adaptor
        /// </returns>
        IPrebookRequestAdaptor CreateAdaptorByComponentType(ComponentType componentType);
    }
}