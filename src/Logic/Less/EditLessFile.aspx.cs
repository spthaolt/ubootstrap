using System.Web.UI;
using umbraco.BasePages;
using System;
using System.IO;
using umbraco;
using umbraco.uicontrols;

namespace Bootstrap.Logic.Less
{
    public partial class EditLessFile : UmbracoEnsuredPage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (!UmbracoPanel.hasMenu)
            {
                return;
            }

            var imageButton = UmbracoPanel.Menu.NewImageButton();
            imageButton.AlternateText = "Save File";
            imageButton.ImageUrl = GlobalSettings.Path + "/images/editor/save.gif";
            imageButton.Click += MenuSaveClick;
        }

        protected override void OnLoad(EventArgs e)
        {
            var file = Request.QueryString["file"];
            var path = Constants.LessPath + file;
            TxtName.Text = file;
            var appPath = Request.ApplicationPath;
            if (appPath == "/")
            {
                appPath = string.Empty;
            }

            LtrlPath.Text = appPath + path;
            if (IsPostBack)
            {
                return;
            }

            string fullPath = Server.MapPath(path);
            if (File.Exists(fullPath))
            {
                string content;
                using (var streamReader = File.OpenText(fullPath))
                {
                    content = streamReader.ReadToEnd();
                }

                if (string.IsNullOrEmpty(content))
                {
                    return;
                }

                EditorSource.Text = content;
            }
            else
            {
                Feedback.Text = (string.Format("The file '{0}' does not exist.", file));
                Feedback.type = Feedback.feedbacktype.error;
                Feedback.Visible = true;
                UmbracoPanel.hasMenu = NamePanel.Visible = PathPanel.Visible = EditorPanel.Visible = false;
            }
        }

        private bool SaveConfigFile(string filename, string contents)
        {
            try
            {
                var path = Server.MapPath(Constants.LessPath + filename);
                using (var text = File.CreateText(path))
                {
                    text.Write(contents);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void MenuSaveClick(object sender, ImageClickEventArgs e)
        {
            if (SaveConfigFile(TxtName.Text, EditorSource.Text))
            {
                ClientTools.ShowSpeechBubble(speechBubbleIcon.save, ui.Text("speechBubbles", "fileSavedHeader"), ui.Text("speechBubbles", "fileSavedText"));
            }
            else
            {
                ClientTools.ShowSpeechBubble(speechBubbleIcon.error, ui.Text("speechBubbles", "fileErrorHeader"), ui.Text("speechBubbles", "fileErrorText"));
            }
        }
    }
}
