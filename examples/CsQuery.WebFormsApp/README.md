##Sample WebForms Application

This application contains a simple framework that lets you pipeline CsQuery into `Page` and `UserControl` rendering so you can perform manipulation of the HTML before it is finally rendered. This works by overriding the `Render` and `RenderControl` methods. The app shows a few things you can do with this; to configure this in an existing application, follow the instructions below. It's probably easist just to look at the `Default.aspx` and `WebUserControl1.ascx` classes to see how it works, though.

####Configuration

You'll need to copy several classes to your application. They are all found in the `CsQueryPage` folder of the sample app:

    CsQueryPage.cs
    CsQueryUserControl.cs
    ICsQueryControl.cs


####Using It

To create a page or user control that can use CsQuery, just inherit `CsQueryPage` or `CsQueryUserControl` instead of the default `System.Web.UI.Page` and `System.Web.UI.UserControl` classes. This exposes a new member:

    public CQ Doc;

You also have two new overridabe methods:

    public virtual void Cq_Render();

    public virtual void Cq_RenderUpdatePanel(CQ dom, string updatePanelId);

You simply override one of these methods, and then write code to manipulate the `Doc` object (or the `doc` parameter, if it's an update panel).

For example, this code would put a border around every div:

    public class _Default : CsQueryPage
    {
      
        // Method is is called at `Page_Render` and the HTML will already reflect anything
        // that's been done with web controls.

        public void Cq_Render() {
            Doc["div"].Css("border", "1px solid red;");
        }
    }

For UpdatePanels, a single method will be called for each one during an async update:

    public void Cq_RenderUpdatePanel(CQ doc, string updatePanelId) {
            Doc["div"].Css("border", "1px solid red;");
    }

In this case, the CQ object will be passed as a parameter, since it will be different for each UpdatePanel.
