using System.Collections.Generic;
using System.Text;
using umbraco.BusinessLogic.Actions;
using umbraco.cms.presentation.Trees;
using umbraco.interfaces;

namespace Bootstrap.Logic.Less
{
    public class LoadLessFiles : FileSystemTree
    {
        protected override string FilePath
        {
            get { return Constants.LessPath; }
        }

        protected override string FileSearchPattern
        {
            get { return Constants.LessSearchPattern; }
        }

        public LoadLessFiles(string application)
            : base(application)
        {
        }

        public override void Render(ref XmlTree tree)
        {
            base.Render(ref tree);
            if (!string.IsNullOrEmpty(NodeKey))
            {
                return;
            }
        }

        public override void RenderJS(ref StringBuilder javascript)
        {
            
            javascript.Append(
            @"
                  function openLessEditor(id) { 
                        parent.right.document.location.href = 'developer/Bootstrap/EditLessFile.aspx?file=' + id; 
               }");
        }

        protected override void CreateRootNode(ref XmlTreeNode rootNode)
        {
            rootNode.Text = "Less Files";
            rootNode.Icon = ".sprTreeFolder";
            rootNode.OpenIcon = ".sprTreeFolder_o";
            rootNode.NodeID = "initConfigFiles";
            rootNode.NodeType = rootNode.NodeID + "_" + TreeAlias;
            rootNode.Menu = new List<IAction> { ActionRefresh.Instance };
        }

        protected override void OnRenderFolderNode(ref XmlTreeNode xNode)
        {
            xNode.Menu = new List<IAction> { ActionRefresh.Instance };
            xNode.NodeType = "configFolder";
        }

        protected override void OnRenderFileNode(ref XmlTreeNode xNode)
        {
            xNode.Action = xNode.Action.Replace("openFile", "openLessEditor");
            xNode.Menu = new List<IAction>();
            xNode.Icon = "../../images/umbraco/settingCss.gif";
            xNode.OpenIcon = xNode.Icon;
        }
    }
}
