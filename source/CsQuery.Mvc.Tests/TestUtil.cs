using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Routing;
using System.IO;
using Moq;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.Mvc.Tests
{
    public class TestUtil
    {
        public static CQ RenderViewCQ<T>(string action) where T : Controller, new()
        {
            return CQ.CreateFragment(TestConfig.Host.RenderView<T>(action));
        }

        
        //public static T ContextBoundController<T>(string controller, string action)  where T: Controller, new()
        //{
        //    _RouteData = RouteData(controller, action);
        //    var ctx = HttpContext(controller+"/"+action);
        //    var ctlr = new T();

            
        //    var controllerContext = new ControllerContext(ctx, _RouteData, ctlr);
        //    ctlr.ControllerContext = controllerContext;

        //    return ctlr;
        //}

        //private static RouteData _RouteData;
        //private static RouteData RouteData(string controller,string action) {
        //    var rd = new RouteData();
        //    rd.Values.Add("action", action);
        //    rd.Values.Add("controller", controller);
        //    rd.RouteHandler = new System.Web.Mvc.MvcRouteHandler();

        //    return rd;
        //}
        //public static HttpContextBase HttpContext(string path)
        //{
        //    var context = new Mock<HttpContextBase>();
        //    var request = new Mock<HttpRequestBase>();
        //    var response = new Mock<HttpResponseBase>();

        //    //StringWriter writer = new StringWriter();
           
        //    //var request = new HttpRequest("", "http://mvc.test.com/" + path, "");
        //    //var response = new HttpResponse(writer);
            
        //    var session = new Mock<HttpSessionStateBase>();
        //    var server = new Mock<HttpServerUtilityBase>();
        //    var user = new Mock<IPrincipal>();
        //    var identity = new Mock<IIdentity>();

        //    request.Setup(req => req.ApplicationPath).Returns("/");
        //    request.Setup(req => req.CurrentExecutionFilePath).Returns("/");
        //    request.Setup(req => req.FilePath).Returns("/");

        //    request.Setup(req => req.AppRelativeCurrentExecutionFilePath).Returns("~/");
            
        //    request.Setup(req => req.HttpMethod).Returns("GET");
        //    request.Setup(req => req.RequestType).Returns("GET");

        //    string physPath = CsQuery.Utility.Support.FindPathTo("CsQuery.Mvc.Tests");
        //    request.Setup(req => req.PhysicalApplicationPath).Returns(physPath);
        //    request.Setup(req => req.PhysicalPath).Returns(physPath.BeforeLast("\\"));
        //    request.Setup(req => req.ApplicationPath).Returns("/");
        //    request.Setup(req => req.MapPath(It.IsAny<string>())).Returns((string p) =>
        //    {
        //        return physPath + p.Replace("~/", "");
        //    });

        //    request.Setup(req => req.AppRelativeCurrentExecutionFilePath).Returns("~/");

        //    string url = "http://www.test.com/" + path;
        //    request.Setup(req => req.Url).Returns(new Uri(url));
        //    request.Setup(req => req.RawUrl).Returns(url);
        //    request.Setup(req => req.Path).Returns(Path(url));
        //    request.Setup(req => req.PathInfo).Returns(PathInfo(url));
        //    request.Setup(req => req.RequestContext).Returns(new RequestContext(context.Object, _RouteData));

        //    response.Setup(res => res.ApplyAppPathModifier(It.IsAny<string>()))
        //        .Returns((string virtualPath) => virtualPath);
        //    user.Setup(usr => usr.Identity).Returns(identity.Object);
        //    identity.Setup(ident => ident.IsAuthenticated).Returns(true);

        //    //var context = new HttpContext(request, response);
            
        //    context.Setup(ctx => ctx.Request).Returns(request.Object);
        //    context.Setup(ctx => ctx.Response).Returns(response.Object);
        //    context.Setup(ctx => ctx.Session).Returns(session.Object);
        //    context.Setup(ctx => ctx.Server).Returns(server.Object);
        //    context.Setup(ctx => ctx.User).Returns(user.Object);

        //    return context.Object;
            
        //}
        //private static string Path(string url)
        //{
        //    int pos1 = url.IndexOf("//");
        //    int pos2 = url.IndexOf("/", pos1 + 2); 
        //    return url.Substring(pos2);
        //}
        //private static string PathInfo(string url)
        //{
        //    string path = Path(url);
        //    int dotPos = path.IndexOf(".");
        //    if (dotPos >= 0)
        //    {
        //        int pos2 = path.IndexOf("/", dotPos + 1);
        //        if (pos2 >= 0)
        //        {
        //            return path.Substring(pos2);
        //        } 
        //    }
        //    return "";
        //}

        //public static HttpContextBase HttpContext()
        //{
        //    return HttpContext("");
        //}
    }
}
