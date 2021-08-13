namespace Web.Template.Application.Results.Factories
{
    using System;

    using Web.Template.Application.Interfaces.Results;

    /// <summary>
    /// Class for getting the correct connect result adaptor based on the type you need it for
    /// </summary>
    public interface IIvConnectResultComponentAdaptorFactory
    {
        /// <summary>
        /// Creates the type of the adaptor by response.
        /// </summary>
        /// <param name="componentType">Type of the response.</param>
        /// <returns>A connect results adaptor</returns>
        /// <exception cref="System.NotImplementedException">Thrown if no adaptor exists for the specified response type</exception>
        IConnectResultComponentAdaptor CreateAdaptorByComponentType(Type componentType);
    }
}