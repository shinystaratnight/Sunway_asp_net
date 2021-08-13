// Copyight © intuitive Ltd. All rights reserved
#nullable enable
namespace Intuitive
{
    /// <summary>
    /// Provides extensions for the <see cref="string"/> type.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Replaces one or more format items in a specified string with the string representation of the specified 
        /// object.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">The object to format.</param>
        /// <returns>
        /// A copy of format in which any format items are replaced by the string representation of arg0.
        /// </returns>
        public static string FormatWith(this string format, object? arg0)
            => string.Format(format, arg0);

        /// <summary>
        /// Replaces one or more format items in a specified string with the string representation of the two specified 
        /// objects.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">The first object to format.</param>
        /// <param name="arg1">The second object to format.</param>
        /// <returns>
        /// A copy of format in which any format items are replaced by the string representation of arg0 and arg1.
        /// </returns>
        public static string FormatWith(this string format, object? arg0, object? arg1)
            => string.Format(format, arg0, arg1);

        /// <summary>
        /// Replaces one or more format items in a specified string with the string representation of the three 
        /// specified objects.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">The first object to format.</param>
        /// <param name="arg1">The second object to format.</param>
        /// <param name="arg2">The third object to format.</param>
        /// <returns>
        /// A copy of format in which any format items are replaced by the string representation of arg0, arg1 and arg2.
        /// </returns>
        public static string FormatWith(this string format, object? arg0, object? arg1, object? arg2)
            => string.Format(format, arg0, arg1, arg2);

        /// <summary>
        /// Replaces one or more format items in a specified string with the string representation of a corresponding 
        /// object in a specified array.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>
        /// A copy of format in which the format items have been replaced by the string representation of the 
        /// corresponding objects in args.
        /// </returns>
        public static string FormatWith(this string format, params object[] args)
            => string.Format(format, args);
    }
}
