//-----------------------------------------------------------------------
// <copyright file="User.cs" company="intuitive">
//     Copyright © 2015 intuitive. All rights reserved.
// </copyright>
// <author>adam</author>
// <date>2015-10-21</date>
// <summary>
// User
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
    /// The user.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
    }
}