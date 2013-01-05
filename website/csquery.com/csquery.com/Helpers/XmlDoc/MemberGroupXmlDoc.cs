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

    public class MemberGroupXmlDoc: IEnumerable<MemberXmlDoc>
    {
        public MemberGroupXmlDoc(Type type, string methodName)
        {
            DeclaringType = type;


            memberXmlDocs = new List<MemberXmlDoc>();
            foreach (var mi in type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static))
            {

                if (mi.DeclaringType != null 
                    && mi.DeclaringType != typeof(object) 
                    && mi.Name == methodName 
                    && (mi is PropertyInfo || mi is MethodInfo))
                {
                    //members.Add(mi);
                    memberXmlDocs.Add(new MemberXmlDoc(mi));

                    if (MemberType == 0)
                    {
                        MemberType = mi.MemberType;
                        ReturnType = memberXmlDocs[0].ReturnType;
                        IsStatic = memberXmlDocs[0].IsStatic;
                    }

                }

            }


         
        }
        public bool IsStatic { get; protected set; }
        private List<MemberXmlDoc> memberXmlDocs;
        
        public MemberTypes MemberType {get; protected set;}
        /// <summary>
        /// The type to which the method belongs
        /// </summary>

        public Type DeclaringType
        {
            get;
            protected set;
        }

        /// <summary>
        /// The return type of the method
        /// </summary>

        public Type ReturnType { get; protected set;}
     

    
        public IEnumerator<MemberXmlDoc> GetEnumerator()
        {
            return memberXmlDocs.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        }
}