using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CsQuery.Mvc.ClientScript;

namespace CsQuery.Mvc
{
    /// <summary>
    /// Custom implementation of RazorViewEngine to support CsQuery processing
    /// </summary>
    public class CsQueryViewEngine : RazorViewEngine
    {
        #region constructores

        /// <summary>
        /// Create a new CsQueryViewEngine
        /// </summary>
        
        public static RazorViewEngine Create()
        {
            return new CsQueryViewEngine();
        }

        /// <summary>
        /// Default constructor.
        /// </summary>

        public CsQueryViewEngine()
        {
            LibraryPath = new PathList();
            LibraryPath.Add("~/scripts/lib");
        }

        #endregion

        #region private properties

        #endregion
        
        #region public properties


        /// <summary>
        /// Options for the CsQueryViewEngine
        /// </summary>

        public ViewEngineOptions Options { get; set; }

        /// <summary>
        /// List of relative paths to search for included files.
        /// </summary>

        public PathList LibraryPath { get; protected set; }

    
        #endregion

        #region private methods

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
            return ConfigureView(new CsQueryView(
                controllerContext,
                partialPath,
                null,
                false,
                base.FileExtensions,
                base.ViewPageActivator,
                true
            ));
           
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
            return ConfigureView(new CsQueryView(
                controllerContext,
                viewPath,
                masterPath,
                true,
                base.FileExtensions,
                base.ViewPageActivator,
                false
            ));
        }

        private CsQueryView ConfigureView(CsQueryView view)  
        {
            view.Options = Options;
            view.LibraryPath = LibraryPath;
            return view;

        }

        #endregion


    }


}