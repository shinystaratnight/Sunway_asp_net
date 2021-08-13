namespace Web.Template.Application.Tests.Net.Logging
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;

    using NUnit.Framework;

    using Web.Template.Application.Net.Logging;

    /// <summary>
    ///     Test for Web Exception Log formatter class
    /// </summary>
    [TestFixture]
    public class WebExceptionLogFormatterTests
    {
        /// <summary>
        ///     Fake web response used for testing
        /// </summary>
        /// <seealso cref="System.Net.WebResponse" />
        private class FakeWebResponse : WebResponse
        {
            /// <summary>
            ///     Gets or sets the response message.
            /// </summary>
            /// <value>
            ///     The response message.
            /// </value>
            public string ResponseMessage { get; set; }

            /// <summary>
            ///     When overridden in a descendant class, returns the data stream from the Internet resource.
            /// </summary>
            /// <returns>
            ///     An instance of the <see cref="T:System.IO.Stream" /> class for reading data from the Internet resource.
            /// </returns>
            /// <PermissionSet>
            ///     <IPermission
            ///         class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
            ///         version="1" Flags="UnmanagedCode, ControlEvidence" />
            /// </PermissionSet>
            public override Stream GetResponseStream()
            {
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                writer.Write(this.ResponseMessage);
                writer.Flush();
                stream.Position = 0;
                return stream;
            }
        }

        /// <summary>
        ///     Format_s the argument null exception with null reference.
        /// </summary>
        [Test]
        public void Format_ArgumentNullExceptionWithNullReference()
        {
            var formatter = new WebExceptionLogFormatter();
            Assert.Throws<ArgumentNullException>(() => formatter.Format(null));
        }

        /// <summary>
        ///     Format_s the unknown error request no body.
        /// </summary>
        [Test]
        public void Format_UnknownErrorRequestNoBody()
        {
            var exception = new WebException("test exception", WebExceptionStatus.UnknownError);
            var formatter = new WebExceptionLogFormatter();

            var expectedOutput = new StringBuilder();
            expectedOutput.AppendLine("Status Code - UnknownError");
            expectedOutput.AppendLine("Message - test exception");

            Assert.AreEqual(formatter.Format(exception), expectedOutput.ToString());
        }

        /// <summary>
        ///     Format_s the unknown error request with body.
        /// </summary>
        [Test]
        public void Format_UnknownErrorRequestWithBody()
        {
            var fakeWebResponse = new FakeWebResponse();
            fakeWebResponse.ResponseMessage = "test response message";

            var exception = new WebException("test exception", new Exception(), WebExceptionStatus.UnknownError, fakeWebResponse);

            var formatter = new WebExceptionLogFormatter();

            var expectedOutput = new StringBuilder();
            expectedOutput.AppendLine("Status Code - UnknownError");
            expectedOutput.AppendLine("Message - test exception");
            expectedOutput.AppendLine("Response - test response message");

            Assert.AreEqual(formatter.Format(exception), expectedOutput.ToString());
        }
    }
}