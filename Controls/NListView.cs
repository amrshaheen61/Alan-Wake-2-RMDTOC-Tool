using System;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace Controls
{

    public partial class NListView : ListView
    {

        [DllImport("uxtheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr h, string subAppName, string subIdList);


        protected override void CreateHandle()
        {
            base.CreateHandle();
            SetWindowTheme(this.Handle, "explorer", null);
        }


        private const int LVM_INSERTITEM = LVM_FIRST + 77;
        private const int LVM_DELETEITEM = LVM_FIRST + 8;
        private const int LVM_DELETEALLITEMS = LVM_FIRST + 9;
        public event EventHandler ItemsCountChanged;
        protected virtual void OnItemsCountChanged(EventArgs e)
        {
            if (ItemsCountChanged != null)
                ItemsCountChanged(this, e);
        }
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            switch (m.Msg)
            {
                case LVM_INSERTITEM:
                case LVM_DELETEITEM:
                case LVM_DELETEALLITEMS:
                    OnItemsCountChanged(EventArgs.Empty);
                    break;
            }

        }
        public void SelectAll()
        {
            foreach (ListViewItem item in Items)
            {
                item.Selected = true;
            }
        }

        public void OnLoeded()
        {
            ListViewItemSorter = m_lstColumnSorter;
        }


        ColumnSorter m_lstColumnSorter = new ColumnSorter();

        public NListView()
        {
            ListViewItemSorter = m_lstColumnSorter;
        }

        protected override void OnColumnClick(ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == m_lstColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (m_lstColumnSorter.Order == SortOrder.Ascending)
                {
                    m_lstColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    m_lstColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                m_lstColumnSorter.SortColumn = e.Column;
                m_lstColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            Sort();
            SetSortIcon(m_lstColumnSorter.SortColumn, m_lstColumnSorter.Order);
        }





        [StructLayout(LayoutKind.Sequential)]
        public struct LVCOLUMN
        {
            public Int32 mask;
            public Int32 cx;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszText;
            public IntPtr hbm;
            public Int32 cchTextMax;
            public Int32 fmt;
            public Int32 iSubItem;
            public Int32 iImage;
            public Int32 iOrder;
        }

        const Int32 HDI_WIDTH = 0x0001;
        const Int32 HDI_HEIGHT = HDI_WIDTH;
        const Int32 HDI_TEXT = 0x0002;
        const Int32 HDI_FORMAT = 0x0004;
        const Int32 HDI_LPARAM = 0x0008;
        const Int32 HDI_BITMAP = 0x0010;
        const Int32 HDI_IMAGE = 0x0020;
        const Int32 HDI_DI_SETITEM = 0x0040;
        const Int32 HDI_ORDER = 0x0080;
        const Int32 HDI_FILTER = 0x0100;

        const Int32 HDF_LEFT = 0x0000;
        const Int32 HDF_RIGHT = 0x0001;
        const Int32 HDF_CENTER = 0x0002;
        const Int32 HDF_JUSTIFYMASK = 0x0003;
        const Int32 HDF_RTLREADING = 0x0004;
        const Int32 HDF_OWNERDRAW = 0x8000;
        const Int32 HDF_STRING = 0x4000;
        const Int32 HDF_BITMAP = 0x2000;
        const Int32 HDF_BITMAP_ON_RIGHT = 0x1000;
        const Int32 HDF_IMAGE = 0x0800;
        const Int32 HDF_SORTUP = 0x0400;
        const Int32 HDF_SORTDOWN = 0x0200;

        const Int32 LVM_FIRST = 0x1000;         // List messages
        const Int32 LVM_GETHEADER = LVM_FIRST + 31;
        const Int32 HDM_FIRST = 0x1200;         // Header messages
        const Int32 HDM_SETIMAGELIST = HDM_FIRST + 8;
        const Int32 HDM_GETIMAGELIST = HDM_FIRST + 9;
        const Int32 HDM_GETITEM = HDM_FIRST + 11;
        const Int32 HDM_SETITEM = HDM_FIRST + 12;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern IntPtr SendMessageLVCOLUMN(IntPtr hWnd, Int32 Msg, IntPtr wParam, ref LVCOLUMN lPLVCOLUMN);


        //This method used to set arrow icon
        public void SetSortIcon(int columnIndex, SortOrder order)
        {
            IntPtr columnHeader = SendMessage(this.Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);

            for (int columnNumber = 0; columnNumber <= Columns.Count - 1; columnNumber++)
            {
                IntPtr columnPtr = new IntPtr(columnNumber);
                LVCOLUMN lvColumn = new LVCOLUMN();
                lvColumn.mask = HDI_FORMAT;

                SendMessageLVCOLUMN(columnHeader, HDM_GETITEM, columnPtr, ref lvColumn);

                if (!(order == SortOrder.None) && columnNumber == columnIndex)
                {
                    switch (order)
                    {
                        case System.Windows.Forms.SortOrder.Ascending:
                            lvColumn.fmt &= ~HDF_SORTDOWN;
                            lvColumn.fmt |= HDF_SORTUP;
                            break;
                        case System.Windows.Forms.SortOrder.Descending:
                            lvColumn.fmt &= ~HDF_SORTUP;
                            lvColumn.fmt |= HDF_SORTDOWN;
                            break;
                    }
                    lvColumn.fmt |= (HDF_LEFT | HDF_BITMAP_ON_RIGHT);
                }
                else
                {
                    lvColumn.fmt &= ~HDF_SORTDOWN & ~HDF_SORTUP & ~HDF_BITMAP_ON_RIGHT;
                }

                SendMessageLVCOLUMN(columnHeader, HDM_SETITEM, columnPtr, ref lvColumn);
            }
        }

    }


    public class ColumnSorter : IComparer
    {
        private int sortColumn;

        public int SortColumn
        {
            set { sortColumn = value; }
            get { return sortColumn; }
        }

        private SortOrder sortOrder;

        public SortOrder Order
        {
            set { sortOrder = value; }
            get { return sortOrder; }
        }

        private Comparer listViewItemComparer;

        public ColumnSorter()
        {
            sortColumn = 0;

            sortOrder = SortOrder.None;

            listViewItemComparer = new Comparer(CultureInfo.CurrentUICulture);
        }

        public int Compare(object x, object y)
        {
            try
            {
                ListViewItem lviX = (ListViewItem)x;
                ListViewItem lviY = (ListViewItem)y;

                int compareResult = 0;

                if (lviX.SubItems[sortColumn].Tag != null && lviY.SubItems[sortColumn].Tag != null)
                {
                    compareResult = listViewItemComparer.Compare(lviX.SubItems[sortColumn].Tag, lviY.SubItems[sortColumn].Tag);
                }
                else
                {
                    compareResult = listViewItemComparer.Compare(lviX.SubItems[sortColumn].Text, lviY.SubItems[sortColumn].Text);
                }

                if (sortOrder == SortOrder.Ascending)
                {
                    return compareResult;
                }
                else if (sortOrder == SortOrder.Descending)
                {
                    return (-compareResult);
                }
                else
                {
                    return 0;
                }

            }
            catch
            {
                return 0;
            }
        }
    }

}
