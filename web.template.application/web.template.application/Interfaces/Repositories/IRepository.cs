namespace Web.Template.Application.Interfaces.Repositories
{
    /// <summary>
    ///     A Generic repository interface
    /// </summary>
    /// <typeparam name="T">The collection type of the repo</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    public interface IRepository<T, in TKey>
        where T : class
    {
        /// <summary>
        ///     Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>An object of type T</returns>
        T Get(TKey id);
    }
}