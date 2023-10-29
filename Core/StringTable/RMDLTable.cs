using Helper;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace alan_wake_2_rmdtoc_Tool.Core.StringTable
{
    public class RMDLTable : List<SrtingTableEntry>, IStringTable
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        class Header
        {
            public int Magic;
            public int Unk;//0x23
            public int unk1;//0x0
            public int unk2;//0x160
            public int TableCount;
            public int unk3;//0x0

        }


        class TableEntry
        {
            public int Type;//??
            public int BlockSize;
            public int Magic;//0x4135DAB6
            public int Null;
            public SrtingTableEntry Srtingtableentry;
            int EndMagic;//0xD34DB33F
            int EndMagic2;//0xD34DB33F


            public void Read(IStream Stream)
            {
                Type = Stream.GetIntValue();
                BlockSize = Stream.GetIntValue();
                Magic = Stream.GetIntValue();
                Null = Stream.GetIntValue();
                Srtingtableentry = new SrtingTableEntry();
                Srtingtableentry.Value = Stream.GetStringValue(Stream.GetIntValue(), System.Text.Encoding.UTF8);
                Srtingtableentry.Name = Stream.GetStringValue(Stream.GetIntValue(), System.Text.Encoding.UTF8);
                EndMagic = Stream.GetIntValue();
                EndMagic2 = Stream.GetIntValue();
            }

            public void Write(IStream Stream)
            {
                int Start = (int)Stream.GetPosition();
                Stream.SetIntValue(Type);
                int BlockSizeOffset = (int)Stream.GetPosition();
                Stream.SetIntValue(BlockSize);

                Stream.SetIntValue(Magic);
                Stream.SetIntValue(Null);



                var bytes = System.Text.Encoding.UTF8.GetBytes(Srtingtableentry.Value);
                Stream.SetIntValue(bytes.Length);
                Stream.SetBytes(bytes);

                bytes = System.Text.Encoding.UTF8.GetBytes(Srtingtableentry.Name);
                Stream.SetIntValue(bytes.Length);
                Stream.SetBytes(bytes);


                Stream.SetIntValue(EndMagic);
                Stream.SetIntValue(EndMagic2);

                //update block size
                BlockSize = (int)Stream.GetPosition() - Start;
                int pos = (int)Stream.GetPosition();
                Stream.SetPosition(BlockSizeOffset);
                Stream.SetIntValue(BlockSize);
                Stream.SetPosition(pos);

            }

        }


        public IStream Stream;

        Header header;
        List<uint> Hashs = new List<uint>();
        int StringTableOffset;
        List<TableEntry> TableEntries = new List<TableEntry>();
        int EndTableOffset;
        byte[] Footer;
        public RMDLTable(IStream Stream)
        {
            this.Stream = Stream;
            ReadStringTable();
        }

        void ReadStringTable()
        {
            header = Stream.Get<Header>();
            if (header.TableCount > 0)
            {
                uint hash = 0;
                while (hash != 0xD34DB33F)
                {
                    hash = Stream.GetUIntValue();
                    Hashs.Add(hash);
                }

            }
            StringTableOffset = (int)Stream.GetPosition();
            for (int i = 0; i < header.TableCount; i++)
            {
                var Entry = new TableEntry();
                Entry.Read(Stream);
                Add(Entry.Srtingtableentry);
                TableEntries.Add(Entry);
            }
            EndTableOffset = (int)Stream.GetPosition();
            Footer = Stream.GetBytes((int)Stream.GetSize() - EndTableOffset);
        }

        public void BuildStringTable()
        {
            header.TableCount = Count;
            Stream.SetSize(0);
            Stream.SetPosition(0);
            Stream.SetStructureValus(header);

            foreach (var hash in Hashs)
            {
                Stream.SetUIntValue(hash);
            }

            foreach (var entry in TableEntries)
            {
                entry.Write(Stream);
            }
            Stream.SetBytes(Footer);
        }
    }
}
