using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Routing;

using Moq;

namespace CsQuery.Mvc.Tests
{
    public class TestObjectFactory
    {

        
        public static T ContextBoundController<T>(string controller, string action)  where T: Controller, new()
        {
            var ctx = HttpContext(controller+"/"+action);
            var ctlr = new T();

            var controllerContext = new ControllerContext(ctx, RouteData(controller, action), ctlr);

            ctlr.ControllerContext = controllerContext;

            return ctlr;
        }
        private static RouteData RouteData(string controller,string action) {
            var rd = new RouteData();
            rd.Values.Add("action", action);
            rd.Values.Add("controller", controller);
            rd.RouteHandler = new System.Web.Mvc.MvcRouteHandler();

            return rd;
        }
        public static HttpContextBase HttpContext(string path)
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();
            var user = new Mock<IPrincipal>();
            var identity = new Mock<IIdentity>();

            request.Setup(req => req.ApplicationPath).Returns("~/");
            request.Setup(req => req.AppRelativeCurrentExecutionFilePath).Returns("~/");
            request.Setup(req => req.HttpMethod).Returns("GET");

            string url = "http://www.test.com/" + path;
            request.Setup(req => req.Url).Returns(new Uri(url));
            request.Setup(req => req.Path).Returns(Path(url));
            request.Setup(req => req.PathInfo).Returns(PathInfo(url));

            response.Setup(res => res.ApplyAppPathModifier(It.IsAny<string>()))
                .Returns((string virtualPath) => virtualPath);
            user.Setup(usr => usr.Identity).Returns(identity.Object);
            identity.Setup(ident => ident.IsAuthenticated).Returns(true);

            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session.Object);
            context.Setup(ctx => ctx.Server).Returns(server.Object);
            context.Setup(ctx => ctx.User).Returns(user.Object);

            return context.Object;
        }
        private static string Path(string url)
        {
            int pos1 = url.IndexOf("//");
            int pos2 = url.IndexOf("/", pos1 + 2); 
            return url.Substring(pos2);
        }
        private static string PathInfo(string url)
        {
            string path = Path(url);
            int dotPos = path.IndexOf(".");
            if (dotPos >= 0)
            {
                int pos2 = path.IndexOf("/", dotPos + 1);
                if (pos2 >= 0)
                {
                    return path.Substring(pos2);
                } 
            }
            return "";
        }

        public static HttpContextBase HttpContext()
        {
            return HttpContext("");
        }
    }
}
