using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.IO;
using Jtc.ExtensionMethods;

namespace Jtc.CsQuery.Server
{
    public class CsQueryHttpContext
    {
        public bool AspNet { get; set; }
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
                
                if (PostData.TryGetValue(e["name"],out value)) {
                    switch (e["type"])
                    {
                        case "checkbox":
                            e.SetAttribute("checked");
                            break;
                        default:
                            // TODO - why?
                            
                            Owner.Select(e).Val(value);
                            break;
                    }

                }
                

            }
            return Owner;
        }
        public HtmlTextWriter Writer
        {
            get
            {
                if (_Writer == null)
                {
                   _sb = new StringBuilder();
                    _sw = new StringWriter(_sb);

                    _Writer = new HtmlTextWriter(_sw);
                }
                return _Writer;
            }
        } protected HtmlTextWriter _Writer;
        protected StringBuilder _sb;
        protected StringWriter _sw;
        public void CreateFromWriter()
        {


            ScriptManager mgr = ScriptManager.GetCurrent(CurrentPage);

            // Asp.Net async postbacks structure data like:
            // "Len | Type | ID | Content" is the format of each asp.net postback
            // Len must match length of Content or it chokes. 

            if (mgr != null && mgr.IsInAsyncPostBack)
            {
                _AsyncPostbackData = new List<AsyncPostbackData>();
                string input =_sb.ToString();
                int inputLength = input.Length;
                string id=String.Empty;
                string type=String.Empty;
                int length=0;

                int pos = 0;
                int step = 1;
                while (pos < inputLength)
                {
                    if (step < 4)
                    {
                        int nextPos = input.IndexOf('|', pos);
                        if (nextPos > inputLength)
                        {
                            throw new Exception("Unable to parse UpdatePanel data");
                        }
                        string data = input.SubstringBetween(pos, nextPos);
                        switch (step)
                        {
                            case 1:
                                length = Convert.ToInt32(data);
                                break;
                            case 2:
                                type = data;
                                break;
                            case 3:
                                id = data;
                                break;
                        }
                        step++;
                        pos = nextPos + 1;
                    }
                    else
                    {
                        AsyncPostbackData postData = new AsyncPostbackData();
                        postData.Create(length, type, id, input.Substring(pos,length));
                        pos += length + 1;
                        step = 1;
                        _AsyncPostbackData.Add(postData);
                    }
                }
            }
            else
            {
                Owner.CreateNew(_sb.ToString());
            }
        }
        HtmlTextWriter RealWriter;
        protected Page CurrentPage;

        /// <summary>
        /// Creates a new CSQuery object from a Page.Render method. The base Render method of a page should be overridden,
        /// and this called from inside it to configure the CsQUery
        /// </summary>
        /// <param name="page">The current System.Web.UI.Page</param>
        /// <param name="renderMethod">The delegate to the base render method</param>
        /// <param name="writer">The HtmlTextWriter to output the final stream (the parameter passed to the Render method)</param>
        public void CreateFromRender(Page page,Action<HtmlTextWriter> renderMethod, HtmlTextWriter writer)
        {
            RealWriter = writer;
            CurrentPage = page;
            renderMethod(Writer);
            CreateFromWriter();
        }


        protected List<AsyncPostbackData> _AsyncPostbackData;
        public IEnumerable<AsyncPostbackData> AsyncPostbackData
        {
            get
            {
                foreach (AsyncPostbackData data in _AsyncPostbackData)
                {
                    if (data.DataType.ToLower() == "updatepanel")
                    {
                        yield return data;
                    }
                }
            }
        }
        public void Render()
        {
            //ScriptManager mgr = ScriptManager.GetCurrent(CurrentPage);

            //// Asp.Net async postbacks structure data like:
            //// "Len | Type | ID | Content" is the format of each asp.net postback
            //// Len must match length of Content or it chokes. 

            //if (mgr != null && mgr.IsInAsyncPostBack)
            //{
            //    string output = Owner.Render();
            //    int index=0;
            //    string[] parts = SplitQuotedString(output,'|');
            //    while (index+3 < parts.Length)
            //    {
            //        parts[index] = parts[index + 3].Length.ToString();
            //        index+=4;
            //    }
            //    for (int i = 0; i < parts.Length; i++)
            //    {
            //        RealWriter.Write(parts[i]+ (i<parts.Length-1 ? "|" : String.Empty));
            //    }
            //    RealWriter.Write(output);
            //} else {
            //    RealWriter.Write(Owner.Render());
            //}
            if (_AsyncPostbackData != null)
            {
                foreach (var data in _AsyncPostbackData)
                {
                    RealWriter.Write(data.Render());
                }
            }
            else
            {
                RealWriter.Write(Owner.Render());
            }
        }
        
        
        protected static string[] SplitQuotedString(string stringToSplit, char token)
        {
            int count = 0;
            List<string> valueList = new List<string>();
            StringBuilder sb = new StringBuilder();
            bool inQuotes = false;
            char closeQuoteChar = '"';

            foreach (Char character in stringToSplit)
            {
                if (character == '\\')
                {
                    continue;
                }
                if (!inQuotes) {
                    switch(character) {
                        case '"':
                        case '\'':
                        case '(':
                            inQuotes = !inQuotes;
                            closeQuoteChar = character == '(' ? ')': character;
                            break;
                    }
                }
                else if (character == closeQuoteChar)
                {
                    inQuotes = false;
                }

                if (!inQuotes && character == token)
                {
                    // check if in 1st position, if so, double-check by trying to parse it. If its not an int then continue on as if nothing happened.
                    // Since this data is HTML is could be messy - this is not a perfect algorithm but it should be rare for it to fail with typical markup
                    if (count>0 && count % 4==0)
                    {
                        int val;
                        if (!int.TryParse(sb.ToString(),out val)) {
                            valueList[valueList.Count - 1] += token + sb.ToString();
                            continue;
                        }
                    }
                    count++;
                    valueList.Add(sb.ToString());
                    sb = new StringBuilder();
                }
                else
                    sb.Append(character);
            }

            if (sb.Length > 0)
                valueList.Add(sb.ToString());

            return valueList.ToArray();
        }
    

    }
    public class AsyncPostbackData
    {
        public void Create(int length, string type, string id, string content)
        {
            Content = content;
            ID = id;
            Length = length;
            DataType = type;
        }
        public string Render()
        {
            string content = _Dom != null ? Dom.Render() : Content;
            return content.Length.ToString() + "|" + DataType + "|" + ID + "|" + content + "|";

        }
        public string Content;
        public CsQuery Dom
        {
            get
            {
                if (_Dom == null)
                {
                    _Dom = CsQuery.Create(Content);
                }
                return _Dom;
            }
        }
        protected CsQuery _Dom = null;
        public string ID
        {
            get
            {
                return _ID;
            }
            protected set {
                _ID = value;
            }
        }
        protected string _ID;
        protected int Length;
        public string DataType
        {
            get
            {
                return _DataType;
            }
            protected set
            {
                _DataType = value;
            }
        }
        protected string _DataType;

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
