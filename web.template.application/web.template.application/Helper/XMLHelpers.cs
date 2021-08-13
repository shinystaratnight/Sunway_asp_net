namespace Web.Template.Application.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Xml;

    using Intuitive;

    /// <summary>
    /// class for xml helpers
    /// </summary>
    public static class XmlHelpers
    {
        /// <summary>
        ///     takes an xml document which will contain a serialized generic list, and turns it into a generic list.
        /// </summary>
        /// <typeparam name="T">The type of object we want to cast each xml node to</typeparam>
        /// <param name="xml">The XML.</param>
        /// <returns> a list of the type passed in</returns>
        public static List<T> XmlToGenericList<T>(XmlDocument xml) where T : class
        {
            var list = new List<T>();
            try
            {
                if (xml.ChildNodes.Count > 0)
                {
                    XmlNodeList nodeList = xml.ChildNodes[0].ChildNodes;
                    foreach (XmlNode node in nodeList)
                    {
                        var item = Serializer.DeSerialize<T>(node.OuterXml);
                        list.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                FileFunctions.AddLogEntry("iVectorConnect/xmlToGenericList", "Error", ex.ToString());
            }

            return list;
        }
    }
}