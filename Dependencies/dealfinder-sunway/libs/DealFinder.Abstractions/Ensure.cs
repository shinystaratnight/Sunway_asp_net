// Copyight © intuitive Ltd. All rights reserved

namespace Intuitive
{
    using System;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides argument validation methods.
    /// </summary>
    #nullable enable
    public static class Ensure
    {
        /// <summary>
        /// Ensures the given argument is within the given range.
        /// </summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="argument">The argument value.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <param name="errorMessage">[Optional] The error message to report.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the argument value is outof the given range.</exception>
        /// <returns>The argument value.</returns>
        [DebuggerStepThrough]
        public static TValue IsInRange<TValue>(
            TValue argument,
            TValue min,
            TValue max,
            string parameterName,
            string? errorMessage = null)
            where TValue : struct, IComparable
        {
            if (argument.CompareTo(min) < 0 || argument.CompareTo(max) > 0)
            {
                if (errorMessage is object)
                {
                    string actualErrorMessage = errorMessage.FormatWith(argument, min, max, parameterName);
                    throw new ArgumentOutOfRangeException(parameterName, actualErrorMessage);
                }

                throw new ArgumentOutOfRangeException(parameterName);
            }

            return argument;
        }

        /// <summary>
        /// Ensures the given argument is not null.
        /// </summary>
        /// <typeparam name="TArgument">The argument type.</typeparam>
        /// <param name="argument">The argument value.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <param name="errorMessage">[Optional] The error message to report.</param>
        /// <exception cref="ArgumentNullException">If the argument value is null or an empty string.</exception>
        /// <returns>The argument value.</returns>
        [DebuggerStepThrough]
        public static TArgument IsNotNull<TArgument>(
            TArgument? argument,
            string parameterName,
            string? errorMessage = null)
            where TArgument : class
        {
            if (argument is null)
            {
                if (errorMessage is object)
                {
                    string actualErrorMessage = errorMessage.FormatWith(parameterName);
                    throw new ArgumentNullException(parameterName, actualErrorMessage);
                }

                throw new ArgumentNullException(parameterName);
            }

            return argument!;
        }

        /// <summary>
        /// Ensures the given argument is not null or an empty string.
        /// </summary>
        /// <param name="argument">The argument value.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <param name="errorMessage">[Optional] The error message to report.</param>
        /// <exception cref="ArgumentException">If the argument value is null or an empty string.</exception>
        /// <returns>The argument value.</returns>
        [DebuggerStepThrough]
        public static string IsNotNullOrEmpty(string? argument, string parameterName, string? errorMessage = null)
        {
            if (string.IsNullOrEmpty(argument))
            {
                string actualErrorMessage = (errorMessage ?? CoreAbstractionsResources.IsNotNullOrEmptyExceptionMessage)
                    .FormatWith(parameterName);

                throw new ArgumentException(
                    actualErrorMessage,
                    parameterName);
            }

            return argument!;
        }

        /// <summary>
        /// Ensures the given argument is not null or an empty set.
        /// </summary>
        /// <typeparam name="TElement">The element type.</typeparam>
        /// <param name="argument">The argument value.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <param name="errorMessage">[Optional] The error message to report.</param>
        /// <exception cref="ArgumentException">If the argument value is null or an empty set.</exception>
        /// <returns>The argument value.</returns>
        public static IEnumerable<TElement> IsNotNullOrEmpty<TElement>(
            IEnumerable<TElement> argument,
            string parameterName,
            string? errorMessage = null)
        {
            IsNotNull(argument, parameterName);

            int length;
            if (argument is TElement[] array)
            {
                length = array.Length;
            }
            else if (argument is IList<TElement> list)
            {
                length = list.Count;
            }
            else
            {
                length = argument.Count();
            }

            if (length == 0)
            {
                string actualErrorMessage = (errorMessage ?? CoreAbstractionsResources.IsNotNullOrEmptySetExceptionMessage)
                    .FormatWith(parameterName);

                throw new ArgumentException(
                    actualErrorMessage,
                    parameterName);
            }

            return argument;
        }
    }
}
