using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CsQuery.Mvc
{
    /// <summary>
    /// Custom implementation of RazorViewEngine to support CsQuery processing
    /// </summary>
    public class CsQueryViewEngine : RazorViewEngine
    {
        /// <summary>
        /// Default constructor.
        /// </summary>

        public CsQueryViewEngine()
        {

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

        /// <summary>
        /// Creates a view by using the specified controller context and the paths of the view and master
        /// view.
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
            return new CsQueryView(
                controllerContext,
                viewPath,
                masterPath,
                true,
                base.FileExtensions,
                base.ViewPageActivator,
                false
            );
        }
    }


}