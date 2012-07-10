using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CsQuery.AspNet;

namespace CsQuery.WebFormsApp
{
    public partial class _Default : CsQueryPage
    {
        protected override void Cq_Render()
        {
            // Add a binding for each unconfigured link
            // 
            foreach (var link in Doc["a[href='#']"])
            {
                var innerText = link.Cq().Text();
                link["href"] = String.Format("javascript:notImplemented('{0}')",innerText);
            };
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}