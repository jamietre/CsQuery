##CsQuery.MVC Framework

CsQuery.MVC is a simple framework that lets you pipeline CsQuery into view rendering so you can perform manipulation of the HTML before it is finally rendered. This works by replacing the default view engine & controllers. 

The frameworks also includes a component called the ScriptManager. There's a separate readme in this folder. This allows you to use a convention to indentify client script dependencies from within the script itself. The convention will automatically import all dependencies and wrap them in a single Bundle using MVC 4.0 bundles.

Instructions for using this are below. To see it in action, just to look at the example MVC application under "examples", especially the `HomeController.cs` and `AboutController.cs` classes.

####Configuration

    Install-Package CsQuery.Mvc

To use it, just replace the default Razor engine with the CsQuery implementation by adding the code below to `Application_Start()`':

    ViewEngines.Engines.Clear();
    ViewEngines.Engines.Add(new CsQueryViewEngine());

You can also configure a special layout controller. This controller defines two methods that will be called when the layout is created but before the action-specific methods are invoked, and is invoked, and again after any action-specific methods have run. You configure a layout controller by creating the `CsQueryViewEngine` object with a generic type parameter that identifies it:

    ViewEngines.Engines.Add(new CsQueryViewEngine<Controllers.LayoutController>());
 
Each layout controller should have just two methods, `void Cq_Start()` and `void Cq_End()`. It must also implement `ICsQueryController` just like any other CsQuery.Mvc controller (see below). 

Note that binding the CsQuery view engine will have no effect on your existing controllers; it's functionality only kicks in when a controller implements `ICsQueryController`.

####Using It

First, reference the assembly `CsQuery.Mvc` in your controller code:

    using CsQuery.Mvc;

To create a controller that can use CsQuery, just implement `ICsQueryController` in any controller. Alternatively, you can just inherit `CsQuery.Mvc.CsQueryController` instead of the default `Controller`:


    public class HomeController: CsQueryController
    {
        public ActionResult Index() {
           ...
        }
        ...
    }

This interface exposes a single new property of the controller:

    public CQ Doc {get;}
 
Within a controller that implements `ICsQueryController`, you can create special action methods which will be called after the `ActionResult` is returned from an action. For example,

    public void Cq_Index() { .. }

would be called after the action associated with the view `Index` was invoked. The `Doc` object will be populated with the HTML rendered from the action, which you may manipulate before it is output to the client.

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

####Example

This code would put a 1 pixel red border around every div:

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


