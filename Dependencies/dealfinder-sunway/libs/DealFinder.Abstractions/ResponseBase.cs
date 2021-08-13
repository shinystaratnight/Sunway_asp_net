// Copyight © intuitive Ltd. All rights reserved

namespace Intuitive.Web.Api
{
    /// <summary>
    /// Provides a base implementation of an API response.
    /// </summary>
    public abstract class ResponseBase
    {
        /// <summary>
        /// Gets or sets whether the result was success.
        /// </summary>
        public bool Success { get; set; }
    }
}
