using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Controls
{
    public partial class NTreeView : TreeView
    {

        [DllImport("uxtheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr h, string subAppName, string subIdList);
        protected override void CreateHandle()
        {
            base.CreateHandle();
            SetWindowTheme(this.Handle, "explorer", null);
        }




        public event EventHandler BeforeClear;

        public void Clear()
        {
            OnBeforeClear(EventArgs.Empty);
            base.Nodes.Clear();
        }

        protected virtual void OnBeforeClear(EventArgs e)
        {
            BeforeClear?.Invoke(this, EventArgs.Empty);
        }
    }
}
