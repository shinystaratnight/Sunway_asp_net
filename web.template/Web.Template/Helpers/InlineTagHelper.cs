namespace Web.Template.Helpers
{
    using System.IO;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Class HtmlHelperExtensions.
    /// </summary>
    public static class InlineTagHelper
    {
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