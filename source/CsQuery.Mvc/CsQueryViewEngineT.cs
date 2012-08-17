using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CsQuery.Utility;

namespace CsQuery.Mvc
{
    /// <summary>
    /// Custom implementation of RazorViewEngine to support CsQuery processing.
    /// </summary>
    ///
    /// <typeparam name="T">
    /// A type defining a CsQuery layout controller, which can contain Cq_Start() and Cq_End()
    /// methods that will be called for all pages.
    /// </typeparam>

    public class CsQueryViewEngine<T> : RazorViewEngine 
        where T: class, ICsQueryController
    {
        /// <summary>
        /// Creates a view by using the specified controller context and the paths of the view and master
        /// view, and invokes methods on a layout controller identified by the generic type of the class.
        /// </summary>
        ///
        /// <param name="controllerContext">
        /// The controller context.
        /// </param>
        /// <param name="viewPath">
        /// The path to the view.
        /// </param>
        /// <param name="masterPath">
        /// The path to the master view.
        /// </param>
        ///
        /// <returns>
        /// The view.
        /// </returns>

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            CsQueryView view = new CsQueryView(
                controllerContext,
                viewPath,
                masterPath,
                true,
                base.FileExtensions,
                base.ViewPageActivator,
                false
            );
            view.LayoutController = Objects.CreateInstance<T>();
            return view;
        }

        /// <summary>
        /// Creates a partial view using the specified controller context and partial path.
        /// </summary>
        ///
        /// <param name="controllerContext">
        /// The controller context.
        /// </param>
        /// <param name="partialPath">
        /// The path to the partial view.
        /// </param>
        ///
        /// <returns>
        /// The partial view.
        /// </returns>

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
           return new CsQueryView(
                controllerContext,
                partialPath,
                null,
                false,
                base.FileExtensions,
                base.ViewPageActivator,
                true
            );
        }
      
    }


}