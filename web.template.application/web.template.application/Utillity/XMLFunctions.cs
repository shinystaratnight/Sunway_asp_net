namespace Web.Template.Application.Utillity
{
    using System;
    using System.Collections.Generic;
    using System.Xml;

    using Intuitive;

    /// <summary>
    /// Utility functions for working with xml
    /// </summary>
    public class XMLFunctions
    {
        /// <summary>
        /// XMLs to generic list.
        /// </summary>
        /// <typeparam name="T">The type to serialize to</typeparam>
        /// <param name="xmlDocument">The XML document.</param>
        /// <returns>
        /// A List of T serialized from the provided xml
        /// </returns>
        public static List<T> XMLToGenericList<T>(XmlDocument xmlDocument)
        {
            var list = new List<T>();
            try
            {
                if (xmlDocument.ChildNodes.Count > 0)
                {
                    XmlNodeList nodeList = xmlDocument.ChildNodes[0].ChildNodes;
                    foreach (XmlNode childNode in nodeList)
                    {
                        T item = (T)Serializer.DeSerialize(typeof(T), childNode.OuterXml);
                        list.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return list;
        }

        /// <summary>
        /// XMLs to generic list.
        /// </summary>
        /// <typeparam name="T">The type to serialize to</typeparam>
        /// <param name="xmlDocument">The XML document.</param>
        /// <param name="xPath">The x path.</param>
        /// <returns>
        /// A List of T serialized from the provided xml
        /// </returns>
        public static List<T> XMLToGenericList<T>(XmlDocument xmlDocument, string xPath)
        {
            var list = new List<T>();
            try
            {
                if (xmlDocument.SelectSingleNode(xPath) != null)
                {
                    XmlNodeList nodeList = xmlDocument.SelectNodes(xPath);
                    if (nodeList != null)
                    {
                        foreach (XmlNode childNode in nodeList)
                        {
                            T item = (T)Serializer.DeSerialize(typeof(T), childNode.OuterXml);
                            list.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return list;
        }
    }
}