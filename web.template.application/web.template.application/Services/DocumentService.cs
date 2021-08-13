namespace Web.Template.Application.Services
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Xml;

    using Intuitive.WebControls;

    /// <summary>
    /// The DocumentService interface.
    /// </summary>
    public interface IDocumentService
    {
        /// <summary>
        /// The get document.
        /// </summary>
        /// <param name="template">
        /// The XSL template used to create the document.
        /// </param>
        /// <param name="filename">
        /// The desired filename that the document will be saved as.
        /// </param>
        /// <param name="runUrl">
        /// The url of the document generator which will be used to generate the document.
        /// </param>
        /// <param name="parameters">
        /// The XSL parameters that will be used in the XSL template.
        /// </param>
        /// <param name="xml">
        /// The xml.
        /// </param>
        /// <returns>
        /// The <see cref="DocumentServiceReturn"/>.
        /// </returns>
        DocumentServiceReturn GetDocument(string template, string filename, string runUrl, XSL.XSLParams parameters, XmlDocument xml);

        /// <summary>
        /// The get document.
        /// </summary>
        /// <param name="xslTemplate">
        /// The XSL template.
        /// </param>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="runUrl">
        /// The run url.
        /// </param>
        /// <returns>
        /// The <see cref="DocumentServiceReturn"/>.
        /// </returns>
        DocumentServiceReturn GetDocument(XSL xslTemplate, string filename, string runUrl);

        /// <summary>
        /// The get document.
        /// </summary>
        /// <param name="xslTemplate">
        /// The XSL template.
        /// </param>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="runUrl">
        /// The run url.
        /// </param>
        /// <param name="type">
        /// the filetype of the poster to be generated.
        /// </param>
        /// <returns>
        /// The <see cref="DocumentServiceReturn"/>.
        /// </returns>
        DocumentServiceReturn GetDocument(XSL xslTemplate, string filename, string runUrl, string type);
    }

    /// <summary>
    /// The document service.
    /// </summary>
    public class DocumentService : IDocumentService
    {
        /// <summary>
        /// The get document.
        /// </summary>
        /// <param name="template">
        /// The XSL template used to create the document.
        /// </param>
        /// <param name="filename">
        /// The desired filename that the document will be saved as.
        /// </param>
        /// <param name="runUrl">
        /// The url of the document generator which will be used to generate the document.
        /// </param>
        /// <param name="parameters">
        /// The XSL parameters that will be used in the XSL template.
        /// </param>
        /// <param name="xml">
        /// The xml.
        /// </param>
        /// <returns>
        /// The <see cref="DocumentServiceReturn"/>.
        /// </returns>
        public DocumentServiceReturn GetDocument(string template, string filename, string runUrl, XSL.XSLParams parameters, XmlDocument xml)
        {
            var xslTemplate = new XSL { XSLTemplate = template, XMLDocument = xml, XSLParameters = parameters };

            return this.GetDocument(xslTemplate, filename, runUrl);
        }

        /// <summary>
        /// The get document.
        /// </summary>
        /// <param name="xslTemplate">
        /// The XSL template.
        /// </param>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="runUrl">
        /// The run url.
        /// </param>
        /// <returns>
        /// The <see cref="DocumentServiceReturn"/>.
        /// </returns>
        public DocumentServiceReturn GetDocument(XSL xslTemplate, string filename, string runUrl)
        {
            return GetDocument(xslTemplate, filename, runUrl, "pdf");
        }

        /// <summary>
        /// The get document.
        /// </summary>
        /// <param name="xslTemplate">
        /// The XSL template.
        /// </param>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="runUrl">
        /// The run url.
        /// </param>
        /// <param name="type">
        /// The filetype of the poster to be generated.
        /// </param>
        /// <returns>
        /// The <see cref="DocumentServiceReturn"/>.
        /// </returns>
        public DocumentServiceReturn GetDocument(XSL xslTemplate, string filename, string runUrl, string type)
        {
            var docReturn = new DocumentServiceReturn();
            bool success;
            try
            {
                var html = xslTemplate.Generate();
                var bytes = Encoding.UTF8.GetBytes(html);

                HttpWebRequest request = (HttpWebRequest)this.GenerateRequest(bytes, runUrl, type);
                

                var responseStream = request.GetResponse().GetResponseStream();
                Directory.CreateDirectory(Path.GetDirectoryName(filename));

                using (FileStream fileStream = File.Create(filename))
                {
                    if (responseStream != null)
                    {
                        responseStream.CopyTo(fileStream);
                        success = true;
                    }
                    else
                    {
                        success = false;
                    }
                }
            }
            catch (Exception ex)
            {
                docReturn.Warnings.Add(ex.ToString());
                success = false;
            }

            docReturn.Success = success;
            if (success)
            {
                docReturn.DocumentUrl = filename;
            }

            return docReturn;
        }

        /// <summary>
        /// The generate request.
        /// </summary>
        /// <param name="bytes">
        /// The bytes.
        /// </param>
        /// <param name="runUrl">
        /// The run url.
        /// </param>
        /// <param name="type">
        /// The filetype of the poster to be generated.
        /// </param>
        /// <returns>
        /// The <see cref="WebRequest"/>.
        /// </returns>
        private WebRequest GenerateRequest(byte[] bytes, string runUrl, string type)
        {
            WebRequest request = WebRequest.Create(runUrl);
            request.Method = "POST";
            request.ContentType = "text/plain; charset=UTF-8";
            request.ContentLength = bytes.Length;

            HttpWebRequest httpRequest = (HttpWebRequest)request;
            string headerType;

            switch (type)
            {
                case "pdf":
                    headerType = "application/pdf";
                    break;
                case "jpg":
                    headerType = "image/jpg";
                    break;
                default:
                    headerType = "application/pdf";
                    break;
            }

            httpRequest.Accept = headerType;

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            return httpRequest;
        }
    }
}