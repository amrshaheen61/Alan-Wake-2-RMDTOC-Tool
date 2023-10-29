using Helper;
using System.Collections.Generic;
using System.Text;

namespace alan_wake_2_rmdtoc_Tool.Core.StringTable
{

    public class SrtingTableEntry
    {
        public string Name;
        public string Value;
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

                Entry.Name = Stream.GetStringValue(Stream.GetIntValue());
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
                var Bytes = Encoding.ASCII.GetBytes(Entry.Name);
                Stream.SetIntValue(Bytes.Length);
                Stream.SetBytes(Bytes);
                Bytes = Encoding.Unicode.GetBytes(Entry.Value);
                Stream.SetIntValue(Bytes.Length / 2);
                Stream.SetBytes(Bytes);
            }
        }
    }
}
