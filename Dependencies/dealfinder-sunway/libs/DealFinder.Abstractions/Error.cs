// Copyight © intuitive Ltd. All rights reserved

#nullable enable
namespace Intuitive.Web.Api
{
    using System.Diagnostics;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a error.
    /// </summary>
    [DebuggerDisplay("{StatusCode} - {SystemCode}")]
    public class Error
    {
        /// <summary>
        /// Initialises a new instance of <see cref="Error"/>
        /// </summary>
        /// <remarks>This constructor is used through deserialisation.</remarks>
        private Error()
        {
            SystemCode = null!;
            Title = null!;
        }

        /// <summary>
        /// Initialises a new instance of <see cref="Error"/>.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="systemCode">The system code for the error.</param>
        /// <param name="title">The error title.</param>
        /// <param name="description">[Optional] The error description.</param>
        /// <param name="category">[Optional] The error category.</param>
        /// <param name="reference">[Optional] A unique reference to provide to the connected client/</param>
        /// <param name="causedBy">[Optional] The error that caused this error.</param>
        /// <param name="data">[Optional] A custom attached data.</param>
        [JsonConstructor]
        public Error(
            int statusCode, 
            string systemCode, 
            string title, 
            string? description = null, 
            string? category = null, 
            string? reference = null, 
            Error? causedBy = null,
            object? data = null)
        {
            StatusCode = Ensure.IsInRange(
                statusCode, 
                100 /* 100 Continue */, 
                511 /* 511 Network Authentication Required */, 
                nameof(statusCode));
            SystemCode = Ensure.IsNotNullOrEmpty(systemCode, nameof(systemCode));
            Title = Ensure.IsNotNullOrEmpty(title, nameof(title));
            Description = description;
            Category = category;
            Reference = reference;
            CausedBy = causedBy;
            Data = data;
        }

        /// <summary>
        /// Gets the error category.
        /// </summary>
        [JsonProperty("category", NullValueHandling = NullValueHandling.Ignore)]
        public string? Category { get; set; }

        /// <summary>
        /// Gets the error that caused this error.
        /// </summary>
        [JsonProperty("causedBy", NullValueHandling = NullValueHandling.Ignore)]
        public Error? CausedBy { get; set; }

        /// <summary>
        /// Gets or sets the custom data
        /// </summary>
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public object? Data { get; set; }

        /// <summary>
        /// Gets the error description.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets the error reference.
        /// </summary>
        [JsonProperty("reference", NullValueHandling = NullValueHandling.Ignore)]
        public string? Reference { get; set; }

        /// <summary>
        /// Gets the status code.
        /// </summary>
        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets the system code.
        /// </summary>
        [JsonProperty("systemCode")]
        public string SystemCode { get; set; }

        /// <summary>
        /// Gets the error title.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }
    }
}