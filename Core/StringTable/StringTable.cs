using Helper;
using System.Collections.Generic;
using System.Text;

namespace alan_wake_2_rmdtoc_Tool.Core.StringTable
{

    public class SrtingTableEntry
    {
        private string _Name;
        private string _Value;
        public string Name
        {
            get
            {
                return ReplaceBreaklines(_Name);
            }
            set
            {
                _Name = ReplaceBreaklines(value, true);
            }
        }
        public string Value
        {
            get { return ReplaceBreaklines(_Value); }
            set { _Value = ReplaceBreaklines(value, true); }
        }

        private static string ReplaceBreaklines(string StringValue, bool Back = false)
        {
            if (!Back)
            {
                StringValue = StringValue.Replace("\r\n", "<cf>");
                StringValue = StringValue.Replace("\r", "<cr>");
                StringValue = StringValue.Replace("\n", "<lf>");
            }
            else
            {
                StringValue = StringValue.Replace("<cf>", "\r\n");
                StringValue = StringValue.Replace("<cr>", "\r");
                StringValue = StringValue.Replace("<lf>", "\n");
            }

            return StringValue;
        }
    }


    public class StringTable : List<SrtingTableEntry>, IStringTable
    {
        public IStream Stream;
        public StringTable(IStream Stream)
        {
            this.Stream = Stream;
            ReadStringTable();
        }

        void ReadStringTable()
        {
            var Count = Stream.GetIntValue();
            for (int i = 0; i < Count; i++)
            {
                var Entry = new SrtingTableEntry();

                Entry.Name = Stream.GetStringValue(Stream.GetIntValue(), Encoding.UTF8);
                Entry.Value = Stream.GetStringValue(Stream.GetIntValue() * 2, Encoding.Unicode);
                Add(Entry);
            }
        }


        public void BuildStringTable()
        {
            Stream.SetSize(0);
            Stream.SetPosition(0);
            Stream.SetIntValue(Count);
            for (int i = 0; i < Count; i++)
            {
                var Entry = this[i];
                var Bytes = Encoding.UTF8.GetBytes(Entry.Name);
                Stream.SetIntValue(Bytes.Length);
                Stream.SetBytes(Bytes);
                Bytes = Encoding.Unicode.GetBytes(Entry.Value);
                Stream.SetIntValue(Bytes.Length / 2);
                Stream.SetBytes(Bytes);
            }
        }
    }
}
