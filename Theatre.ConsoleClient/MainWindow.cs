using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Akka.Actor;
using Theatre.ConsoleClient.Actor;

namespace Theatre.ConsoleClient
{
    public partial class MainWindow : Form
    {
        public IActorRef UiActor { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        public void CreateRootNode(string rootPath)
        {
            this.TreeView.Invoke((MethodInvoker) delegate
            {
                var pathParts = rootPath.Split('\\');
                var nodeCollection = this.TreeView.Nodes;
                foreach (var pathPart in pathParts)
                {
                    var node = nodeCollection.Find(pathPart, false).SingleOrDefault();
                    if (node == null)
                    {
                        var newTreeNode = new TreeNode(pathPart);
                        nodeCollection.Add(newTreeNode);
                        nodeCollection = newTreeNode.Nodes;
                    }
                }
            });
        }

        private void TreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var s = (TreeView) sender;
            var node = s.SelectedNode;
            var path = new List<string>();
            while (node.Parent != null)
            {
                path.Add(node.Name);
            }

            path.Reverse();

            this.UiActor.Tell(new OpenDirectory(string.Join("\\", path)));
        }
    }
}