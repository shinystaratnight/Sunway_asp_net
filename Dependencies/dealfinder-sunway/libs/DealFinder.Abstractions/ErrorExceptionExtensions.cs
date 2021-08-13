// Copyight © intuitive Ltd. All rights reserved
#nullable enable
namespace Intuitive.Web.Api
{
    using System;

    /// <summary>
    /// Provides extension methods for the <see cref="Exception"/> type.
    /// </summary>
    public static class ErrorExceptionExtensions
    {
        /// <summary>
        /// Returns an <see cref="Error"/> instance for the given exception.
        /// </summary>
        /// <param name="exception">The exception instance.</param>
        /// <param name="statusCode">[Optional] The HTTP status code.</param>
        /// <param name="systemCode">[Optional] The system code for the error.</param>
        /// <param name="title">[Optional] The error title.</param>
        /// <param name="description">[Optional] The error description.</param>
        /// <param name="category">[Optional] The error category.</param>
        /// <param name="reference">[Optional] A unique reference to provide to the connected client/</param>
        /// <returns>The <see cref="Error"/> instance.</returns>
        public static Error? ToError(
            this Exception exception, 
            int statusCode = 500, 
            string? systemCode = null, 
            string? title = null, 
            string? description = null, 
            string? category = null, 
            string? reference = null)
        {
            if (exception is null)
            {
                return null;
            }

            var cause = exception.InnerException.ToError();
            var type = exception.GetType();

            return new Error(
                statusCode: statusCode,
                systemCode: systemCode ?? type.Name,
                title: title ?? type.Name,
                description: description ?? exception.Message,
                category: category ?? type.Namespace,
                reference: reference,
                causedBy: cause);
        }
    }
}