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
using System.Web.Optimization;
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

        public static void SetupApplicationHost(string applicationPath, string binPath) 
        {

            binPath = binPath ?? AppDomain.CurrentDomain.BaseDirectory;

            string destPath = applicationPath + "\\bin";

            var bin = new DirectoryInfo(binPath);
            BinTarget = new DirectoryInfo(destPath);
            ApplicationPath = applicationPath;

            CsQuery.Utility.Support.CopyFiles(bin, BinTarget, "*.dll", "*.pdb");

            
        }

        public static MvcAppHost CreateApplicationHost<T>() where T : HttpApplication, new()
        {
            
            var host = (MvcAppHost)ApplicationHost.CreateApplicationHost(
                typeof(MvcAppHost),
                "/",
                ApplicationPath);

            host.TempBinPath = BinTarget;
            host.InitializeApplication<T>();

            return host;
        }

        public static void CleanupApplicationHost() {
            CsQuery.Utility.Support.DeleteFiles(BinTarget, "*.pdb", "*.dll");
        }

        private static DirectoryInfo BinTarget;

        private static string ApplicationPath;
        /// <summary>
        /// Gets or sets the active HttpContext.
        /// </summary>

        private HttpContext Context { get; set; }

        public ViewEngineOptions ViewEngineOptions
        {
            get
            {
                return CsQuery.Mvc.Tests.MvcTestApp.ViewEngine.Options;
            }
            set
            {
                CsQuery.Mvc.Tests.MvcTestApp.ViewEngine.Options = value;
            }
        }

        /// <summary>
        /// Gets or sets the full pathname of the temporary application binaries (the same as the target
        /// app path). These will be copied from the binary folder, and deleted when the host is
        /// disposed.
        /// </summary>

        public DirectoryInfo TempBinPath { get; protected set; }

        /// <summary>
        /// Initializes the application host by invoking the global Application_Start method.
        /// </summary>
        ///
        /// <typeparam name="T">
        /// Generic type parameter.
        /// </typeparam>

        public void InitializeApplication<T>() where T: HttpApplication, new()
        {

            var global = new T();
            var globalType = typeof(T);
            MethodInfo mi = globalType.GetMethod("Application_Start", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            mi.Invoke(global, new object[] { });
        }

        /// <summary>
        /// Renders an MVC view defined by a controller and named action.
        /// </summary>
        ///
        /// <remarks>
        /// The basis for this was taken from user http://stackoverflow.com/users/66372/eglasius 's
        /// answer answer for this question:  
        /// http://stackoverflow.com/questions/3702526/is-there-a-way-to-process-an-mvc-view-aspx-file-
        /// from-a-non-web-application.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when the named action could not be found on the controller.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when there is already an application HTTP context.
        /// </exception>
        ///
        /// <typeparam name="T">
        /// Generic type of the controller.
        /// </typeparam>
        /// <param name="action">
        /// The action name.
        /// </param>
        /// <param name="destroyContext">
        /// true to destroy context.
        /// </param>
        ///
        /// <returns>
        /// The string of HTML
        /// </returns>
        
        public string RenderView<T>(string action, bool destroyContext=true)  where T: Controller, new()
        {
            // get method info

            var mi = GetCaseInsensitiveMethod(typeof(T), action);
            if (mi == null)
            {
                throw new ArgumentException(String.Format("Unable to find action \"{0}\" on controller \"{1}\"",
                    action,
                    typeof(T).FullName));
            }

            string controllerName = ("."+typeof(T).FullName)
                .BeforeLast("Controller").AfterLast(".");

            string path = controllerName + "/" + action;
            //string url = "http://mvc.test.abc/" + path;

            var controller = new T();
            //var writer = new StringWriter();

            //var httpContext = new HttpContext(new HttpRequest("", url, ""), new HttpResponse(writer));
            

            if (HttpContext.Current != null)
            {
                throw new InvalidOperationException("HttpContext was already set");
            }

            HttpResponse response;
            StringWriter writer;
            Context = GetHttpContext(out writer, out response);

            HttpContext.Current = Context;
            var routeData = new RouteData();
            routeData.Values.Add("controller", controllerName);
            routeData.Values.Add("action", mi.Name);

            var controllerContext = new ControllerContext(new HttpContextWrapper(Context), routeData, controller);
            controller.ControllerContext = controllerContext;

            ActionResult res = (ActionResult)mi.Invoke(controller, new object[] { });
            
            res.ExecuteResult(controllerContext);

            if (destroyContext)
            {
                ClearContext();
            }

            return writer.ToString();
        }

        /// <summary>
        /// Enumerates bundle files assicuated with a given bundleUrl for the active context.
        /// </summary>
        ///
        /// <param name="url">
        /// URL of the document.
        /// </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process bundle files for URL in this
        /// collection.
        /// </returns>

        public IEnumerable<string> BundleFilesForUrl(string url)
        {
            var bundle = BundleForUrl(url);

            var bundleFiles = bundle.EnumerateFiles(BundleContextForUrl(url));
            return bundleFiles.Select(item => item.FullName).ToList();
        }

        /// <summary>
        /// Get the actual contents of the bundle.
        /// </summary>
        ///
        /// <param name="url">
        /// URL of the document.
        /// </param>
        ///
        /// <returns>
        /// A string of javascript.
        /// </returns>

        public string BundlesContentsForUrl(string url)
        {
            var bundle = BundleForUrl(url);
            var bundleContext = BundleContextForUrl(url);
            
            var content = bundle.GenerateBundleResponse(bundleContext).Content;
            //var response = bundle.ApplyTransforms(bundleContext, content, bundle.EnumerateFiles(bundleContext));
            //return response.Content;
             return content;
        }

        /// <summary>
        /// Gets the library path from the ViewEngine
        /// </summary>

        public IList<string> LibraryPath
        {
            get
            {
                return CsQuery.Mvc.Tests.MvcTestApp.ViewEngine.LibraryPath;
            }
        }


        
        /// <summary>
        /// Clears the current HttpContext. This should be done when using methods that do not create
        /// their own context.
        /// </summary>

        public void ClearContext()
        {
            HttpContext.Current = null;
            Context = null;
        }
        /// <summary>
        /// Cleans up temporary files created in the host environment
        /// </summary>

        public void Dispose()
        {
            ClearContext();
        }

        #region private methods

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
                    mi.GetParameters().Length == 0)
                {
                    return mi;
                }
            }
            return null;
        }

        private HttpContext GetHttpContext(out StringWriter writer, out HttpResponse response)
        {
            string url = "http://mvc.test.abc/";
            writer = new StringWriter();
            response = new HttpResponse(writer);
            var context = new HttpContext(new HttpRequest("", url, ""), response);
            return context;

        }

        private BundleContext BundleContextForUrl(string url)
        {
            return new BundleContext(new HttpContextWrapper(Context), BundleTable.Bundles, url);
        }
        private string PathOnlyFromUrl(string url)
        {
            string path = (url.StartsWith("~") ? "" : "~") + url.Before("?");
            return path;
        }
        private Bundle BundleForUrl(string url)
        {
            string path = PathOnlyFromUrl(url);
            var bundles = BundleTable.Bundles;
            var bundle = bundles.GetRegisteredBundles().Where(item => item.Path == path).Single();
            return bundle;
        }
        #endregion

    }
}
