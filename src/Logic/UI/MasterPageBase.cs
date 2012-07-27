using System;
using System.Web.UI;

namespace Bootstrap.Logic.UI
{
    public class MasterPageBase : MasterPage
    {
        protected override void OnInit(EventArgs e)
        {
            ((umbraco.UmbracoDefault)Page).ValidateRequest = false;
            base.OnInit(e);
        }
    }
}
