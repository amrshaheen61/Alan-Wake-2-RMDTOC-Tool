﻿using alan_wake_2_rmdtoc_Tool.Forms;
using Controls;
using Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

                if (item.IsEdited)
                    LVitem.Font = new System.Drawing.Font(LVitem.Font, System.Drawing.FontStyle.Bold);

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
            importfiles.Enabled = true;
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
                var MStream = new MStream(fileInfo.Name, fileInfo.GetFile());
                var frm = new FrmStringTable(MStream);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        fileInfo.IsEdited = true;
                        fileInfo.NewFileBytes = MStream.ToArray();
                        Modifiedtrmdtoc.Add(fileInfo.Rmdtoc);
                        MessageBox.Show("Done!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else if (fileInfo.Name.EndsWith(".png", StringComparison.InvariantCulture) ||
                    fileInfo.Name.EndsWith(".jpg", StringComparison.InvariantCulture) ||
                      fileInfo.Name.EndsWith(".bmp", StringComparison.InvariantCulture) ||
                     fileInfo.Name.EndsWith(".tga", StringComparison.InvariantCulture) ||
                fileInfo.Name.EndsWith(".dds", StringComparison.InvariantCulture) ||
                fileInfo.Name.EndsWith(".tex", StringComparison.InvariantCulture))
            {
                try
                {
                    var MStream = new MStream(fileInfo.Name, fileInfo.GetFile());
                    var frm = new frmImageViewer(MStream);
                    frm.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }


        }

        private void exportRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderDialog folderDialog = new FolderDialog();
            if (folderDialog.ShowDialog() != DialogResult.OK)
                return;
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                var fileInfo = item.Tag as FileInfo;
                File.WriteAllBytes(Path.Combine(folderDialog.FileName, fileInfo.Name), fileInfo.GetRow());
            }

            MessageBox.Show("Done!");
        }

        private void imageViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmImageViewer().Show();
        }

        private void importfiles_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Make sure that the files names in this list");
            var ofd = new OpenFileDialog();
            ofd.Filter = "All Files|*.*";
            ofd.Multiselect = true;
            ofd.Title = "Select Files";
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            var ImportedFiles = 0;
            foreach(var FilePath in ofd.FileNames)
            {
                var item= listView1.Items.Cast<ListViewItem>().FirstOrDefault(x => x.Text == Path.GetFileName(FilePath));
                if (item == null)
                {
                    MessageBox.Show($"File {Path.GetFileName(FilePath)} not found in list, the file will be ignored");
                    continue;
                }
                var fileInfo = item.Tag as FileInfo;
                fileInfo.IsEdited = true;
                fileInfo.NewFileBytes = File.ReadAllBytes(FilePath);
                Modifiedtrmdtoc.Add(fileInfo.Rmdtoc);
                ImportedFiles++;

                item.Font= new System.Drawing.Font(item.Font, System.Drawing.FontStyle.Bold);
            }

            MessageBox.Show($"Done! {ImportedFiles} files imported");
        }
    }
}
