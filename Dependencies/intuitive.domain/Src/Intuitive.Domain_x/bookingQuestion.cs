//-----------------------------------------------------------------------
// <copyright file="BookingQuestion.cs" company="intuitive">
//     Copyright © 2015 intuitive. All rights reserved.
// </copyright>
// <author>adam</author>
// <date>2015-10-21</date>
// <summary>
// Question definition
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
    /// Class BookingQuestion.
    /// </summary>
    public class BookingQuestion
    {
        /// <summary>
        /// Gets or sets the question.
        /// </summary>
        /// <value>The question.</value>
        public string Question { get; set; }

        /// <summary>
        /// Gets or sets the answer.
        /// </summary>
        /// <value>The answer.</value>
        public string Answer { get; set; }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>The options.</value>
        public IList<string> Options { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BookingQuestion"/> is mandatory.
        /// </summary>
        /// <value><c>true</c> if mandatory; otherwise, <c>false</c>.</value>
        public bool Mandatory { get; set; }

        /// <summary>
        /// Gets or sets the third party reference.
        /// </summary>
        /// <value>The third party reference.</value>
        public string ThirdPartyReference { get; set; }
    }
}