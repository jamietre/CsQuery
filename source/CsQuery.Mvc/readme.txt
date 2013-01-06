USING CSQUERY.MVC

You need to register the CsQueryMVC View Engine as follows. Add this to Application_Start() 
in Global.asax.cs

    ViewEngines.Engines.Clear();
    ViewEngines.Engines.Add(new CsQueryViewEngine());

To create a controller that can use CsQuery, just inherit CsQueryController:


    public class HomeController: CsQueryController
    {
        public ActionResult Index() {
           ...
        }
        ...
    }

Alternatively you can implement `ICsQueryController` in any existing controller; this interface 
requires a single new property exist on the controller:

    public CQ Doc {get;}
 
Within a controller that implements `ICsQueryController`, the following optional methods have 
special functionality:

    public void Cq_Start()
    public void Cq_{ActionName}()
    public void Cq_End()
    

Cq_Start() and Cq_End() are run before and after any action-specific methods, regardless of the 
action invoked. A method with the same name as a specific action runs inbetween, e.g. Cq_Index() 
runs for the Index action.

When these methods run, the `Doc` object will be populated with the HTML rendered from the action, 
which you may manipulate before it is output to the client.


####Example

This code would put a 1 pixel red border around every div for the Index action, and removes all 
hidden divs regardless of the action.

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

    public void Cq_End() {
        // remove any invisible divs. They won't be needed.
        Doc["div:hidden"].Remove();
    }


