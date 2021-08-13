namespace Web.Template.Application.Search.Adaptor
{
    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Search.SearchModels;

    /// <summary>
    /// Interface IExtraSearchModelAdaptor
    /// </summary>
    public interface IExtraSearchModelAdaptor
    {
        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <param name="basket">The basket.</param>
        /// <param name="extraBasketSearchModel">The extra basket search model.</param>
        /// <returns>The ExtraSearchModel.</returns>
        ExtraSearchModel Create(IBasket basket, IExtraBasketSearchModel extraBasketSearchModel);
    }
}