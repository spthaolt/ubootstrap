using System.Xml;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;
using System.Web;

namespace Bootstrap.Installer.Actions
{
    /// <summary>
    /// This Package action will add a new entry to the ExamineSettings.config file
    /// </summary>
    /// <remarks>
    /// I hope this package action will be part of the PackageActionsContrib Project
    /// </remarks>
    public class AddExamineIndexProvider : IPackageAction
    {
        /// <summary>
        /// This Alias must be unique and is used as an identifier that must match the alias in the package action XML
        /// </summary>
        /// <returns>The Alias in string format</returns>
        public string Alias()
        {
            return "AddExamineIndexProvider";
        }

        /// <summary>
        /// Appends the xmlData  to the ExamineSettings.config file
        /// </summary>
        /// <param name="packageName">Name of the package that we install</param>
        /// <param name="xmlData">The data that must be appended to the ExamineSettings.config file</param>
        /// <returns>True when succeeded</returns>
        public bool Execute(string packageName, XmlNode xmlData)
        {
            // Set result default to false
            bool result = false;

            // Check if the xmlData has a childnode (the IndexSet rule node)
            if (xmlData.HasChildNodes)
            {
                // Open the examine index file
                XmlDocument examineIndexFile = umbraco.xmlHelper.OpenAsXmlDocument(VirtualPathUtility.ToAbsolute("~/config/ExamineSettings.config"));

                // Select ExamineLuceneIndexSets node in the config file
                XmlNode provider = examineIndexFile.SelectSingleNode("//Examine/ExamineIndexProviders/providers");

                // Select IndexSet from the supplied xmlData
                XmlNode indexSetNode = xmlData.SelectSingleNode("./add");

                // Add the node
                if (provider != null && provider.OwnerDocument != null && indexSetNode != null)
                {
                    var newNode = provider.OwnerDocument.ImportNode(indexSetNode, true);
                    provider.AppendChild(newNode);
                }

                // Save the config file
                examineIndexFile.Save(HttpContext.Current.Server.MapPath(VirtualPathUtility.ToAbsolute("/config/ExamineSettings.config")));

                // No errors so the result is true
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Removes the xmlData Node from the ExamineSettings.config file based on the rulename 
        /// </summary>
        /// <param name="packageName">Name of the package that we install</param>
        /// <param name="xmlData">The data</param>
        /// <returns>True when succeeded</returns>
        public bool Undo(string packageName, XmlNode xmlData)
        {
            bool result = false;

            // Check if the xmlData has a childnode (the IndexSet rule node)
            if (xmlData.HasChildNodes)
            {
                // Open the examine index file
                XmlDocument examineIndexFile = umbraco.xmlHelper.OpenAsXmlDocument(VirtualPathUtility.ToAbsolute("~/config/ExamineSettings.config"));

                // Select ExamineLuceneIndexSets node in the config file
                XmlNode provider = examineIndexFile.SelectSingleNode("//Examine/ExamineIndexProviders/providers");

                // Select IndexSet from the supplied xmlData
                XmlNode indexSetNode = xmlData.SelectSingleNode("./add");

                // Get the index name
                if (indexSetNode != null && indexSetNode.Attributes != null)
                {
                    string indexName = indexSetNode.Attributes["name"].Value;

                    // Select the node by name from the config file
                    if (provider != null)
                    {
                        XmlNode index = provider.SelectSingleNode("//add[@name = '" + indexName + "']");
                        if (index != null)
                        {
                            // Index is found, remove it from the xml document
                            provider.RemoveChild(index);

                            //Save the modified configuration file
                            examineIndexFile.Save(HttpContext.Current.Server.MapPath("/config/ExamineSettings.config"));
                        }
                    }
                }

                result = true;
            }
            return result;
        }

        /// <summary>
        /// Returns a Sample XML Node
        /// </summary>
        /// <returns>The sample xml as node</returns>
        public XmlNode SampleXml()
        {
            return helper.parseStringToXmlNode(
                "<Action runat=\"install\" undo=\"true\" alias=\"AddExamineIndexProvider\">" +
                    "<add name=\"BootstrapENIndexer\"" +
                    "type=\"UmbracoExamine.UmbracoContentIndexer, UmbracoExamine\"" +
                    "dataService=\"UmbracoExamine.DataServices.UmbracoDataService, UmbracoExamine\"" +
                    "indexSet=\"BootstrapENIndexSet\"" +
                    "supportUnpublished=\"false\"" +
                    "supportProtected=\"false\"" +
                    "runAsync=\"true\"" +
                    "interval=\"30\"" +
                    "analyzer=\"Lucene.Net.Analysis.Standard.StandardAnalyzer, Lucene.Net\"" +
                    "enableDefaultEventHandler=\"true\"/>" +
                "</Action>");
        }
    }
}