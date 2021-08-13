namespace Web.Template.Application.Tests.Net.Logging
{
    using System;
    using System.Net;

    using Intuitive;

    using Moq;

    using NUnit.Framework;

    using Web.Template.Application.Interfaces.Logging;
    using Web.Template.Application.Net.Logging;

    /// <summary>
    ///     Test the web request logger
    /// </summary>
    [TestFixture]
    public class WebRequestLoggerTests
    {
        /// <summary>
        ///     Logs the exception.
        /// </summary>
        [Test]
        public void Log_Exception()
        {
            const string RequestFormattedMessage = "Request formatted message";
            const string ExceptionFormattedMessage = "Exception formatted message";

            var webRequestMock = new Mock<WebRequest>();
            webRequestMock.Setup(x => x.RequestUri).Returns(new Uri("http://google.com/"));

            var webExceptionMock = new Mock<WebException>();

            var requestFormatterMock = new Mock<WebRequestLogFormatter>();
            requestFormatterMock.Setup(x => x.Format(webRequestMock.Object, RequestFormattedMessage)).Returns(RequestFormattedMessage);

            var exceptionFormatterMock = new Mock<WebExceptionLogFormatter>();
            exceptionFormatterMock.Setup(x => x.Format(webExceptionMock.Object)).Returns(ExceptionFormattedMessage);

            var logWriterMock = new Mock<ILogWriter>();

            var logger = new WebRequestLogger(requestFormatterMock.Object, exceptionFormatterMock.Object, logWriterMock.Object);

            logger.Log(webRequestMock.Object, RequestFormattedMessage, webExceptionMock.Object);

            logWriterMock.Verify(x => x.Write("Web Request Exception", "Request to http://google.com/", string.Concat(RequestFormattedMessage, Environment.NewLine, ExceptionFormattedMessage)));
        }
    }
}