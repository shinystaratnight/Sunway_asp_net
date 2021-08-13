namespace Web.TradeMMB.Modules
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Web;

    /// <summary>
    /// Class SVG Compression Module.
    /// </summary>
    public class SvgCompressionModule : IHttpModule
    {
        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule" />.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Initializes the specified application.
        /// </summary>
        /// <param name="application">The application.</param>
        public void Init(HttpApplication application)
        {
            application.BeginRequest += this.ApplicationOnBeginRequest;
        }

        /// <summary>
        /// Determines whether [is SVG request] [the specified request].
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns><c>true</c> if [is SVG request] [the specified request]; otherwise, <c>false</c>.</returns>
        protected virtual bool IsSvgRequest(HttpRequest request)
        {
            var path = request.Url.AbsolutePath;
            return Path.HasExtension(path)
                   && Path.GetExtension(path).Equals(".svg", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary> 
        /// Applications the on begin request.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ApplicationOnBeginRequest(object sender, EventArgs eventArgs)
        {
            var app = (HttpApplication)sender;
            if (app == null)
            {
                return;
            }

            var context = app.Context;
            if (!this.IsSvgRequest(context.Request))
            {
                return;
            }

            context.Response.Filter = new GZipStream(context.Response.Filter, CompressionMode.Compress);
            context.Response.AddHeader("Content-encoding", "gzip");
        }
    }
}