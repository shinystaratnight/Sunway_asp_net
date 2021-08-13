namespace Web.Template.Application.Interfaces.Basket
{
    using iVectorConnectInterface.Interfaces;

    /// <summary>
    /// Interface IReleaseFlightSeatLockFactory
    /// </summary>
    public interface IReleaseFlightSeatLockFactory
    {
        /// <summary>
        /// Creates the specified release flight seat lock model.
        /// </summary>
        /// <param name="basketToken">The basket token.</param>
        /// <returns>The iVectorConnectRequest.</returns>
        iVectorConnectRequest Create(string basketToken);
    }
}
