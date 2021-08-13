// Copyight © intuitive Ltd. All rights reserved

namespace Intuitive.Web.Api
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents an error response.
    /// </summary>
    public class ErrorResponse : ResponseBase
    {
        /// <summary>
        /// Initialises a new instance of <see cref="ErrorResponse"/>
        /// </summary>
        /// <remarks>This constructor is used through deserialisation.</remarks>
        private ErrorResponse()
        {
            Success = false;
            Errors = null!;
        }

        /// <summary>
        /// Initialises a new instance of <see cref="ErrorResponse"/>.
        /// </summary>
        /// <param name="errors">The set of errors.</param>
        [JsonConstructor]
        public ErrorResponse(Error[] errors)
        {
            Ensure.IsNotNullOrEmpty(errors, nameof(errors));

            Success = false;
            Errors = errors;
        }

        /// <summary>
        /// Gets the set of errors.
        /// </summary>
        [JsonProperty("errors")]
        public Error[] Errors { get; set; }
    }
}