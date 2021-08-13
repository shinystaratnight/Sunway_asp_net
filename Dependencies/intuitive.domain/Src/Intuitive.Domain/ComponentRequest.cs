//-----------------------------------------------------------------------
// <copyright file="ComponentRequest.cs" company="intuitive">
//     Copyright © 2015 intuitive. All rights reserved.
// </copyright>
// <author>adam</author>
// <date>2015-10-21</date>
// <summary>
// Component Requests
// </summary>
//-----------------------------------------------------------------------
namespace Intuitive.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Component Request Model
    /// </summary>
    public class ComponentRequest
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the details.
        /// </summary>
        /// <value>
        /// The details.
        /// </value>
        public string Details { get; set; }

        /// <summary>
        /// Gets or sets the booking identifier.
        /// </summary>
        /// <value>
        /// The booking identifier.
        /// </value>
        public int BookingId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ComponentRequest"/> is important.
        /// </summary>
        /// <value>
        ///   <c>true</c> if important; otherwise, <c>false</c>.
        /// </value>
        public bool Important { get; set; }

        /// <summary>
        /// Gets or sets the type of the component.
        /// </summary>
        /// <value>
        /// The type of the component.
        /// </value>
        public string ComponentType { get; set; }

        /// <summary>
        /// Gets or sets the component identifier.
        /// </summary>
        /// <value>
        /// The component identifier.
        /// </value>
        public int ComponentId { get; set; }
    }
}