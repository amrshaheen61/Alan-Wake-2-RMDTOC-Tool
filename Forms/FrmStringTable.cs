using alan_wake_2_rmdtoc_Tool.Core.StringTable;
using Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace alan_wake_2_rmdtoc_Tool.Forms
{
    public partial class FrmStringTable : Form
    {
        bool IsStreamFile = false;
        public IStream Stream;
        public IStringTable stringtable;
        public FrmStringTable()
        {
            InitializeComponent();
        }

        public FrmStringTable(IStream stream)
        {

            InitializeComponent();
            Stream = stream;
            IsStreamFile = false;
            openToolStripMenuItem.Visible = false;
            Load();
        }


        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "StringTable Files|*.bin";
            openFileDialog.Title = "Select StringTable File";
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            Stream = new MStream(openFileDialog.FileName);
            IsStreamFile = true;


            Load();
        }

        private void Load()
        {
            if (Stream.GetUIntValue(false) == 0x4C444D52)
            {
                stringtable = new RMDLTable(Stream);
            }
            else
            {
                stringtable = new StringTable(Stream);
            }

            PrintItems();

            toolToolStripMenuItem.Enabled = true;
            saveToolStripMenuItem.Enabled = true;
        }

        private void PrintItems()
        {
            dataGridView1.Rows.Clear();
            var rows = new List<DataGridViewRow>();
            foreach (var Table in stringtable as List<SrtingTableEntry>)
            {

                DataGridViewRow Row = new DataGridViewRow();
                Row.Tag = Table;
                Row.Cells.Add(new DataGridViewTextBoxCell() { Value = Table.Name });
                Row.Cells.Add(new DataGridViewTextBoxCell() { Value = Table.Value });
                rows.Add(Row);
            }
            dataGridView1.Rows.AddRange(rows.ToArray());
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

            var table = dataGridView1.Rows[e.RowIndex].Tag as SrtingTableEntry;
            table.Name = dataGridView1.Rows[e.RowIndex].Cells["TableName"].Value.ToString();
            table.Value = dataGridView1.Rows[e.RowIndex].Cells["TableValue"].Value.ToString();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsStreamFile)
            {
                var SFD = new SaveFileDialog();
                SFD.Filter = "StringTable Files|*.bin";
                SFD.Title = "Select StringTable File";
                SFD.FileName = Path.GetFileName(Stream.Name) + ".new";
                if (SFD.ShowDialog() != DialogResult.OK)
                    return;
                stringtable.BuildStringTable();
                Stream.WriteFile(SFD.FileName);
                return;
            }
            else
            {
                stringtable.BuildStringTable();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }


        }

        private void exportAllToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void importAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void namesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sdf = new SaveFileDialog();
            sdf.Filter = "Text Files|*.txt";
            sdf.Title = "Save Text File";
            sdf.FileName = Path.GetFileName(Stream.Name) + ".txt";
            if (sdf.ShowDialog() != DialogResult.OK)
                return;

            var sb = new System.Text.StringBuilder();
            foreach (var Table in stringtable as List<SrtingTableEntry>)
            {
                sb.AppendLine(Table.Name);
            }

            File.WriteAllText(sdf.FileName, sb.ToString());

            MessageBox.Show("Done!");
        }

        private void valuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sdf = new SaveFileDialog();
            sdf.Filter = "Text Files|*.txt";
            sdf.Title = "Save Text File";
            sdf.FileName = Path.GetFileName(Stream.Name) + ".txt";
            if (sdf.ShowDialog() != DialogResult.OK)
                return;

            var sb = new System.Text.StringBuilder();
            foreach (var Table in stringtable as List<SrtingTableEntry>)
            {
                sb.AppendLine(Table.Value);
            }

            File.WriteAllText(sdf.FileName, sb.ToString());

            MessageBox.Show("Done!");
        }

        private void namesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Text Files|*.txt";
            ofd.Title = "Select Text File";
            ofd.FileName = Path.GetFileName(Stream.Name) + ".txt";
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            var lines = File.ReadAllLines(ofd.FileName);
            int i = 0;
            foreach (var line in lines)
            {
                if (i >= dataGridView1.Rows.Count) break;
                var Table = dataGridView1.Rows[i++];
                Table.Cells["TableName"].Value = line;
            }

            MessageBox.Show("Done!");
        }

        private void valuesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Text Files|*.txt";
            ofd.Title = "Select Text File";
            ofd.FileName = Path.GetFileName(Stream.Name) + ".txt";
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            var lines = File.ReadAllLines(ofd.FileName);
            int i = 0;
            foreach (var line in lines)
            {
                if (i >= dataGridView1.Rows.Count) break;

                var Table = dataGridView1.Rows[i++];
                Table.Cells["TableValue"].Value = line;

            }

            MessageBox.Show("Done!");
        }

        private void bothToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Text Files|*.txt";
            ofd.Title = "Select Text File";
            ofd.FileName = Path.GetFileName(Stream.Name) + ".txt";
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            var lines = File.ReadAllLines(ofd.FileName);
            int i = 0;
            foreach (var line in lines)
            {
                if (i >= dataGridView1.Rows.Count) break;
                var split = line.Split(new[] { '=' }, 2);
                if (split.Length != 2)
                    continue;

                var name = split[0];
                var value = split[1];

                var Table = dataGridView1.Rows[i++];
                Table.Cells["TableName"].Value = name;
                Table.Cells["TableValue"].Value = value;
            }

            MessageBox.Show("Done!");
        }

        private void bothToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var sdf = new SaveFileDialog();
            sdf.Filter = "Text Files|*.txt";
            sdf.Title = "Save Text File";
            sdf.FileName = Path.GetFileName(Stream.Name) + ".txt";
            if (sdf.ShowDialog() != DialogResult.OK)
                return;

            var sb = new System.Text.StringBuilder();
            foreach (var Table in stringtable as List<SrtingTableEntry>)
            {
                sb.AppendLine(Table.Name + "=" + Table.Value);
            }
            File.WriteAllText(sdf.FileName, sb.ToString());
        }

        private void FrmStringTable_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F)
            {
                searchBox1.Visible = true;
            }
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            searchBox1.Visible = true;
        }
    }
}
