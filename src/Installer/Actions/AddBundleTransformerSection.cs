using System.Configuration;
using System.Web.Configuration;
using System.Xml;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;

namespace Bootstrap.Installer.Actions
{
    public class AddBundleTransformerSection : IPackageAction
    {
        public string Alias()
        {
            return "AddBundleTransformerSection";
        }

        public bool Execute(string packageName, XmlNode xmlData)
        {
            var webConfig = WebConfigurationManager.OpenWebConfiguration("~");
            if (!webConfig.HasFile)
            {
                return false;
            }

            var bundleTransformerSection = webConfig.GetSection("BundleTransformer");
            if (bundleTransformerSection != null)
            {
                return true;
            }

            //var configuration = new ConfigurationSection() { DefinitionUrl = "~/config/BundleTransformer.config" };
            //webConfig.Sections.Add("BundleTransformer", configuration);
            //try
            //{
            //    webConfig.Save(ConfigurationSaveMode.Modified);
            //    Log.Add(LogTypes.Custom, -1, "web.config has been updated");
            //    return true;
            //}
            //catch
            //{
            //    Log.Add(LogTypes.Error, -1, "Unable to update the handlers section");
            //    return false;
            //}
            return false;
        }

        public bool Undo(string packageName, XmlNode xmlData)
        {
            var webConfig = WebConfigurationManager.OpenWebConfiguration("~");
            if (!webConfig.HasFile)
            {
                return false;
            }

            var combres = webConfig.GetSection("BundleTransformer");
            if (combres == null)
            {
                return true;
            }

            webConfig.Sections.Remove("BundleTransformer");

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
            return helper.parseStringToXmlNode("<Action runat=\"install\" undo=\"true\" alias=\"AddBundleTransformerSection\" />");
        }
    }
}
