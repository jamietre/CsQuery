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

        #region constructor

        /// <summary>
        /// Constructor for CsQueryHttpContext. Usually, you should use WebForms.CreateFromRender to
        /// create one of these.
        /// </summary>
        ///
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="page">
        /// The ASP.NET WebForms Page object bound to this context.
        /// </param>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="renderMethod">
        /// The render method.
        /// </param>

        public CsQueryHttpContext(HttpContext context, Page page, HtmlTextWriter writer, Action<HtmlTextWriter> renderMethod)
        {
            Context = context;
            RealWriter = writer;
            ControlRenderMethod = renderMethod;
            Page = page;

            Create();
        }

        #endregion

        #region private properties

        private HttpContext _Context;
        private HtmlTextWriter _Writer;
        private StringBuilder _sb;
        private StringWriter _sw;
        private List<AsyncPostbackData> _AsyncPostbackData;

        /// <summary>
        /// Gets or sets the current HttpContext.
        /// </summary>
        ///
        /// <value>
        /// The context.
        /// </value>

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
        }

        /// <summary>
        /// Gets the writer.
        /// </summary>
        ///
        /// <value>
        /// The interim writer
        /// </value>

        private HtmlTextWriter Writer
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
        }

        /// <summary>
        /// A delegate to the Render method of a WebForms Page object
        /// </summary>
        protected Action<HtmlTextWriter> ControlRenderMethod { get; set; }

        /// <summary>
        /// A reference to the HtmlTextWriter passed into the Render method
        /// </summary>
        protected HtmlTextWriter RealWriter { get; set; }

        /// <summary>
        /// The ASP.NET WebForms Page object bound to this context
        /// </summary>
        protected Page Page { get; set; }

        #endregion

        #region public properties

        /// <summary>
        /// The CQ object representing the output from the Render method.
        /// </summary>
        ///
        /// <value>
        /// The dom.
        /// </value>

        public CQ Dom { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether this is an asynchronous get (e.g., an UpdatePanel).
        /// </summary>
        ///
        /// <value>
        /// true if this object is asynchronous, false if not.
        /// </value>

        public bool IsAsync
        {
            get
            {
                return _AsyncPostbackData != null;
            }
        }

        /// <summary>
        /// Sequence of AsyncPostbackData objects representing the HTML and metadata for each UpdatePanel
        /// that is part of the response
        /// </summary>
        ///
        /// <value>
        /// Object encapsulating the UpdatePanel data.
        /// </value>

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

        #endregion

        #region public methods

        /// <summary>
        /// Renders the DOM to the bound TextWriter.
        /// </summary>

        public void Render()
        {
            Render(DomRenderingOptions.Default);  
        }

        /// <summary>
        /// Renders the DOM to the bound TextWriter with the passed options
        /// </summary>
        ///
        /// <param name="options">
        /// Options for controlling the operation.
        /// </param>

        public void Render(DomRenderingOptions options)
        {

            if (_AsyncPostbackData != null)
            {
                foreach (var data in _AsyncPostbackData)
                {

                    RealWriter.Write(data.Render(options));
                }
            }
            else
            {
                string content = Dom.Render(options);
                RealWriter.Write(content);
            }
        }
        #endregion

        #region private methods

        /// <summary>
        /// Create a context from the bound method information
        /// </summary>
        
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
                Dom = CQ.CreateDocument(_sb.ToString());
            }
        }

        #endregion

      
    }
}
