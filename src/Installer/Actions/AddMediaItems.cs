using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using Bootstrap.Installer.Utils;
using umbraco.BusinessLogic;
using umbraco.DataLayer;
using umbraco.IO;
using umbraco.cms.businesslogic.datatype.controls;
using umbraco.cms.businesslogic.media;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;

namespace Bootstrap.Installer.Actions
{
    public class AddMediaItems : IPackageAction
    {
        private readonly User adminUser = new User(0);
        private readonly MediaType folderType = MediaType.GetByAlias("Folder");
        private readonly MediaType imageType = MediaType.GetByAlias("Image");
        private readonly IDataType uploadField = new Factory().GetNewObject(new Guid("5032a6e6-69e3-491d-bb28-cd31cd11086c"));
        private readonly ISqlHelper sqlHelper = Application.SqlHelper;
        private const string BootstrapPath = "/umbraco/developer/Bootstrap/";

        public bool Execute(string packageName, XmlNode xmlData)
        {
            try
            {
                var path = HttpContext.Current.Server.MapPath(string.Concat(BootstrapPath, "media.json"));
                var imageDic = new Dictionary<int, int>();

                // Set images in media library
                var picturesFolder = Media.MakeNew("Bootstrap Images", folderType, adminUser, -1);

                // Brazil
                var brazilFolder = Media.MakeNew("Brazil", folderType, adminUser, picturesFolder.Id);
                imageDic.Add(1401, CreateImage(brazilFolder.Id, "Holambra", "holambra.jpg").Id);
                imageDic.Add(1402, CreateImage(brazilFolder.Id, "Frevo", "ibirapuera-three.jpg").Id);
                imageDic.Add(1405, CreateImage(brazilFolder.Id, "Iguazu", "iguazu.jpg").Id);
                imageDic.Add(1407, CreateImage(brazilFolder.Id, "Liberdade", "liberdade-one.jpg").Id);
                imageDic.Add(1410, CreateImage(brazilFolder.Id, "Praca da se", "praca-da-se.jpg").Id);

                // London
                var londonFolder = Media.MakeNew("London", folderType, adminUser, picturesFolder.Id);
                imageDic.Add(1430, CreateImage(londonFolder.Id, "Jubilee Walkway", "jubilee-walkway.jpg").Id);
                imageDic.Add(1431, CreateImage(londonFolder.Id, "London Bridge", "london-bridge.jpg").Id);
                imageDic.Add(1433, CreateImage(londonFolder.Id, "Wimbledon Common", "wimbledon-common.jpg").Id);

                // Spain
                var spainFolder = Media.MakeNew("Spain", folderType, adminUser, picturesFolder.Id);
                imageDic.Add(1435, CreateImage(spainFolder.Id, "Alhambra", "alhambra.jpg").Id);
                imageDic.Add(1436, CreateImage(spainFolder.Id, "Cordoba", "cordoba.jpg").Id);
                imageDic.Add(1437, CreateImage(spainFolder.Id, "El retiro", "el-retiro.jpg").Id);
                imageDic.Add(1438, CreateImage(spainFolder.Id, "Sevilla", "sevilla.jpg").Id);
                imageDic.Add(1439, CreateImage(spainFolder.Id, "Tenerife", "tenerife.jpg").Id);

                // Images folder
                var imagesFolder = Media.MakeNew("Others", folderType, adminUser,picturesFolder.Id);
                imageDic.Add(1395, CreateImage(imagesFolder.Id, "Bootstrap", "bootstrap-from-twitter.jpg").Id);
                imageDic.Add(1396, CreateImage(imagesFolder.Id, "Settings Less", "settings-less.jpg").Id);

                var json = imageDic.DictionaryToJson();
                File.WriteAllText(path, json);
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
            return "AddMediaItems";
        }

        public bool Undo(string packageName, XmlNode xmlData)
        {
            try
            {
                foreach (var rootMedia in Media.GetRootMedias().Where(x => x.Text == "Bootstrap Images"))
                {
                    rootMedia.delete(true);
                }

                var path = HttpContext.Current.Server.MapPath(string.Concat(BootstrapPath, "media.json"));
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

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
            string sample = "<Action runat=\"install\" undo=\"true\" alias=\"AddMediaItems\" />";
            return helper.parseStringToXmlNode(sample);
        }

        private Media CreateImage(int parentId, string name, string fileName)
        {
            var media = Media.MakeNew(name, imageType, adminUser, parentId);

            string sql = string.Format(@"
                        select cpd.id as id from cmsPropertyData cpd
                            inner join cmsPropertyType cpt on cpd.propertytypeid = cpt.Id
                            inner join cmsDataType cdt on cpt.dataTypeId = cdt.nodeId
                        where cpd.contentNodeId = {0}
                        and cdt.controlId = '{1}'", media.Id, uploadField.Id);

            var propertyId = sqlHelper.ExecuteScalar<int>(sql);

            var file = new FileInfo(IOHelper.MapPath(SystemDirectories.Media + "/" + propertyId + "/" + fileName));

            // If the directory doesn't exist then create it.
            if (file.Directory != null && !file.Directory.Exists)
            {
                file.Directory.Create();
            }

            // Write the file to the media folder.
            var bytes = File.ReadAllBytes(HttpContext.Current.Server.MapPath(BootstrapPath + fileName));
            File.WriteAllBytes(file.FullName, bytes);

            var umbracoFile = SystemDirectories.Media + "/" + propertyId + "/" + fileName;
            if (umbracoFile.StartsWith("~"))
            {
                umbracoFile = umbracoFile.TrimStart(new[] { '~' });
            }

            if (media.getProperty("umbracoFile") != null)
            {
                media.getProperty("umbracoFile").Value = umbracoFile;
            }

            if (media.getProperty("umbracoExtension") != null)
            {
                media.getProperty("umbracoExtension").Value = Path.GetExtension(file.Name).Replace(".", string.Empty);
            }

            if (media.getProperty("umbracoBytes") != null)
            {
                media.getProperty("umbracoBytes").Value = bytes.Length;
            }

            var image = Image.FromFile(file.FullName);

            if (media.getProperty("umbracoWidth") != null)
            {
                media.getProperty("umbracoWidth").Value = image.Width.ToString();
            }

            if (media.getProperty("umbracoHeight") != null)
            {
                media.getProperty("umbracoHeight").Value = image.Height.ToString();
            }

            // Create a thumbnail from the image.
            string fileNameThumb = Path.Combine(file.Directory.FullName, file.Name.Replace(Path.GetExtension(file.Name), "_thumb.jpg"));
            GenerateThumbnail(image, 100, image.Width, image.Height, fileNameThumb);

            // Clean the image.
            image.Dispose();

            // Save the media file to update the xml and to make sure some event's get called.
            media.Save();
            media.XmlGenerate(new XmlDocument());
            return media;
        }

        private void GenerateThumbnail(Image image, int maxWidthHeight, int fileWidth, int fileHeight, string thumbnailFileName)
        {
            // Generate thumbnail.
            float fx = fileWidth / maxWidthHeight;
            float fy = fileHeight / maxWidthHeight;

            // Must fit in thumbnail size.
            float f = Math.Max(fx, fy); if (f < 1) f = 1;
            int widthTh = (int)(fileWidth / f); int heightTh = (int)(fileHeight / f);

            // Create new image with best quality settings.
            var bp = new Bitmap(widthTh, heightTh);
            var g = Graphics.FromImage(bp);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // Copy the old image to the new and resized.
            var rect = new Rectangle(0, 0, widthTh, heightTh);
            g.DrawImage(image, rect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);

            // Copy metadata.
            var codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo codec = null;
            foreach (var t in codecs.Where(t => t.MimeType.Equals("image/jpeg")))
            {
                codec = t;
            }

            // Set compresion ratio to 90%.
            var ep = new EncoderParameters();
            ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);

            // Save the new image.
            bp.Save(thumbnailFileName, codec, ep);
            bp.Dispose();
            g.Dispose();
        }
    }
}
