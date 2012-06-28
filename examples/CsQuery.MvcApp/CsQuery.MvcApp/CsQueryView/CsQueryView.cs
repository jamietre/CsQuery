using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using CsQuery.MvcApp.Controllers;

namespace CsQuery.Mvc
{
    /// <summary>
    /// A custom view that provides access to a CQ object of the html before it is output
    /// </summary>
    public class CsQueryView : RazorView
    {
        #region constructors

        public CsQueryView(ControllerContext controllerContext, string viewPath, string layoutPath, bool runViewStartPages, IEnumerable<string> viewStartFileExtensions, bool isPartial)
            : base(controllerContext, viewPath, layoutPath, runViewStartPages, viewStartFileExtensions)
        {
            IsPartial = isPartial;
        }

        public CsQueryView(ControllerContext controllerContext, string viewPath, string layoutPath, bool runViewStartPages, IEnumerable<string> viewStartFileExtensions, IViewPageActivator viewPageActivator, bool isPartial)
            : base(controllerContext, viewPath, layoutPath, runViewStartPages, viewStartFileExtensions, viewPageActivator)
        {
            IsPartial = isPartial;
        }

        #endregion

        #region private properties

        private bool IsPartial;

        #endregion

        /// <summary>
        /// Intercept rendering and create a CsQuery object
        /// </summary>
        /// <param name="viewContext"></param>
        /// <param name="writer"></param>
        /// <param name="instance"></param>
        protected override void RenderView(ViewContext viewContext, TextWriter writer, object instance)
        {

            if (viewContext.Controller is CsQueryController)
            {
                CsQueryController controller = (CsQueryController)viewContext.Controller;
                Action<CQ> handler = IsPartial ?
                    controller.GetCqHandler(instance.ToString()) :
                    controller.GetCqHandler();

                if (handler!=null)
                {
                    var sb = new StringBuilder();
                    var sw = new StringWriter(sb);

                    // render the view into out local writer
                    base.RenderView(viewContext, sw, instance);
                    var cqDoc = CQ.Create(sb.ToString());

                    handler(cqDoc);

                    writer.Write(cqDoc.Render());
                    return;
                }
            }
            base.RenderView(viewContext, writer, instance);
        }
    }
}