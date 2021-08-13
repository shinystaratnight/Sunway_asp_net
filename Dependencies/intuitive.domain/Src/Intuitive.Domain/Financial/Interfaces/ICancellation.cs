//-----------------------------------------------------------------------
// <copyright file="ICancellation.cs" company="Sunway travel">
//     Company copyright tag.
// </copyright>
//-----------------------------------------------------------------------

namespace Intuitive.Domain.Financial.Interfaces
{
    using System;

    /// <summary>
    ///     Cancellation interface.
    /// </summary>
    public interface ICancellation
    {
        /// <summary>
        ///     Gets or sets the start date.
        /// </summary>
        /// <value>
        ///     The start date.
        /// </value>
        DateTime StartDate { get; set; }

        /// <summary>
        ///     Gets or sets the end date.
        /// </summary>
        /// <value>
        ///     The end date.
        /// </value>
        DateTime EndDate { get; set; }

        /// <summary>
        ///     Gets or sets the amount.
        /// </summary>
        /// <value>
        ///     The amount.
        /// </value>
        decimal Amount { get; set; }
    }
}