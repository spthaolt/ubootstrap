using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using Bootstrap.Installer.Utils;
using umbraco;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.language;
using umbraco.cms.businesslogic.web;
using umbraco.interfaces;
using helper = umbraco.cms.businesslogic.packager.standardPackageActions.helper;

namespace Bootstrap.Installer.Actions
{
    public class UpdateRoot : IPackageAction
    {
        private const string DomainFormat = "{0}.umbraco.local";
        private const string IndexSetFormat = "Bootstrap{0}IndexSet";
        private const string BootstrapPath = "/umbraco/developer/Bootstrap/";

        public bool Execute(string packageName, XmlNode xmlData)
        {
            try
            {
                if (xmlData.Attributes == null)
                {
                    return false;
                }

                var documentName = xmlData.Attributes["documentName"].Value;
                var language = xmlData.Attributes["language"].Value;
                foreach (var root in Document.GetRootDocuments().Where(x => x.Text == documentName))
                {
                    UpdateMediaImages(root);
                    SetDomain(root, language);
                    SetExamineParentId(root, language);
                    root.PublishWithChildrenWithResult(User.GetUser(0));
                }

                library.RefreshContent();
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
            return "UpdateRoot";
        }

        public bool Undo(string packageName, XmlNode xmlData)
        {
            try
            {
                if (xmlData.Attributes == null)
                {
                    return false;
                }

                var documentName = xmlData.Attributes["documentName"].Value;
                var language = xmlData.Attributes["language"].Value;
                foreach (var root in Document.GetRootDocuments().Where(x => x.Text == documentName))
                {
                    root.delete(true);
                }

                // try to delete the examine index directory
                var culture = CultureInfo.GetCultureInfo(language);
                var indexName = string.Format(IndexSetFormat, culture.TwoLetterISOLanguageName.ToUpperInvariant());
                var examineIndexFile = xmlHelper.OpenAsXmlDocument(VirtualPathUtility.ToAbsolute("~/config/ExamineIndex.config"));
                var examineLuceneIndexSetsNode = examineIndexFile.SelectSingleNode("//ExamineLuceneIndexSets");
                if (examineLuceneIndexSetsNode == null)
                {
                    return false;
                }

                var index = examineLuceneIndexSetsNode.SelectSingleNode("//IndexSet[@SetName = '" + indexName + "']");
                if (index == null || index.Attributes == null)
                {
                    return false;
                }

                var indexPath = VirtualPathUtility.ToAbsolute(index.Attributes["IndexPath"].Value);
                var indexDirectory = HttpContext.Current.Server.MapPath(indexPath);
                Directory.Delete(indexDirectory);
                return true;
            }
            catch (Exception ex)
            {
                Log.Add(LogTypes.PackagerInstall, -1, ex.Message);
                return false;
            }
        }

        public XmlNode SampleXml()
        {
            return helper.parseStringToXmlNode("<Action runat=\"install\" undo=\"true\" alias=\"UpdateRoot\" documentName=\"Home\" language=\"en-US\" />");
        }

        private void UpdateMediaImages(Document root)
        {
            Dictionary<int, int> imageDictionary;
            var path = HttpContext.Current.Server.MapPath(string.Concat(BootstrapPath, "media.json"));
            if (File.Exists(path))
            {
                imageDictionary = File.ReadAllText(path).JsonToDictionary<Dictionary<int, int>>();
            }
            else
            {
                return;
            }

            int imageId;
            if (root.getProperty("bodyImage") != null && root.getProperty("bodyImage").Value is int)
            {
                if (imageDictionary.TryGetValue((int)root.getProperty("bodyImage").Value, out imageId))
                {
                    root.getProperty("bodyImage").Value = imageId;
                }
            }

            foreach (Document doc in root.GetDescendants())
            {
                if (doc.getProperty("bodyImage") != null && doc.getProperty("bodyImage").Value is int)
                {
                    if (imageDictionary.TryGetValue((int)doc.getProperty("bodyImage").Value, out imageId))
                    {
                        doc.getProperty("bodyImage").Value = imageId;
                    }
                }

                if (doc.getProperty("galleryThumbnail") != null && doc.getProperty("galleryThumbnail").Value is int)
                {
                    if (imageDictionary.TryGetValue((int)doc.getProperty("galleryThumbnail").Value, out imageId))
                    {
                        doc.getProperty("galleryThumbnail").Value = imageId;
                    }
                }
            }
        }

        private void SetDomain(Document root, string language)
        {
            var langId = Language.GetByCultureCode(language).id;
            var culture = CultureInfo.GetCultureInfo(language);
            var domain = string.Format(DomainFormat, culture.TwoLetterISOLanguageName.ToLowerInvariant());
            Domain.MakeNew(domain, root.Id, langId);
        }

        private void SetExamineParentId(Document root, string language)
        {
            var culture = CultureInfo.GetCultureInfo(language);
            var indexName = string.Format(IndexSetFormat, culture.TwoLetterISOLanguageName.ToUpperInvariant());

            // Update the examine index parent id
            var examineIndexFile = xmlHelper.OpenAsXmlDocument(VirtualPathUtility.ToAbsolute("~/config/ExamineIndex.config"));
            var examineLuceneIndexSetsNode = examineIndexFile.SelectSingleNode("//ExamineLuceneIndexSets");
            if (examineLuceneIndexSetsNode == null)
            {
                return;
            }

            var index = examineLuceneIndexSetsNode.SelectSingleNode("//IndexSet[@SetName = '" + indexName + "']");
            if (index == null || index.Attributes == null)
            {
                return;
            }

            index.Attributes["IndexParentId"].Value = root.Id.ToString();
            examineIndexFile.Save(HttpContext.Current.Server.MapPath("/config/ExamineIndex.config"));
        }
    }
}
