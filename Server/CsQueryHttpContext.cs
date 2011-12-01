using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Threading;
using Jtc.CsQuery.ExtensionMethods;
using System.Web.Script.Serialization;

namespace Jtc.CsQuery.Server
{
    public class CsQueryHttpContext
    {
        public bool AspNet { get; set; }
        public CsQuery Owner = null;
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
        public SimpleDictionary<string> PostData
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
        public CsQueryHttpContext(CsQuery owner, HttpContext context)
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
            foreach (IDomElement e in Owner.Select("[name], textarea"))
            {
                string value;
                if (PostData.TryGetValue(e["name"], out value))
                {
                    switch (e.NodeName)
                    {
                        case "textarea":
                            e.InnerText = value;
                            break;
                        case "input":
                            switch (e["type"])
                            {
                                case "checkbox":
                                    e.SetAttribute("checked");
                                    break;
                                case "select":
                                    Owner[e].Val(value);
                                    break;
                                default:
                                    e.SetAttribute("value", value);
                                    break;
                            }
                            break;
                        default:
                            throw new Exception("What input tag isn't textarea, select or input?");
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

        public void CreateFromUrl(string url)
        {
            CsqWebRequest con = new CsqWebRequest();
            con.Get(url);
            Owner.Load(con.Html);
        }
        /// <summary>
        /// Queue an asynchronous request for data from a URL
        /// </summary>
        /// <param name="url"></param>
        public static void QueueAsyncRequest(string url)
        {
            CsqWebRequest req = new CsqWebRequest();
            req.Url = url;

        }
        public void CreateFromWriter()
        {


            ScriptManager mgr = ScriptManager.GetCurrent(CurrentPage);

            // Asp.Net async postbacks structure data like:
            // "Len | Type | ID | Content" is the format of each asp.net postback
            // Len must match length of Content or it chokes. 

            if (mgr != null && mgr.IsInAsyncPostBack)
            {
                _AsyncPostbackData = new List<AsyncPostbackData>();
                string input = _sb.ToString();
                int inputLength = input.Length;
                string id = String.Empty;
                string type = String.Empty;
                int length = 0;

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
                        postData.Create(length, type, id, input.Substring(pos, length));
                        pos += length + 1;
                        step = 1;
                        _AsyncPostbackData.Add(postData);
                    }
                }
            }
            else
            {
                Owner.Load(_sb.ToString());
            }
        }
        HtmlTextWriter RealWriter;
        protected Page CurrentPage;
        protected StringBuilder UserOutput
        {
            get
            {
                if (_UserOutput == null)
                {
                    _UserOutput = new StringBuilder();
                }
                return _UserOutput;
            }
        } protected StringBuilder _UserOutput = null;
        /// <summary>
        /// Creates a new CSQuery object from a Page.Render method. The base Render method of a page should be overridden,
        /// and this called from inside it to configure the CsQUery
        /// </summary>
        /// <param name="page">The current System.Web.UI.Page</param>
        /// <param name="renderMethod">The delegate to the base render method</param>
        /// <param name="writer">The HtmlTextWriter to output the final stream (the parameter passed to the Render method)</param>
        public void CreateFromRender(Page page, Action<HtmlTextWriter> renderMethod, HtmlTextWriter writer)
        {
            RealWriter = writer;
            CurrentPage = page;
            renderMethod(Writer);
            CreateFromWriter();
        }

        /// <summary>
        /// Write json data to a global variable
        /// </summary>
        /// <param name="data"></param>
        public void WriteJson(string target, object data)
        {
            UserOutput.Append(CsQueryHttpContext.JsonStringDef(target, data));


        }

        public bool IsAsync
        {
            get
            {
                return _AsyncPostbackData != null;
            }
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
            if (_AsyncPostbackData != null)
            {
                foreach (var data in _AsyncPostbackData)
                {
                    RealWriter.Write(data.Render());
                }
            }
            else
            {
                string content = Owner.Render();
                if (_UserOutput != null)
                {
                    content += "<script type=\"text/javascript\">" + System.Environment.NewLine + UserOutput.ToString() + "</script>";
                }
                RealWriter.Write(content);
            }
        }

        internal static string JsonStringDef(string target, object data)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return target + "=$.parseJSON('" +
                serializer.Serialize(data) +
                "');" + System.Environment.NewLine;
        }
        protected List<ManualResetEvent> AsyncEvents
        {
            get
            {
                return _AsyncEvents.Value;
            }
        }
        private Lazy<List<ManualResetEvent>> _AsyncEvents = new Lazy<List<ManualResetEvent>>();
    }
}
