using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.IO;

namespace CsQuery.Mvc
{
    /// <summary>
    /// Encapsulate the deffered creation of the CQ object during processing a view, so it's never created if we don't need it.
    /// </summary>

    internal class DeferredCq
    {
        public DeferredCq(Action<ViewContext, TextWriter, object> baseRender, ViewContext viewContext, object instance)
        {
            ViewContext = viewContext;
            CaptureInstanceHtml(baseRender,instance);
        }
        
        private ViewContext ViewContext;
        private CQ _Dom;

        /// <summary>
        /// Source HTML of the view.
        /// </summary>

        public StringBuilder SourceHtml;

        /// <summary>
        /// Gets the dom, either already built or on demand.
        /// </summary>

        public CQ Dom
        {
            get
            {
                if (_Dom == null)
                {
                    _Dom = GetCqDoc();
                }
                return _Dom;
            }
        }

        /// <summary>
        /// If the instance is going to go out of scope, use this to ensure the deferred object remains
        /// valid.
        /// </summary>


        private void CaptureInstanceHtml(Action<ViewContext, TextWriter, object> baseRender, object instance)
        {
            if (SourceHtml == null)
            {
                SourceHtml = new StringBuilder();
                var sw = new StringWriter(SourceHtml);

                // render the view into out local writer
                baseRender(ViewContext, sw, instance);
            }
        }

        /// <summary>
        /// When true, the Dom object has been accessed and therefore constructed.
        /// </summary>

        public bool IsDomCreated
        {
            get
            {
                return _Dom != null;
            }
        }
        private CQ GetCqDoc()
        {
            return CQ.Create(SourceHtml.ToString());
        }

        

    }
}
