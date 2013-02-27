using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Reflection;

namespace CsQuerySite.Helpers.XmlDoc
{
    /// <summary>
    /// Represents XML documentation
    /// </summary>

    public class MemberXmlDoc: IReadOnlyDictionary<string,string>
    {
        #region constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        ///
        /// <param name="methodInfo">
        /// Information describing the method.
        /// </param>

        public MemberXmlDoc(MemberInfo methodInfo)
        {
            MemberInfo = methodInfo;
            
            // set return type
            // 
            if (methodInfo.MemberType == MemberTypes.Method)
            {
                var mi = (MethodInfo)methodInfo;
                ReturnType = mi.ReturnType;
                IsStatic = mi.IsStatic;
                Parameters = mi.GetParameters();
            }
            else if (methodInfo.MemberType == MemberTypes.Property)
            {
                var pi = (PropertyInfo)methodInfo;
                ReturnType = pi.PropertyType;
                IsStatic = pi.GetGetMethod().IsStatic;

                Parameters = new ParameterInfo[0];
                CanReadProperty = pi.GetGetMethod() != null;
                CanWriteProperty = pi.GetSetMethod() != null;
            }

            xmlDocs = new Dictionary<string, string>();
            var doc = DocsByReflection.XMLFromMember(methodInfo);

            foreach (XmlNode node in doc)
            {
                ProcessXml(node);
            }
        }

        private void ProcessXml(XmlNode node)
        {
            if (node is XmlElement)
            {
                var el = (XmlElement)node;
                if (el.Name == "member")
                {
                    foreach (XmlNode child in el.ChildNodes)
                    {
                        ProcessXml(child);
                    }
                }
                else
                {
                    xmlDocs[node.Name] = node.InnerText;
                }
            }

        }

        #endregion

        #region private properties

        private Dictionary<string, string> xmlDocs;

        private bool IsParamArray(ParameterInfo info)
        {
            return info.GetCustomAttribute(typeof(ParamArrayAttribute), true) != null;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Test whether this member is static
        /// </summary>

        public bool IsStatic { get; protected set; }

        /// <summary>
        /// Return the memberinfo object for this member
        /// </summary>

        public MemberInfo MemberInfo { get; protected set; }

        /// <summary>
        /// The return type for this property or method
        /// </summary>

        public Type ReturnType { get; protected set; }

        /// <summary>
        /// Gets the type of the member.
        /// </summary>

        public MemberTypes MemberType { get { return MemberInfo.MemberType; } }

        /// <summary>
        /// ParameterInfo array
        /// </summary>

        public ParameterInfo[] Parameters
        {
            get;
            protected set;
        }

        /// <summary>
        /// When true, the property is gettable
        /// </summary>

        public bool CanReadProperty { get; protected set; }

        /// <summary>
        /// When true, the property is settable
        /// </summary>

        public bool CanWriteProperty { get; protected set; }

        /// <summary>
        /// Gets or sets the signature of the function.
        /// </summary>
        ///
        /// <remarks>
        /// The signature is built from reflected info. The signature returns <c>formatted code</c> that
        /// should match the compilable method signature.
        /// </remarks>

        public string Signature
        {

            get
            {
                string sig = "public";
                if (IsStatic)
                {
                    sig += " static";
                }
                sig += " "+ReturnType.Name;
                sig += Name; 
                if (MemberType == MemberTypes.Property)
                {
                    sig += " {" + (CanReadProperty ? " get;" : "" ) + (CanWriteProperty ? " set;" : "") + " }";

                } else if (MemberType == MemberTypes.Method) {
                    sig += "(";
                    bool first=true;
                    foreach (var item in Parameters) {
                        if (!first)
                        {
                            sig += ",";
                        }
                        if (first)
                        {
                            first = false;
                        }
                        if (item.IsOut) 
                        {
                            sig += "out ";
                        }
                        if (IsParamArray(item))
                        {
                            sig += "params ";
                        }
                        sig += item.ParameterType.Name;
                        if (item.IsOptional)
                        {
                            sig += "=" + item.DefaultValue;
                        }
                        

                    }
                    sig += ")";
                } else {
                    throw new InvalidOperationException("Unsupported member type.");
                }
                return sig;
            }
        }


        public string Name
        {
            get
            {
                return MemberInfo.Name;
            }
        }

        /// <summary>
        /// Gets the keys of the XML docs for this member; e.g. the node names
        /// </summary>

        public IEnumerable<string> Keys
        {
            get { return xmlDocs.Keys; }
        }

        #endregion

        public bool ContainsKey(string key)
        {
            return xmlDocs.ContainsKey(key);
        }

       

        public bool TryGetValue(string key, out string value)
        {
            return xmlDocs.TryGetValue(key, out value);
        }

        public IEnumerable<string> Values
        {
            get { return xmlDocs.Values; }
        }

        public string this[string key]
        {
            get { return xmlDocs[key]; }
        }

        public int Count
        {
            get { return xmlDocs.Count; }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}