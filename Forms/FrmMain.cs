using alan_wake_2_rmdtoc_Tool.Forms;
using Controls;
using Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace alan_wake_2_rmdtoc_Tool
{
    public partial class FrmMain : Form
    {
        HashSet<rmdtoc> Modifiedtrmdtoc = new HashSet<rmdtoc>();
        public FrmMain()
        {
            InitializeComponent();

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            listView1.Items.Clear();
            var Tag = e.Node.Tag as FolderInfo;



            if (Tag == null)
                return;



            foreach (var item in Tag.Files)
            {
                var LVitem = new ListViewItem();
                LVitem.Text = item.Name;
                LVitem.Tag = item;
                LVitem.ImageIndex = 0;
                LVitem.SubItems.Add(item.GetOffset());
                LVitem.SubItems.Add(item.GetSize());
                listView1.Items.Add(LVitem);
            }
        }

        private void eportSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderDialog folderDialog = new FolderDialog();
            if (folderDialog.ShowDialog() != DialogResult.OK)
                return;
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                var fileInfo = item.Tag as FileInfo;
                File.WriteAllBytes(Path.Combine(folderDialog.FileName, fileInfo.Name), fileInfo.GetFile());
            }

            MessageBox.Show("Done!");
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "rmdtoc Files|*.rmdtoc";
            ofd.Multiselect = true;
            ofd.Title = "Select rmdtoc Files";
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            treeView1.Clear();
            Modifiedtrmdtoc.Clear();



            foreach (var FilePath in ofd.FileNames)
            {
                var Root = new rmdtoc(FilePath).Root;
                treeView1.Nodes.Add(Root);
            }

            saveToolStripMenuItem.Enabled = true;
            expotAllToolStripMenuItem.Enabled = true;
            replaceSelectedFileToolStripMenuItem.Enabled = true;
            eportSelectedToolStripMenuItem.Enabled = true;
            toolToolStripMenuItem.Enabled = true;
        }

        private void expotAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count == 0)
            {
                MessageBox.Show("list is empty!");
                return;
            }

            FolderDialog folderDialog = new FolderDialog();
            if (folderDialog.ShowDialog() != DialogResult.OK)
                return;

            foreach (ListViewItem item in listView1.Items)
            {
                var fileInfo = item.Tag as FileInfo;
                File.WriteAllBytes(Path.Combine(folderDialog.FileName, fileInfo.Name), fileInfo.GetFile());
            }
        }

        private void stringTableEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FrmStringTable().Show();
        }

        private void replaceSelectedFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("No file selected!");
                return;
            }

            if (listView1.SelectedItems.Count > 1)
            {
                MessageBox.Show("Select only one file!");
                return;
            }

            var ofd = new OpenFileDialog();
            ofd.Filter = "All Files|*.*";
            ofd.Title = "Select File";
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            if (!File.Exists(ofd.FileName))
            {
                MessageBox.Show("File not found!");
                return;
            }

            var fileInfo = listView1.SelectedItems[0].Tag as FileInfo;
            fileInfo.IsEdited = true;
            fileInfo.NewFileBytes = File.ReadAllBytes(ofd.FileName);
            Modifiedtrmdtoc.Add(fileInfo.Rmdtoc);

            MessageBox.Show("Done!, Don't forget to save!");
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var rmdtoc in Modifiedtrmdtoc)
            {
                rmdtoc.Save();
            }
            MessageBox.Show("Done!");
            Modifiedtrmdtoc.Clear();
        }

        private void treeView1_BeforeClear_1(object sender, EventArgs e)
        {
            foreach (TreeNode folder in treeView1.Nodes)
            {
                (folder.Tag as FolderInfo).Rmdtoc.Dispose();
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 1) return;

            var fileInfo = listView1.SelectedItems[0].Tag as FileInfo;

            if (fileInfo.Name == "string_table.bin" || fileInfo.GetId() == "RMDL")
            {
                var MStream = new MStream(fileInfo.GetFile());
                var frm = new FrmStringTable(MStream);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    fileInfo.IsEdited = true;
                    fileInfo.NewFileBytes = MStream.ToArray();
                    Modifiedtrmdtoc.Add(fileInfo.Rmdtoc);
                    MessageBox.Show("Done!");
                }
            }
        }
    }
}
