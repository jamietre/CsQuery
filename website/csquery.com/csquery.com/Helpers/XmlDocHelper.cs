using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace CsQuerySite.Helpers
{
    public static class XmlDocHelper
    {
        /// <summary>
        /// Gets the XML documentation for a method
        /// </summary>
        ///
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="methodName">
        /// Name of the method.
        /// </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process get XML document in this collection.
        /// </returns>

        public static IEnumerable<KeyValuePair<string, string>> GetXmlDoc<T>(string methodName)
        {
            return GetXmlDoc(typeof(T), methodName);
        }

        /// <summary>
        /// Gets the XML documentation for a method.
        /// </summary>
        ///
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="methodName">
        /// Name of the method.
        /// </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process get XML document in this collection.
        /// </returns>

        public static IEnumerable<KeyValuePair<string, string>> GetXmlDoc(Type type, string methodName)
        {
            
            XmlElement documentation = DocsByReflection.XMLFromMember(type.GetMethod(methodName));

           
        }

        /// <summary>
        /// Gets the XML documentation for a method.
        /// </summary>
        ///
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="methodName">
        /// Name of the method.
        /// </param>
        /// <param name="member">
        /// The member.
        /// </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process get XML document in this collection.
        /// </returns>

        public static string GetXmlDoc(Type type, string methodName, string member)
        {
            return GetXmlDoc(type, methodName).Where(item => item.Key == methodName)
                .FirstOrDefault().Value;
        }

        private static IEnumerable<KeyValuePair<string,string>> NodesToKvps(XmlElement documentation) {
            foreach (XmlNode node in documentation)
            {
                if (node is XmlElement)
                {
                    var el = (XmlElement)node;
                    yield return new KeyValuePair<string, string>(el.Name, el.Value);
                }
            }
        }
    }
}