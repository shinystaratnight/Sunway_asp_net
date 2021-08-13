namespace Web.Template.Application.Search.Adaptor
{
    using System.Web;

    using iVectorConnectInterface.Interfaces;

    using Web.Template.Application.Interfaces.Models;

    public interface IExtraSearchRequestAdaptor
    {
        /// <summary>
        /// Creates the specified searchmodel.
        /// </summary>
        /// <param name="searchmodel">The searchmodel.</param>
        /// <param name="context">The context.</param>
        /// <returns>iVectorConnectInterface.Interfaces.iVectorConnectRequest.</returns>
        iVectorConnectRequest Create(IExtraSearchModel searchmodel, HttpContext context);
    }
}