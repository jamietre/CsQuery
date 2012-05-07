using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Web
{
    /// <summary>
    /// Represents async data from an ASP.NET webforms UpdatePanel
    /// </summary>
    public class AsyncPostbackData
    {
        /// <summary>
        /// Write JSON data to a global variable
        /// </summary>
        /// <param name="target"></param>
        /// <param name="data"></param>

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
        public void WriteJson(string target, object data)
        {
            UserOutput.Append(CsQueryHttpContext.JsonStringDef(target, data));

        }
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
            if (_UserOutput != null)
            {
                content += "<script type=\"text/javascript\">" + System.Environment.NewLine + UserOutput.ToString() + "</script>";
            }

            return content.Length.ToString() + "|" + DataType + "|" + ID + "|" + content + "|";

        }
        public string Content;
        public CQ Dom
        {
            get
            {
                if (_Dom == null)
                {
                    _Dom = CQ.Create(Content);
                }
                return _Dom;
            }
        }
        protected CQ _Dom = null;
        public string ID
        {
            get
            {
                return _ID;
            }
            protected set
            {
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
}
