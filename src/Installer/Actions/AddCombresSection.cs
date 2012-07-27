using System.Configuration;
using System.Web.Configuration;
using System.Xml;
using Combres;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;

namespace Bootstrap.Installer.Actions
{
    public class AddCombresSection : IPackageAction
    {
        public string Alias()
        {
            return "AddCombresSection";
        }

        public bool Execute(string packageName, XmlNode xmlData)
        {
            var webConfig = WebConfigurationManager.OpenWebConfiguration("~");
            if (!webConfig.HasFile)
            {
                return false;
            }

            var combres = webConfig.GetSection("combres");
            if (combres != null)
            {
                return true;
            }

            var configuration = new ConfigSectionSetting { DefinitionUrl = "~/config/Combres.config" };
            webConfig.Sections.Add("combres", configuration);
            try
            {
                webConfig.Save(ConfigurationSaveMode.Modified);
                Log.Add(LogTypes.Custom, -1, "web.config has been updated");
                return true;
            }
            catch
            {
                Log.Add(LogTypes.Error, -1, "Unable to update the handlers section");
                return false;
            }
        }

        public bool Undo(string packageName, XmlNode xmlData)
        {
            var webConfig = WebConfigurationManager.OpenWebConfiguration("~");
            if (!webConfig.HasFile)
            {
                return false;
            }

            var combres = webConfig.GetSection("combres");
            if (combres == null)
            {
                return true;
            }

            webConfig.Sections.Remove("combres");

            // try saving the web.config
            try
            {
                webConfig.Save(ConfigurationSaveMode.Modified);
                Log.Add(LogTypes.Custom, -1, "web.config has been updated");
                return true;
            }
            catch
            {
                Log.Add(LogTypes.Error, -1, "Unable to update the handlers section");
                return false;
            }
        }

        public XmlNode SampleXml()
        {
            return helper.parseStringToXmlNode("<Action runat=\"install\" undo=\"true\" alias=\"AddCombresSection\" />");
        }
    }
}
