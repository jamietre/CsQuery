using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Reflection;

namespace CsQuerySite.Helpers
{
    /// <summary>
    /// Represents XML documentation
    /// </summary>

    public class MemberXmlDoc: IReadOnlyDictionary<string,string>
    {
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
                if (node is XmlElement)
                {
                    var el = (XmlElement)node;
                    xmlDocs[node.Name] = node.Value;
                }
            }
        }

        private Dictionary<string, string> xmlDocs;
        
        public bool IsStatic { get; set; }

        public MemberInfo MemberInfo { get; protected set; }

        public Type ReturnType { get; protected set; }

        public MemberTypes MemberType { get { return MemberInfo.MemberType; } }

        public ParameterInfo[] Parameters
        {
            get;
            protected set;
        }
        public bool CanReadProperty { get; protected set; }
        public bool CanWriteProperty { get; protected set; }

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

                if (Parameters.Count == 0)
                {


                }

                
            }
        }

        private bool IsParamArray(ParameterInfo info)
        {
            foreach (var attr in info.Attributes)
            {
                if (attr is ParamArrayAttribute)
                {
                    return true;
                }
            }
            return false;

        }
        public string Name
        {
            get
            {
                return MemberInfo.Name;
            }
        }

        public bool ContainsKey(string key)
        {
            return xmlDocs.ContainsKey(key);
        }

        public IEnumerable<string> Keys
        {
            get { return xmlDocs.Keys; }
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