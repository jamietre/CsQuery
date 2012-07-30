using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Routing;
using System.IO;
using System.Reflection;
using System.Web.Hosting;
using Moq;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.Mvc.Tests
{
    public class MvcAppHost : MarshalByRefObject, IDisposable
    {
        /// <summary>
        /// Creates an application host environment for the project at the provided applicationPath. If
        /// binPath is provided, copies binaries to the applicationPath from there.
        /// </summary>
        ///
        /// <typeparam name="T">
        /// Generic type parameter of the HttpApplication type (e.g. globals.asax) for this application.
        /// </typeparam>
        /// <param name="applicationPath">
        /// Full path to the application.
        /// </param>
        /// <param name="binPath">
        /// (optional) The path to the binaries that should be executed, if not found in the "bin"
        /// subfolder of the application path.
        /// </param>
        ///
        /// <returns>
        /// A new application host.
        /// </returns>

        public static MvcAppHost CreateApplicationHost<T>(string applicationPath, string binPath = null) where T : HttpApplication, new()
        {

            binPath = binPath ?? AppDomain.CurrentDomain.BaseDirectory;

            string destPath = applicationPath + "\\bin";

            var bin = new DirectoryInfo(binPath);
            var binTarget = new DirectoryInfo(destPath);

            CsQuery.Utility.Support.CopyFiles(bin, binTarget, "*.dll", "*.pdb");

            var host = (MvcAppHost)ApplicationHost.CreateApplicationHost(
                typeof(MvcAppHost),
                "/",
                applicationPath);

            host.AppBinPath = binTarget;
            host.InitializeApplication<T>();

            return host;
        }
        public DirectoryInfo AppBinPath { get; protected set; }

        public void InitializeApplication<T>() where T: HttpApplication, new()
        {

            var global = new T();
            var globalType = typeof(T);
            MethodInfo mi = globalType.GetMethod("Application_Start", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            mi.Invoke(global, new object[] { });
        }
        public string RenderView<T>(string action)  where T: Controller, new()
        {
            // get method info

            var mi = GetCaseInsensitiveMethod(typeof(T), action);
            if (mi == null)
            {
                throw new ArgumentException(String.Format("Unable to find action {0} on controller {1}",
                    action,
                    mi.Name));
            }

            string controllerName = ("."+typeof(T).FullName)
                .BeforeLast("Controller").AfterLast(".");

            string path = controllerName + "/" + action;
            string url = "http://mvc.test.abc/" + path;

            var controller = new T();
            var writer = new StringWriter();

            var httpContext = new HttpContext(new HttpRequest("", url, ""), new HttpResponse(writer));
                
            if (HttpContext.Current != null) throw new NotSupportedException("httpcontext was already set");

            HttpContext.Current = httpContext;
            var routeData = new RouteData();
            routeData.Values.Add("controller", controllerName);
            routeData.Values.Add("action", mi.Name);
            var controllerContext = new ControllerContext(new HttpContextWrapper(httpContext), routeData, controller);
            controller.ControllerContext = controllerContext;


            ActionResult res = (ActionResult)mi.Invoke(controller, new object[] { });
            
            res.ExecuteResult(controllerContext);
            HttpContext.Current = null;
            return writer.ToString();
            
        }

        /// <summary>
        /// Return MethodInfo for a named method with no parameters and a case-insensitive match for name.
        /// </summary>
        ///
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="method">
        /// The method name.
        /// </param>
        ///
        /// <returns>
        /// The MethodInfo.
        /// </returns>

        private MethodInfo GetCaseInsensitiveMethod(Type type, string method)
        {
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (var mi in methods)
            {
                if (mi.Name.Equals(method, StringComparison.CurrentCultureIgnoreCase) &&
                    mi.GetParameters().Length==0)
                {
                    return mi;
                }
            }
            return null;
        }

        /// <summary>
        /// Cleans up temporary files created in the host environment
        /// </summary>

        public void Dispose()
        {
            CsQuery.Utility.Support.DeleteFiles(AppBinPath, "*.pdb", "*.dll");
        }
    }
}
