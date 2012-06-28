##Sample MVC Application

This shows how to pipeline CsQuery into view rendering so you can perform manipulation of the HTML before it is finally rendered. This works by replacing the default view engine & controllers. The app shows a few things you can do with this; to configure this in an existing application, follow the instructions below.

####Configuration

You'll need to copy three classes to your application. They are all found in the `CsQueryView` folder of the sample app:

    CsQueryViewEngine.cs
    CsQueryView.cs
    CsQueryController.cs

Next, replace the default Razor engine with the CsQuery implementation, just add these two lines to `Application_Start()`. This won't change anything unless you specifically invoke CsQuery methods as described later.

    ViewEngines.Engines.Clear();
    ViewEngines.Engines.Add(new CsQueryViewEngine());


####Using It

To create a controller that can use CsQuery, just inherit `CsQuery.Mvc.CsQueryController` instead of the default `Controller`. Then you have a new method available called `SetCqHandler` which lets you reference a function where you'll be able to manipulate the HTML after it's rendered. For example, this code would put a border around every div:

    public class HomeController : CsQueryController
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            SetCqHandler(FinishRendering);

            return View();
        }

        private void FinishRendering(CQ doc) {
            doc.Find("div").Css("border", "1px solid red;");
        }
    }

You can also do it using an anonymous function, of course:

    SetCqHandler((doc) =>
    {
        doc["div"].Css("border", "1px solid red;");
    });


If you want to bind a handler to a specific partial view, you can do that too:

    SetCqHandler((doc,"LogOnPartial") =>
    {
        doc["a"].Text("New Log On Text");
    });

This works by matching the string to the end of the view's internal class name. This is basically the path, with underscores instead of slashes and dots. You can omit the "_cshtml" part. Be careful if you have partials with the same name in two different folders, you'll want to include more of the path in that case.

Usually, though, you won't need to bother with partial views. It's more efficient to just handle the entire document at once, and use CSS selectors to identify specific parts of it (that could be rendered from a partial view).
