namespace Web.Template.Application.Tests.Net.Logging
{
    using System;
    using System.Net;
    using System.Text;

    using NUnit.Framework;

    using Web.Template.Application.Net.Logging;

    /// <summary>
    ///     Test for the Web Request log formatter class
    /// </summary>
    [TestFixture]
    public class WebRequestLogFormatterTests
    {
        /// <summary>
        ///     Format_s the argument null exception with null reference.
        /// </summary>
        [Test]
        public void Format_ArgumentNullExceptionWithNullReference()
        {
            var formatter = new WebRequestLogFormatter();

            Assert.Throws<ArgumentNullException>(() => formatter.Format(null, string.Empty));
        }

        /// <summary>
        ///     Format_s the request with headers and body.
        /// </summary>
        [Test]
        public void Format_RequestWithHeadersAndBody()
        {
            var formatter = new WebRequestLogFormatter();
            var fakeWebRequest = new FakeWebRequest();
            fakeWebRequest.SetRequestUri(new Uri("http://google.com/"));

            fakeWebRequest.Headers.Add("key", "value");
            fakeWebRequest.Headers.Add("key2", "value2");

            var body = "Sample body";

            var expectedReturn = new StringBuilder();
            expectedReturn.AppendLine("URL: http://google.com/");
            expectedReturn.AppendLine("Headers:");
            expectedReturn.AppendLine("key - value");
            expectedReturn.AppendLine("key2 - value2");
            expectedReturn.AppendLine("Body:");
            expectedReturn.AppendLine(body);

            Assert.AreEqual(formatter.Format(fakeWebRequest, body), expectedReturn.ToString());
        }

        /// <summary>
        ///     Format_s the request with headers and no body.
        /// </summary>
        [Test]
        public void Format_RequestWithHeadersAndNoBody()
        {
            var formatter = new WebRequestLogFormatter();
            var fakeWebRequest = new FakeWebRequest();
            fakeWebRequest.SetRequestUri(new Uri("http://google.com/"));

            fakeWebRequest.Headers.Add("key", "value");
            fakeWebRequest.Headers.Add("key2", "value2");

            var expectedReturn = new StringBuilder();
            expectedReturn.AppendLine("URL: http://google.com/");
            expectedReturn.AppendLine("Headers:");
            expectedReturn.AppendLine("key - value");
            expectedReturn.AppendLine("key2 - value2");
            expectedReturn.AppendLine("Body:");
            expectedReturn.AppendLine(string.Empty);

            Assert.AreEqual(formatter.Format(fakeWebRequest, null), expectedReturn.ToString());
        }

        /// <summary>
        ///     Format_s the request without headers or body.
        /// </summary>
        [Test]
        public void Format_RequestWithoutHeadersOrBody()
        {
            var formatter = new WebRequestLogFormatter();
            var fakeWebRequest = new FakeWebRequest();
            fakeWebRequest.SetRequestUri(new Uri("http://google.com/"));

            var expectedReturn = new StringBuilder();
            expectedReturn.AppendLine("URL: http://google.com/");
            expectedReturn.AppendLine("Headers:");
            expectedReturn.AppendLine("Body:");
            expectedReturn.AppendLine(string.Empty);

            Assert.AreEqual(formatter.Format(fakeWebRequest, null), expectedReturn.ToString());
        }

        /// <summary>
        ///     A fake web request used in testing
        /// </summary>
        /// <seealso cref="System.Net.WebRequest" />
        private class FakeWebRequest : WebRequest
        {
            /// <summary>
            ///     The headers
            /// </summary>
            private WebHeaderCollection headers;

            /// <summary>
            ///     The request URI
            /// </summary>
            private Uri requestUri;

            /// <summary>
            ///     Initializes a new instance of the <see cref="FakeWebRequest" /> class.
            /// </summary>
            public FakeWebRequest()
            {
                this.headers = new WebHeaderCollection();
            }

            /// <summary>
            ///     When overridden in a descendant class, gets or sets the collection of header name/value pairs associated with the
            ///     request.
            /// </summary>
            /// <PermissionSet>
            ///     <IPermission
            ///         class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
            ///         version="1" Flags="UnmanagedCode, ControlEvidence" />
            /// </PermissionSet>
            public override WebHeaderCollection Headers
            {
                get
                {
                    return this.headers;
                }

                set
                {
                    this.headers = value;
                }
            }

            /// <summary>
            ///     When overridden in a descendant class, gets the URI of the Internet resource associated with the request.
            /// </summary>
            /// <PermissionSet>
            ///     <IPermission
            ///         class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
            ///         version="1" Flags="UnmanagedCode, ControlEvidence" />
            /// </PermissionSet>
            public override Uri RequestUri => this.requestUri;

            /// <summary>
            ///     Sets the request URI.
            /// </summary>
            /// <param name="uri">The URI.</param>
            public void SetRequestUri(Uri uri)
            {
                this.requestUri = uri;
            }
        }
    }
}