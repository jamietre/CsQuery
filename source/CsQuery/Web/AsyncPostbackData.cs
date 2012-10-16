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

        private StringBuilder _UserOutput = null;
        private StringBuilder UserOutput
        {
            get
            {
                if (_UserOutput == null)
                {
                    _UserOutput = new StringBuilder();
                }
                return _UserOutput;
            }
        } 

        //public void WriteJson(string target, object data)
        //{
        //    UserOutput.Append(CsQueryHttpContext.JsonStringDef(target, data));

        //}

        /// <summary>
        /// Populate the content from a ASP.NET updatepanel data block
        /// </summary>
        /// <param name="length"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="content"></param>
        public void Create(int length, string type, string id, string content)
        {
            Content = content;
            ID = id;
            Length = length;
            DataType = type;
        }

        /// <summary>
        /// Output this data block, recalulating the lengtht parameter based on the new output.
        /// </summary>
        ///
        /// <returns>
        /// HTML string.
        /// </returns>

        public string Render()
        {
            return Render(DomRenderingOptions.Default);

        }

        /// <summary>
        /// Output this data block, recalulating the lengtht parameter based on the new output, using the passed options.
        /// </summary>
        ///
        /// <param name="options">
        /// Options for controlling the operation.
        /// </param>
        ///
        /// <returns>
        /// HTML string.
        /// </returns>

        public string Render(DomRenderingOptions options)
        {

            string content = _Dom != null ? Dom.Render(options) : Content;
            if (_UserOutput != null)
            {
                content += "<script type=\"text/javascript\">" + System.Environment.NewLine + UserOutput.ToString() + "</script>";
            }

            return content.Length.ToString() + "|" + DataType + "|" + ID + "|" + content + "|";
        }
        /// <summary>
        /// The content of the data packet (HTML). Probably, you'd rather be looking at the Dom property.
        /// </summary>

        public string Content;

        /// <summary>
        /// Gets the DOM created from the HTML of this UpdatePanel data packet
        /// </summary>

        public CQ Dom
        {
            get
            {
                if (_Dom == null)
                {
                    _Dom = CQ.CreateFragment(Content);
                }
                return _Dom;
            }
        }
        private CQ _Dom = null;

        /// <summary>
        /// Gets or sets the UpdatePanel identifier.
        /// </summary>

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

        private  string _ID;

        /// <summary>
        /// The length, in bytes, of the data component of this UpdatePanel data packet
        /// </summary>

        protected int Length;

        /// <summary>
        /// Gets or sets the type of the data. This is a Microsoft entity.
        /// </summary>

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
        private string _DataType;

    }
}
