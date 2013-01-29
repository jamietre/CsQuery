using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Reflection;
using CsQuery.Mvc.ClientScript;

namespace CsQuery.Mvc
{
    /// <summary>
    /// A custom view that provides access to a CQ object of the html before it is output. When used
    /// as the view engine for a controller marked with ICsQueryController, it will populate the one
    /// property of that interface "Dom" with a CQ object representing the final HTML for this view,
    /// and then call for the following methods:
    /// 
    /// 
    /// void Cq_Start()     // called for any action first
    /// 
    /// void Cq_Action()    // called for a specfic action
    /// 
    /// void Cq_End()       // called for any action last
    /// 
    /// 
    /// void Cq_PartialViewName()          // called whenever the partial view
    ///                                    // that matches either the  file name
    /// 
    /// void Cq_Action_PartialViewName()   // caled whenever the matching partial view is
    ///                                    // rendered just for this action.
    /// </summary>
    ///
    /// <remarks>   James Treworgy, 7/1/2012. </remarks>

    public class CsQueryView : RazorView
    {
        #region constructors

        /// <summary>
        /// Constructor for CsQueryView
        /// </summary>
        ///
        /// <param name="controllerContext">
        /// Context for the controller.
        /// </param>
        /// <param name="viewPath">
        /// Full pathname of the view file.
        /// </param>
        /// <param name="layoutPath">
        /// Full pathname of the layout file.
        /// </param>
        /// <param name="runViewStartPages">
        /// true to run view start pages.
        /// </param>
        /// <param name="viewStartFileExtensions">
        /// The view start file extensions.
        /// </param>
        /// <param name="isPartial">
        /// true if this object is partial.
        /// </param>

        public CsQueryView(ControllerContext controllerContext, string viewPath, string layoutPath, bool runViewStartPages, IEnumerable<string> viewStartFileExtensions, 
            bool isPartial
            )
            : base(controllerContext, viewPath, layoutPath, runViewStartPages, viewStartFileExtensions)
        {
            IsPartial = isPartial;
        }

        /// <summary>
        /// Constructor for a CsQueryView
        /// </summary>
        ///
        /// <param name="controllerContext">
        /// Context for the controller.
        /// </param>
        /// <param name="viewPath">
        /// Full pathname of the view file.
        /// </param>
        /// <param name="layoutPath">
        /// Full pathname of the layout file.
        /// </param>
        /// <param name="runViewStartPages">
        /// true to run view start pages.
        /// </param>
        /// <param name="viewStartFileExtensions">
        /// The view start file extensions.
        /// </param>
        /// <param name="viewPageActivator">
        /// The view page activator.
        /// </param>
        /// <param name="isPartial">
        /// true if this object is partial.
        /// </param>

        public CsQueryView(ControllerContext controllerContext, string viewPath, string layoutPath, bool runViewStartPages, IEnumerable<string> viewStartFileExtensions, IViewPageActivator viewPageActivator, 
            bool isPartial
            )
            : base(controllerContext, viewPath, layoutPath, runViewStartPages, viewStartFileExtensions, viewPageActivator)
        {
            IsPartial = isPartial;
        }

        #endregion

        #region private properties

        private bool IsPartial;

        

        /// <summary>
        /// A lookup of ClassName, indicating whether to bother with CQ for this controller
        /// </summary>
        private static IDictionary<string, bool> HasCqMethodsRef = new Dictionary<string, bool>(StringComparer.CurrentCultureIgnoreCase);
        
        /// <summary>
        /// A map of ClassName+ActonName to a method
        /// </summary>
        private static IDictionary<string, MethodInfo> CqMethods = new Dictionary<string, MethodInfo>(StringComparer.CurrentCultureIgnoreCase);

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the strings in the library file search path. 
        /// </summary>
        
        public PathList LibraryPath { get; set; }
        
        /// <summary>
        /// Options for controlling the operation of the view
        /// </summary>

        public ViewEngineOptions Options { get; set; }

       
        #endregion

        /// <summary>
        /// Intercept rendering and create a CsQuery object
        /// </summary>
        /// <param name="viewContext"></param>
        /// <param name="writer"></param>
        /// <param name="instance"></param>
        protected override void RenderView(ViewContext viewContext, TextWriter writer, object instance)
        {

            if (viewContext.Controller is ICsQueryController)
            {
                CsQueryController controller = (CsQueryController)viewContext.Controller;

                bool hasMethods = HasCqMethods(viewContext);
                bool scriptManagerActive = !IsPartial && Options.HasFlag(ViewEngineOptions.EnableScriptManager);

                if (hasMethods || scriptManagerActive)
                {
                    var deferredCq = new DeferredCq(base.RenderView, viewContext, instance);
                    
                    if (hasMethods)
                    {

                        string currentAction = GetCurrentAction(viewContext);
                        string controllerName = GetControllerName(viewContext);
                        
                        if (IsPartial)
                        {
                            // viewName is the name of the cshtml file with underscores e.g "ASP.namespace_subspace_shared__layout_cshtml"
                            // we've mapped things that could be partials with the "_cshtml" attached to the key in the dictionary

                            var viewInfo = ParseInstance(instance);
                            string prefix = controllerName + "_Cq_";
                            string actionPrefix = prefix + currentAction + "_";

                            MethodInfo mi;

                            if (CqMethods.TryGetValue(prefix + viewInfo.FileName, out mi) ||
                                CqMethods.TryGetValue(prefix + viewInfo.FilePath, out mi))
                            {

                                mi.Invoke(controller, new object[] { deferredCq.Dom });
                            }

                            // look for an action-specific method
                            if (CqMethods.TryGetValue(actionPrefix + viewInfo.FileName, out mi) ||
                                CqMethods.TryGetValue(actionPrefix + viewInfo.FilePath, out mi))
                            {
                                mi.Invoke(controller, new object[] { deferredCq.Dom });
                            }
                        }
                        else
                        {

                            controller.Deferred = deferredCq;
                            MethodInfo method;

                            if (CqMethods.TryGetValue(controllerName + "_Cq_Start", out method))
                            {
                                method.Invoke(controller, null);
                            }


                            if (CqMethods.TryGetValue(controllerName + "_Cq_" + currentAction, out method))
                            {
                                method.Invoke(controller, null);
                            }

                            if (CqMethods.TryGetValue(controllerName + "_Cq_End", out method))
                            {
                                method.Invoke(controller, null);
                            }


                        }
                    }
                    if (scriptManagerActive)
                    {
                        
                        ManageScripts(deferredCq.Dom, (System.Web.Mvc.WebViewPage)instance,viewContext);
                    }

                    
                    if (deferredCq.IsDomCreated)
                    {
                        writer.Write(deferredCq.Dom.Render());
                        return;
                    }
                }
            }
            base.RenderView(viewContext, writer, instance);
        }

        /// <summary>
        /// Manage scripts.
        /// </summary>
        ///
        /// <param name="cqDoc">
        /// The cq document.
        /// </param>
        /// <param name="viewPage">
        /// The view page.
        /// </param>
        /// <param name="viewContext">
        /// The active view context
        /// </param>


        private void ManageScripts(CQ cqDoc, WebViewPage viewPage, ViewContext viewContext) {
            ScriptManager mgr = new ScriptManager(new ScriptEnvironment
            {
                  LibraryPath = LibraryPath,
                  RelativePathRoot =viewContext.RequestContext.HttpContext.Request.AppRelativeCurrentExecutionFilePath,
                  MapPath = viewContext.RequestContext.HttpContext.Server.MapPath
            });

            mgr.Options = Options;

            mgr.ResolveScriptDependencies(cqDoc);
            
        }

        /// <summary>
        /// Return a structure contiaining the filename only part of a partial view, and the full
        /// unique name of the view including its relative path concatenated with underscores
        /// to match a method name.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        private PartialViewInfo ParseInstance(object instance)
        {
            string[] viewPath = ((System.Web.Mvc.WebViewPage)instance).VirtualPath
                .Split('/')
                .Skip(1)
                .ToArray();

            string partialFileName = viewPath.Last();
            if (partialFileName.IndexOf(".") >= 0)
            {
                partialFileName = partialFileName.Split('.')[0];
                viewPath[viewPath.Length - 1] = partialFileName;
            }


            return new PartialViewInfo
            {
                FileName = partialFileName,
                FilePath = String.Join("_", viewPath)
            };

        }

        private bool HasCqMethods(ViewContext viewContext)
        {
            bool hasMethods;
            string controllerName = GetControllerName(viewContext);
            if (HasCqMethodsRef.TryGetValue(controllerName, out hasMethods))
            {
                return hasMethods;
            }
            else
            {
                EnsureMethodsMapped(viewContext.Controller.GetType(),controllerName);
                return HasCqMethods(viewContext);
            }
        }

        /// <summary>
        /// Determine if a type contains CQ controller methods
        /// </summary>
        ///
        /// <param name="controllerType">
        /// Type of the controller.
        /// </param>
        /// <param name="controllerName">
        /// Name of the controller.
        /// </param>
        ///
        /// <returns>
        /// true if it has cq methods, false if not.
        /// </returns>

        private bool HasCqMethods(Type controllerType, string controllerName)
        {
            bool hasMethods;
            if (HasCqMethodsRef.TryGetValue(controllerName, out hasMethods))
            {
                return hasMethods;
            }
            else
            {
                EnsureMethodsMapped(controllerType, controllerName);
                return HasCqMethods(controllerType, controllerName);
            }
        }

        private string GetControllerName(ViewContext viewContext)
        {
            return viewContext.Controller.ControllerContext.RouteData.Values["controller"].ToString();
        }
        private void EnsureMethodsMapped(Type controllerType, string controllerName)
        {
            bool hasMethods = false;

            foreach (var mi in controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (Helpers.HasCqSignature(mi))
                {

                    string name = mi.Name;

                    if (hasMethods == false)
                    {
                        hasMethods = true;
                    }
                    CqMethods.Add(controllerName+"_"+name, mi);
                }
            }
            HasCqMethodsRef[controllerName] = hasMethods;
        }

      
        private string GetCurrentAction(ViewContext context)
        {
            return context.Controller.ControllerContext.RouteData.Values
                .Where(item => item.Key == "action")
                .First().Value.ToString();

        }

        private class PartialViewInfo
        {
            public string FileName;
            public string FilePath;
        }

     
    }
}