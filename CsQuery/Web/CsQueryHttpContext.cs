using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Threading;
using System.Web.Script.Serialization;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.Web
{
    /// <summary>
    /// Server extension to CsQuery -- adds functionality for dealing with postbacks, and getting data from 
    /// external sources
    /// </summary>
    public class CsQueryHttpContext
    {
        public CsQueryHttpContext(HttpContext context)
        {
            Context = context;


        }
        public Action<HtmlTextWriter> ControlRenderMethod { get; set; }
        public bool AspNet { get; set; }
        public CQ Dom { get; protected set; }
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



       
        protected HtmlTextWriter Writer
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


        public HtmlTextWriter RealWriter { get; set; }
        public Page Page { get; set; }

        public void Create()
        {
            ControlRenderMethod(Writer);
 
            ScriptManager mgr = ScriptManager.GetCurrent(Page);

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
                            throw new InvalidOperationException("Unable to parse UpdatePanel data");
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
                Dom = CQ.Create(_sb.ToString());
            }
        }

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
                string content = Dom.Render();
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

    }
}
