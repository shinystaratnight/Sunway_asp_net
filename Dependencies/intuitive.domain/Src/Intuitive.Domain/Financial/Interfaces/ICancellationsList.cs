//-----------------------------------------------------------------------
// <copyright file="ICancellationsList.cs" company="Sunway travel">
//     Company copyright tag.
// </copyright>
//-----------------------------------------------------------------------

namespace Intuitive.Domain.Financial.Interfaces
{
    using System;
    using System.Collections.Generic;

    using Intuitive.Domain.Financial.Enums;

    /// <summary>
    /// Cancellations list interface.
    /// </summary>
    /// <typeparam name="T">
    /// Cancellation type.
    /// </typeparam>
    /// <seealso cref="System.Collections.Generic.IList{T}"/>
    public interface ICancellationsList<T> : IList<T>
        where T : class, ICancellation, new()
    {
        /// <summary>
        /// Gets the total cancellations amount.
        /// </summary>
        /// <value>
        /// The total cancellations amount.
        /// </value>
        decimal TotalAmount { get; }

        /// <summary>
        /// Adds the specified start date, end date, and amount as a cancellation.
        /// </summary>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
        void Add(DateTime startDate, DateTime endDate, decimal amount);

        /// <summary>
        /// Adds the specified start date, end date, and rounded amount as a cancellation..
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="roundDecimals">The round decimals.</param>
        void Add(DateTime startDate, DateTime endDate, decimal amount, int roundDecimals);

        /// <summary>
        /// Solidifies the cancellations.
        /// </summary>
        /// <param name="solidifyType">
        /// Type of the solidify.
        /// </param>
        /// <param name="solidifyTo">
        /// The solidify to.
        /// </param>
        /// <param name="finalCost">
        /// The final cost.
        /// </param>
        void Solidify(CancellationSolidifyType solidifyType, DateTime? solidifyTo = null, decimal finalCost = 0);
    }
}