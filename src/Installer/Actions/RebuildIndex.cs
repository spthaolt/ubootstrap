using System;
using System.Xml;
using Examine;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;

namespace Bootstrap.Installer.Actions
{
    public class RebuildIndex : IPackageAction
    {
        public bool Execute(string packageName, XmlNode xmlData)
        {
            try
            {
                if (xmlData.Attributes != null)
                {
                    var indexerName = xmlData.Attributes["indexerName"].Value;
                    ExamineManager.Instance.IndexProviderCollection[indexerName].RebuildIndex();
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.Add(LogTypes.PackagerInstall, -1, ex.Message);
                return false;
            }
        }

        public string Alias()
        {
            return "RebuildIndex";
        }

        public bool Undo(string packageName, XmlNode xmlData)
        {
            return true;
        }

        public XmlNode SampleXml()
        {
            return helper.parseStringToXmlNode("<Action runat=\"install\" undo=\"true\" alias=\"RebuildIndex\" indexerName=\"MyIndexerName\" />");
        }
    }
}
