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
        /// Create a new CsQueryViewEngine
        /// </summary>
        
        public static RazorViewEngine Create()
        {
            return new CsQueryViewEngine();
        }

        /// <summary>
        /// Create a new CsQueryViewEngine using common controller of type T
        /// </summary>

        public static RazorViewEngine Create<T>() where T : class, ICsQueryController
        {
            return new CsQueryViewEngine<T>();
        }

        /// <summary>
        /// Default constructor.
        /// </summary>

        public CsQueryViewEngine()
        {

        }

        /// <summary>
        /// When true, activates script manager functionality
        /// </summary>
        
        public bool EnableScriptManager { get; set; }

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
                true,
                EnableScriptManager
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
                false,
                EnableScriptManager
            );
        }
    }


}