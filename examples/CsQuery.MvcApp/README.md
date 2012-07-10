##Sample MVC Application

This application contains a simple framework that lets you pipeline CsQuery into view rendering so you can perform manipulation of the HTML before it is finally rendered. This works by replacing the default view engine & controllers. The app shows a few things you can do with this.

Instructions for using this are below. To see how it works, just to look at the example app, especially the `HomeController.cs` and `AboutController.cs` classes.

####Configuration

You'll need to copy several classes to your application. They are all found in the `CsQueryView` folder of the sample app:

    CsQueryViewEngine.cs
    CsQueryView.cs
    CsQueryController.cs
    ICsQueryController.cs

Next, replace the default Razor engine with the CsQuery implementation, just add these two lines to `Application_Start()`. This won't change anything unless you specifically invoke CsQuery methods as described later.

    ViewEngines.Engines.Clear();
    ViewEngines.Engines.Add(new CsQueryViewEngine());


####Using It

To create a controller that can use CsQuery, just implement `ICsQueryController` in any controller. This exposes a new member of the controller:

    public CQ Doc;


Alternatively, just inherit `CsQuery.Mvc.CsQueryController` instead of the default `Controller`.
 
You can create action methods that being with `Cq_` which will be called after the `ActionResult` is returned from an action. For example,

    public void Cq_Index() { .. }

would be called whenever the action associated with the view `Index` was invoked. The `Doc` object will be populated with the HTML rendered from the action, which you may manipulate before it is output to the client.

You can also bind CsQuery actions to partial views:

    public void Cq__LogOnPartial(CQ doc)

would be called for the rendering of the default `_LogOnPartial` view. The `CQ` object passed as its single parameter contains only the markup from the partial view. Note that in the default MVC3 template, this view is actually `Shared\_LogOnPartial.cshtml`. The CsQuery controller lets you match just the view name without the full path or the `cshtml` extension. If this happened to be ambigious you can include the full path using underscores instead of slashes:

    public void Cq_Shared__LogOnPartial(CQ doc)

You can also create a method bound to a partial view only for a specific action, e.g.

    public void Cq_Index__LogOnPartial(CQ doc)

will only be called for the Index action.

There are also two general purpose methods available:

    public void Cq_Start() { ... }
    public void Cq_End() { ... }

The `Cq_Start` method will be called before the action-specific method in a given controller regardless of the action; the `Cq_End` method likewise will be called afterwards. All the Cq methods are always called after any code is executed in the real action however - this handling doesn't begin until the `ActionResult` is returned.

For example, this code would put a border around every div:

    public class HomeController : CsQueryController
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";
            return View();
        }

        // Method is called after Index() returns its results with 
        // the Doc property populated by the HTML before it's rendered

        public void Cq_Index() {
            Doc.Find("div").Css("border", "1px solid red;");
        }
    }

If you wanted something to run after all other CsQuery processing for *all* methods of this controller, e.g. some general-purpose page setup, add a start or end method:

    public void Cq_End() {
        // remove any invisible divs. They won't be needed.
        Doc["div:hidden"].Remove();
    }


