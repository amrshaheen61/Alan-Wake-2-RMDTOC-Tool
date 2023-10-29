/*
# Alan Wake 2
# RMDTOC/RMDBLOB extract
# QuickBMS script by DKDave, 2023
*/

using Helper;
using LZ4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace alan_wake_2_rmdtoc_Tool
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class rmdtocHeader
    {
        public int Magic;
        public int unko; //=2
        public int CompressionInfoTableOffset;
        public int CompressionInfoTableSize;

        public int Unko2;//=0
        public int Unko3;//=1

        public int Table_1_Offset;//Table size is Table_1_Count* 0x1C
        public int Table_1_Count;

        public int Table_2_Offset;//Table size is Table_1_Count* 0x20
        public int Table_2_Count;

        public int Names_Offset;
        public int Names_Size;

        public int Table_3_Offset;//Table size is Table_1_Count* 0x8
        public int Table_3_Count;

        public int Table_4_Offset;//0x48
        public int Table_4_Size;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public int[] Unko4;

        public int Table_5_Offset;//0x16
        public int Table_5_Size;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class CompressInfo
    {
        public byte unko;
        public byte FileIndex;
        public byte unko2;
        public int BufferOffset;
        public byte unko3;
        public int BufferUSize;
        public int BufferSize;
    }


    public class FolderInfo
    {
        const int Size = 0x1c;

        public int FolderIndex;
        public int unko;
        public int unko2;
        public int FilesIndex;
        public int FilesCount;
        public int NameOffset;
        public int NameLength;
        public List<FileInfo> Files;
        public rmdtoc Rmdtoc;


        public string Name;

        public void Read(IStream stream)
        {
            FolderIndex = stream.Get<int>();
            unko = stream.Get<int>();
            unko2 = stream.Get<int>();
            FilesIndex = stream.Get<int>();
            FilesCount = stream.Get<int>();
            NameOffset = stream.Get<int>();
            NameLength = stream.Get<int>();
        }

        public void Write(IStream stream)
        {
            stream.SetIntValue(FolderIndex);
            stream.SetIntValue(unko);
            stream.SetIntValue(unko2);
            stream.SetIntValue(FilesIndex);
            stream.SetIntValue(FilesCount);
            stream.SetIntValue(NameOffset);
            stream.SetIntValue(NameLength);
        }

    }

    public class FileInfo
    {
        const int Size = 0x20;
        public int CompressionInfoTableOffset;
        public int CompressionInfoTableSize;
        public int FileId;//??
        public int NameOffset;
        public int NameLength;
        public int DecompressSize;
        public int DMKPTableOffset;
        public int DMKPTableSize;
        public byte[] DMKPBlock;


        public rmdtoc Rmdtoc;
        public CompressInfo[] CompressInfos;

        public string Name;

        public bool IsEdited = false;

        public byte[] NewFileBytes;




        public void Read(IStream stream)
        {
            CompressionInfoTableOffset = stream.Get<int>();
            CompressionInfoTableSize = stream.Get<int>();
            FileId = stream.Get<int>();
            NameOffset = stream.Get<int>();
            NameLength = stream.Get<int>();
            DecompressSize = stream.Get<int>();
            DMKPTableOffset = stream.Get<int>();
            DMKPTableSize = stream.Get<int>();
        }
        public void Write(IStream stream)
        {
            stream.SetIntValue(CompressionInfoTableOffset);
            stream.SetIntValue(CompressionInfoTableSize);
            stream.SetIntValue(FileId);
            stream.SetIntValue(NameOffset);
            stream.SetIntValue(NameLength);
            stream.SetIntValue(DecompressSize);
            stream.SetIntValue(DMKPTableOffset);
            stream.SetIntValue(DMKPTableSize);
        }

        public string GetOffset()
        {
            int offset = 0;
            foreach (var item in CompressInfos)
            {
                offset += item.BufferOffset;
                break;
            }
            return "0x" + offset.ToString("X");
        }

        public string GetSize()
        {
            int size = 0;
            foreach (var item in CompressInfos)
            {
                if (item.BufferSize != 0)
                    size += item.BufferSize;
                else
                    size += item.BufferUSize;
            }
            return "0x" + size.ToString("X");
        }

        public byte[] GetFile()
        {
            var DecompressBuffer = new MStream();

            foreach (var info in CompressInfos)
            {
                string path = Path.ChangeExtension(Rmdtoc.rmdtocStream.Name, null) + "-" + info.FileIndex.ToString("D3") + ".rmdblob";
                Console.WriteLine(path);
                var stream = new FStream(path, FileMode.Open, FileAccess.Read);


                if (info.BufferSize == 0)
                {
                    DecompressBuffer.SetBytes(stream.GetBytes(info.BufferUSize, false, info.BufferOffset));
                }
                else
                {
                    DecompressBuffer.SetBytes(LZ4Codec.Decode(stream.GetBytes(info.BufferSize, false, info.BufferOffset), 0, info.BufferSize, info.BufferUSize));
                }
                stream.Close();
            }


            return DecompressBuffer.ToArray();
        }

        public string GetId()
        {
            if (CompressInfos.Length == 0)
            {
                return "";
            }


            string path = Path.ChangeExtension(Rmdtoc.rmdtocStream.Name, null) + "-" + CompressInfos[0].FileIndex.ToString("D3") + ".rmdblob";
            Console.WriteLine(path);
            var stream = new FStream(path, FileMode.Open, FileAccess.Read);

            string id;
            if (CompressInfos[0].BufferSize == 0)
            {
                id = stream.GetStringValue(4, false, CompressInfos[0].BufferOffset);
            }
            else
            {
                id = stream.GetStringValue(4, false, CompressInfos[0].BufferOffset + 1);
            }


            stream.Close();

            return id;
        }


        public void SetFile()
        {
            var info = CompressInfos[0];
            string path = Path.ChangeExtension(Rmdtoc.rmdtocStream.Name, null) + "-" + info.FileIndex.ToString("D3") + ".rmdblob";
            Console.WriteLine(path);
            var stream = new FStream(path, FileMode.Open, FileAccess.ReadWrite);

            info.BufferUSize = NewFileBytes.Length;
            var CompressedBytes = LZ4Codec.Encode(NewFileBytes, 0, info.BufferUSize);
            info.BufferSize = CompressedBytes.Length;

            stream.Seek(0, SeekOrigin.End);
            info.BufferOffset = (int)stream.Position;
            stream.SetBytes(CompressedBytes);

            CompressInfos = new CompressInfo[1];
            CompressInfos[0] = info;
            CompressionInfoTableSize = Marshal.SizeOf<CompressInfo>();
            DecompressSize = NewFileBytes.Length;
            stream.Close();

            IsEdited = false;
            NewFileBytes = null;
        }


    }




    public class rmdtoc : IDisposable
    {

        public IStream rmdtocStream;
        public rmdtocHeader header;




        public rmdtoc(string rmdtocPath)
        {
            rmdtocStream = new FStream(rmdtocPath, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite);


            Load();
        }

        public IStream BufferStream;
        public IStream Table1;
        public IStream Table2;
        public IStream Table3;
        public IStream Table4;
        public IStream Table5;
        public IStream NameTable;
        public List<FileInfo> Files;
        public List<FolderInfo> Folders;
        public TreeNode Root;
        void Load()
        {
            header = rmdtocStream.Get<rmdtocHeader>();


            BufferStream = new MStream(GetBuffer(rmdtocStream, header.CompressionInfoTableOffset, header.CompressionInfoTableSize));
            Table1 = new MStream(BufferStream.GetBytes(header.Table_1_Count * 0x1C, SeekToOffset: header.Table_1_Offset));
            Table2 = new MStream(BufferStream.GetBytes(header.Table_2_Count * 0x20, SeekToOffset: header.Table_2_Offset));
            Table3 = new MStream(BufferStream.GetBytes(header.Table_3_Count * 0x8, SeekToOffset: header.Table_3_Offset));
            Table4 = new MStream(BufferStream.GetBytes(header.Table_4_Size, SeekToOffset: header.Table_4_Offset));
            Table5 = new MStream(BufferStream.GetBytes(header.Table_5_Size, SeekToOffset: header.Table_5_Offset));



            NameTable = new MStream(BufferStream.GetBytes(header.Names_Size, SeekToOffset: header.Names_Offset));


            Files = new List<FileInfo>();
            for (int i = 0; i < header.Table_2_Count; i++)
            {
                var file = new FileInfo();
                file.Read(Table2);
                file.Rmdtoc = this;
                file.DMKPBlock = Table4.GetBytes(file.DMKPTableSize, false, file.DMKPTableOffset);
                file.CompressInfos = Table5.GetArray<CompressInfo>(file.CompressionInfoTableSize / Marshal.SizeOf<CompressInfo>(), false, file.CompressionInfoTableOffset);
                file.Name = GetName(file.NameOffset, file.NameLength);
                Files.Add(file);
            }

            Folders = new List<FolderInfo>();
            for (int i = 0; i < header.Table_1_Count; i++)
            {
                var folder = new FolderInfo();
                folder.Read(Table1);
                folder.Rmdtoc = this;
                folder.Files = Files.GetRange(folder.FilesIndex, folder.FilesCount);
                folder.Name = GetName(folder.NameOffset, folder.NameLength);

                //Console.WriteLine(folder.Name);
                Folders.Add(folder);

            }




            Root = new TreeNode(Path.GetFileNameWithoutExtension(Folders[0].Name));
            Root.Tag = Folders[0];
            var Temp = Root;
            MakeNode(Folders, Root, Folders[0]);
            Root = Temp;

        }

        private void MakeNode(List<FolderInfo> folders, TreeNode node, FolderInfo Folder)
        {
            for (int i = Folder.unko; i < Folder.unko + Folder.unko2; i++)
            {
                Root = new TreeNode(folders[i].Name);
                Root.Tag = folders[i];

                node.Nodes.Add(Root);
                MakeNode(folders, Root, folders[i]);


            }
        }

        public byte[] GetBuffer(IStream stream, long offset, int BlockSize)
        {
            var DecompressBuffer = new MStream();

            var StoreOffset = stream.Position;
            stream.Position = offset;
            for (int i = 0; i < BlockSize / Marshal.SizeOf<CompressInfo>(); i++)
            {
                var info = stream.Get<CompressInfo>();

                DecompressBuffer.SetBytes(LZ4Codec.Decode(stream.GetBytes(info.BufferSize, false, info.BufferOffset), 0, info.BufferSize, info.BufferUSize));
            }

            stream.Position = StoreOffset;

            return DecompressBuffer.ToArray();
        }


        public string GetName(int offset, int length)
        {
            return NameTable.GetStringValue(length, SeekToOffset: offset);
        }


        private CompressInfo[] CompressFile(byte[] bytes, out byte[] CompressedBytes)
        {
            var info = new List<CompressInfo>();
            var compressinfo = new CompressInfo();
            compressinfo.unko = 0x10;
            compressinfo.BufferUSize = bytes.Length;
            CompressedBytes = LZ4Codec.Encode(bytes, 0, compressinfo.BufferUSize);
            compressinfo.BufferSize = CompressedBytes.Length;
            info.Add(compressinfo);
            return info.ToArray();
        }


        public void Save()
        {

            Table5 = new MStream();
            foreach (var file in Files)
            {
                if (file.IsEdited)
                {
                    file.SetFile();
                }
                file.CompressionInfoTableOffset = (int)Table5.Position;
                foreach (var info in file.CompressInfos)
                {
                    Table5.SetStructureValus(info);
                }
            }

            Table2 = new MStream();
            foreach (var file in Files)
            {
                file.Write(Table2);
            }

            BufferStream.SetSize(header.Table_1_Offset);
            BufferStream.SetPosition(header.Table_1_Offset);
            BufferStream.SetBytes(Table1.ToArray());

            header.Table_2_Offset = (int)BufferStream.Position;
            BufferStream.SetBytes(Table2.ToArray());

            header.Names_Offset = (int)BufferStream.Position;
            BufferStream.SetBytes(NameTable.ToArray());

            header.Table_3_Offset = (int)BufferStream.Position;
            BufferStream.SetBytes(Table3.ToArray());

            header.Table_4_Offset = (int)BufferStream.Position;
            BufferStream.SetBytes(Table4.ToArray());

            header.Table_5_Offset = (int)BufferStream.Position;
            BufferStream.SetBytes(Table5.ToArray());


            rmdtocStream.SetSize(4096);
            rmdtocStream.SetPosition(4096);


            var infoTable = CompressFile(BufferStream.ToArray(), out byte[] CompressedBytes);

            rmdtocStream.SetBytes(CompressedBytes);

            rmdtocStream.Seek(0);
            rmdtocStream.SetBytes(new byte[4096], false);

            header.CompressionInfoTableSize = infoTable.Length * Marshal.SizeOf<CompressInfo>();

            rmdtocStream.SetStructureValus(header);

            int offset = 4096;
            for (int i = 0; i < infoTable.Length; i++)
            {
                infoTable[i].BufferOffset = offset;
                rmdtocStream.SetStructureValus(infoTable[i]);
                offset += infoTable[i].BufferSize;
            }

        }

        public void Dispose()
        {
            if (rmdtocStream != null)
            {
                rmdtocStream.Close();
                rmdtocStream.Dispose();

                GC.SuppressFinalize(this);
            }
        }

        ~rmdtoc()
        {
            Dispose();
        }
    }
}
