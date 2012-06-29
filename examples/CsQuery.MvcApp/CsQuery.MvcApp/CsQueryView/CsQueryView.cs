using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Reflection;
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

        /// <summary>
        /// A lookup of ClassName, indicating whether to bother with CQ for this controller
        /// </summary>
        private static IDictionary<string, bool> HasCqMethodsRef = new Dictionary<string, bool>();
        
        /// <summary>
        /// A map of ClassName+ActonName to a method
        /// </summary>
        private static IDictionary<string, MethodInfo> CqMethods;

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
                
                if (HasCqMethods(viewContext) && !IsPartial)
                {
                    var sb = new StringBuilder();
                    var sw = new StringWriter(sb);

                    // render the view into out local writer
                    base.RenderView(viewContext, sw, instance);
                    var cqDoc = CQ.Create(sb.ToString());


                    string currentAction = GetCurrentAction(viewContext);
                    string controllerName = GetControllerName(viewContext);

                    if (IsPartial)
                    {
                        // viewName is the name of the cshtml file with underscores e.g "ASP.namespace_subspace_shared__layout_cshtml"
                        // we've mapped things that could be partials with the "_cshtml" attached to the key in the dictionary

                        string viewName = instance.ToString();
                        
                        string prefix = controllerName + "_" + currentAction;

                        string key = CqMethods.Keys
                            .Where(item => viewName.EndsWith(item)).FirstOrDefault();

                        if (key != null)
                        {
                            var del = CqMethods[prefix +"_"+ key];
                            del.Invoke(controller, new object[] { cqDoc  });
                        } 
                    }
                    else
                    {
                        controller.Doc = cqDoc;
                        MethodInfo method;

                        if (CqMethods.TryGetValue(controllerName + "_Cq_Start", out method))
                        {
                            method.Invoke(controller, null);
                        }


                        if (CqMethods.TryGetValue(controllerName + "_Cq_" + currentAction, out method))
                        {
                            method.Invoke(controller, null);
                        }

                        if (CqMethods.TryGetValue(controllerName+"_Cq_End", out method))
                        {
                            method.Invoke(controller, null);
                        }


                    }

                   
                    writer.Write(cqDoc.Render());
                    return;
                }
            }
            base.RenderView(viewContext, writer, instance);
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
        private string GetControllerName(ViewContext viewContext)
        {
            return viewContext.Controller.ControllerContext.RouteData.Values["controller"].ToString();
        }
        private void EnsureMethodsMapped(Type controllerType, string controllerName)
        {
            bool hasMethods = false;

            foreach (var mi in controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (HasCqSignature(mi))
                {

                    string name = mi.Name;
                    if (name.IndexOf("_",3) > 0)
                    {
                        if (!name.EndsWith("_cshtml"))
                        {
                            name += "_cshtml";
                        }
                    }
                    if (hasMethods==false) {
                        hasMethods = true;
                        if (CqMethods == null)
                        {
                            CqMethods = new Dictionary<string, MethodInfo>();
                        }
                    }
                    CqMethods.Add(controllerName+"_"+name, mi);
                }
            }
            HasCqMethodsRef[controllerName] = hasMethods;
        }

        private bool HasCqSignature(MethodInfo mi)
        {
            var pi = mi.GetParameters();
            //return pi.Length == 1 && pi[0].ParameterType == typeof(CQ);
            //if (mi.Name.StartsWith("Cq_"))
            //{
            //    Console.Write("");
            //}
            return mi.Name.StartsWith("Cq_") && 
                mi.ReturnType == typeof(void) &&
                (
                    pi.Length == 0  && mi.Name.IndexOf("_",3)<0 ||
                    pi.Length == 1 && pi[0].ParameterType == typeof(CQ)
                );
        }

        private string GetCurrentAction(ViewContext context)
        {
            return context.Controller.ControllerContext.RouteData.Values
                .Where(item => item.Key == "action")
                .First().Value.ToString();

        }
    }
}