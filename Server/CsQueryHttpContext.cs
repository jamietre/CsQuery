using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace Jtc.CsQuery.Server
{
    public class CsQueryHttpContext
    {
        protected CsQuery Owner=null;
        protected HttpContext Context
        {
            get
            {
                return _Context;
            }
            set
            {
                _Context = value;
            }
        } protected HttpContext _Context = null;
        public  SimpleDictionary<string> PostData
        {
            get
            {
                if (_PostData == null)
                {

                    _PostData = new SimpleDictionary<string>(Context.Request.Form);
                }
                return _PostData;
            }
        } protected SimpleDictionary<string> _PostData = null;
        public SimpleDictionary<string> GetData
        {
            get
            {
                if (_GetData == null)
                {

                    _GetData = new SimpleDictionary<string>(Context.Request.QueryString);
                }
                return _GetData;
            }
        } protected SimpleDictionary<string> _GetData = null;
        public CsQueryHttpContext(CsQuery owner,HttpContext context)
        {
            Context = context;
            Owner = owner;
            
        }
        
        protected void ParseContext()
        {
            //Context.Request.Form 
        }
        /// <summary>
        /// Repopulates all selected elements with their postback data (if any exists)
        /// </summary>
        public CsQuery RestorePost()
        {
            foreach (DomElement e in Owner.Find("[name]"))
            {
                string value;
                
                if (PostData.TryGetValue(e.Name,out value)) {
                    switch (e.Type)
                    {
                        case "checkbox":
                            e.SetAttribute("checked");
                            break;
                        default:
                            // TODO - why?
                            CsQuery.CreateFromElement(e).Val(value);
                            break;
                    }

                }
                

            }
            return Owner;
        }
    }
    public class SimpleDictionary<T> where T: class
    {
        public SimpleDictionary(NameValueCollection dataSource)
        {
            DataSource = dataSource;

        }
        protected NameValueCollection DataSource;
        protected Dictionary<string,T> InnerDict
        {
            get
            {
                if (_InnerDict == null)
                {
                    _InnerDict = new Dictionary<string, T>();
                }
                return _InnerDict;
            }
        } protected Dictionary<string,T> _InnerDict = new Dictionary<string,T>();
        public bool TryGetValue(string key, out T value)
        {
            T storedValue;
            if (InnerDict.TryGetValue(key, out storedValue)) {
                value = storedValue;
                return true;
            } else {
                string sourceValue = DataSource[key];
                if (sourceValue!=null) {
                    _InnerDict.Add(key,sourceValue as T);
                    value = sourceValue as T;
                    return true;
                }
            }
            value= default(T);
            return false;
        }
        public T GetValueOrDefault(string key)
        {
            return GetValueOrDefault(key, default(T));
        }
        public T GetValueOrDefault(string key, T defaultValue) {
            T value;
            if (TryGetValue(key,out value)) {
                return value;
            } else {
                return defaultValue;
            }
        }
    }

}
