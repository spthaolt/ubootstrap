using System.Web.UI;
using System;
using System.Linq;
using umbraco.cms.businesslogic.web;
using umbraco.uicontrols;

namespace Bootstrap.Installer.UserControls
{
    public partial class Installer : UserControl
    {
        protected override void OnLoad(EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            Install();
        }

        private void Install()
        {
            try
            {
                // Set the sort order for the meta tab in the base document type
                var tab = DocumentType.GetByAlias("Base").getVirtualTabs.Where(x => x.Caption == "Meta").FirstOrDefault();
                if (tab != null)
                {
                    DocumentType.GetByAlias("Base").SetTabSortOrder(tab.Id, 10);
                }
            }
            catch (Exception ex)
            {
                Feedback.type = Feedback.feedbacktype.error;
                Feedback.Text = string.Format("{0}<br />{1}<br />{2}<br />", ex.Message, ex.Source, ex.StackTrace);
            }
        }
    }
}
