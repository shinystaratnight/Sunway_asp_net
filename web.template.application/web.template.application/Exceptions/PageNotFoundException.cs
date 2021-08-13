namespace Web.Template.Application.Exceptions
{
    using System;

    /// <summary>
    ///     An Exception used when a page can not be found
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class PageNotFoundException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PageNotFoundException" /> class.
        /// </summary>
        public PageNotFoundException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PageNotFoundException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public PageNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PageNotFoundException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The inner.</param>
        public PageNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}