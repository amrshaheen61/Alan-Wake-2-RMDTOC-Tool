using Helper;
using System;
using System.IO;
using System.Windows.Forms;

namespace alan_wake_2_rmdtoc_Tool.Forms
{
    public partial class frmImageViewer : Form
    {
        bool IsStreamFile = false;
        public IStream Stream;
        public frmImageViewer()
        {
            InitializeComponent();
        }

        public frmImageViewer(IStream stream)
        {

            InitializeComponent();
            Stream = stream;
            IsStreamFile = false;
            openToolStripMenuItem.Visible = false;
            PrintImage();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "All Images|*.tex;*.dds;*.png;*.jpg;*.bmp;*.tga";
            ofd.Title = "Select Image";
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            Stream = FStream.Open(ofd.FileName, FileMode.Open, FileAccess.Read);
            PrintImage();
        }

        private void PrintImage()
        {
            if (Stream.GetIntValue(false) == 0x20534444)
            {
                pictureBox1.Image = DDSToBitmap.Convert(Stream);
            }
            else if (Stream.Name.EndsWith(".png", StringComparison.InvariantCulture) ||
                     Stream.Name.EndsWith(".jpg", StringComparison.InvariantCulture) ||
                     Stream.Name.EndsWith(".bmp", StringComparison.InvariantCulture) ||
                     Stream.Name.EndsWith(".tga", StringComparison.InvariantCulture))
            {
                pictureBox1.Image = System.Drawing.Image.FromStream((Stream)Stream);
            }
            else
            {
                MessageBox.Show("Not supported file!");
            }
        }
    }
}
