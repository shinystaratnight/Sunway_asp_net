namespace Web.Template.Application.Interfaces.Results
{
    using System;
    using System.Web;

    using Web.Template.Application.Enum;
    using Web.Template.Application.Interfaces.Models;

    /// <summary>
    /// Interface IConnectResultComponentAdaptor
    /// </summary>
    public interface IConnectResultComponentAdaptor
    {
        /// <summary>
        /// Gets the type of the component.
        /// </summary>
        /// <value>The type of the component.</value>
        Type ComponentType { get; }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <param name="connectResult">The connect result.</param>
        /// <param name="searchMode">The search mode.</param>
        /// <param name="context">The context.</param>
        /// <returns>The Result.</returns>
        IResult Create(object connectResult, SearchMode searchMode, HttpContext context);

        /// <summary>
        /// Sets the arrival date.
        /// </summary>
        /// <param name="arrivalDate">The arrival date.</param>
        void SetArrivalDate(DateTime arrivalDate);

        /// <summary>
        /// Sets the duration.
        /// </summary>
        /// <param name="duration">The duration.</param>
        void SetDuration(int duration);
    }
}