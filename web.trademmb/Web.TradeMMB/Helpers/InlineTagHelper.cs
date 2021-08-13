namespace Web.TradeMMB.Helpers
{
    using System.IO;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Class HtmlHelperExtensions.
    /// </summary>
    public static class InlineTagHelper
    {
        /// <summary>
        /// Inlines the external styles.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="bundleUrl">The bundle URL.</param>
        /// <returns>
        /// IHtmlString.
        /// </returns>
        public static IHtmlString InlineExternalStyles(this HtmlHelper htmlHelper, string bundleUrl)
        {
            string htmlTag = string.Empty;
            using (WebClient client = new WebClient())
            {
                try
                {
                    string bundleContent = client.DownloadString(bundleUrl);
                    if (!string.IsNullOrWhiteSpace(bundleContent))
                    {
                        htmlTag = $"<style>{bundleContent}</style>";
                    }
                }
                catch (WebException)
                {
                }
            }

            return new HtmlString(htmlTag);
        }

        /// <summary>
        /// Adds the script inline.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="bundleVirtualPath">The bundle virtual path.</param>
        /// <returns>
        /// IHtmlString.
        /// </returns>
        public static IHtmlString InlineScript(this HtmlHelper htmlHelper, string bundleVirtualPath)
        {
            string bundleContent = LoadFileContent(htmlHelper.ViewContext.HttpContext, bundleVirtualPath);
            string htmlTag = $"<script>{bundleContent}</script>";

            return new HtmlString(htmlTag);
        }

        /// <summary>
        /// Inline the styles.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="bundleVirtualPath">The bundle virtual path.</param>
        /// <returns>HTML string containing inline styles.</returns>
        public static IHtmlString InlineStyles(this HtmlHelper htmlHelper, string bundleVirtualPath)
        {
            string bundleContent = LoadFileContent(htmlHelper.ViewContext.HttpContext, bundleVirtualPath);
            string htmlTag = $"<style>{bundleContent}</style>";

            return new HtmlString(htmlTag);
        }

        /// <summary>
        /// Loads the content of the file.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="bundleVirtualPath">The bundle virtual path.</param>
        /// <returns>a System.String.</returns>
        private static string LoadFileContent(HttpContextBase httpContext, string bundleVirtualPath)
        {
            var content = string.Empty;
            if (File.Exists(httpContext.Server.MapPath(bundleVirtualPath)))
            {
                content = File.ReadAllText(httpContext.Server.MapPath(bundleVirtualPath));
            }

            return content;
        }
    }
}