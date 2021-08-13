namespace Web.Template.Application.IVectorConnect.Requests
{
    using System.Web;

    using iVectorConnectInterface;

    /// <summary>
    /// An interface defining a connect login details factory.
    /// </summary>
    public interface IConnectLoginDetailsFactory
    {
        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="useContentDetails"></param>
        /// <returns>
        /// A connect login details
        /// </returns>
        LoginDetails Create(HttpContext context, bool useContentDetails = false);
    }
}