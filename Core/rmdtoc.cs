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

        public int RMDBLOB_Path_Offset;
        public int RMDBLOB_Path_Count;

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


    public class RMDBLOBPath
    {
        public const int Size = 0x10;
        public int NameOffset;
        public int NameLength;
        public ulong Hash;
        public string Path;

        public void Read(IStream stream)
        {
            NameOffset = stream.Get<int>();
            NameLength = stream.Get<int>();
            Hash = stream.Get<ulong>();
        }

        public void Write(IStream stream)
        {
            stream.SetIntValue(NameOffset);
            stream.SetIntValue(NameLength);
            stream.SetUInt64Value(Hash);
        }
    }

    public class CompressInfo
    {
        public const int Size = 0x10;
        public byte isCompressed;
        public byte FileIndex;
        public byte unko2;
        public uint _bufferoffset;
        public byte unko3;
        public int BufferUSize;
        public int BufferSize;

        public long BufferOffset
        {
            get
            {
                long value = _bufferoffset;
                return value + (unko3 * 0x100000000);
            }
            set
            {
                long newUnko3 = value / 0x100000000;
                unko3 = (byte)newUnko3;
                _bufferoffset = (uint)(value - (newUnko3 * 0x100000000));
            }
        }

        public void Read(IStream stream)
        {
            isCompressed = stream.Get<byte>();
            FileIndex = stream.Get<byte>();
            unko2 = stream.Get<byte>();
            _bufferoffset = stream.Get<uint>();
            unko3 = stream.Get<byte>();
            BufferUSize = stream.Get<int>();
            BufferSize = stream.Get<int>();
        }

        public void Write(IStream stream)
        {
            stream.SetByteValue(isCompressed);
            stream.SetByteValue(FileIndex);
            stream.SetByteValue(unko2);
            stream.SetUIntValue(_bufferoffset);
            stream.SetByteValue(unko3);
            stream.SetIntValue(BufferUSize);
            stream.SetIntValue(BufferSize);
        }

    }


    public class FolderInfo
    {
        const int Size = 0x1c;

        public int TreeLevel;
        public int FolderIndex;
        public int FolderCount;
        public int FilesIndex;
        public int FilesCount;
        public int NameOffset;
        public int NameLength;
        public List<FileInfo> Files;
        public rmdtoc Rmdtoc;


        public string Name;

        public void Read(IStream stream)
        {
            TreeLevel = stream.Get<int>();
            FolderIndex = stream.Get<int>();
            FolderCount = stream.Get<int>();
            FilesIndex = stream.Get<int>();
            FilesCount = stream.Get<int>();
            NameOffset = stream.Get<int>();
            NameLength = stream.Get<int>();
        }

        public void Write(IStream stream)
        {
            stream.SetIntValue(TreeLevel);
            stream.SetIntValue(FolderIndex);
            stream.SetIntValue(FolderCount);
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
        public List<CompressInfo> CompressInfos;

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

            long offset = 0;
            foreach (var item in CompressInfos)
            {
                offset += item.BufferOffset;
                break;
            }

            Console.WriteLine(offset);
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

                var stream = new FStream(GetRMDBLOBPath(info), FileMode.Open, FileAccess.Read);
                stream.Position = info.BufferOffset;
                if (info.BufferSize == 0 || info.isCompressed == 0)
                {
                    DecompressBuffer.SetBytes(stream.GetBytes(info.BufferUSize));
                }
                else
                {
                    DecompressBuffer.SetBytes(LZ4Codec.Decode(stream.GetBytes(info.BufferSize), 0, info.BufferSize, info.BufferUSize));
                }
                stream.Close();
            }


            return DecompressBuffer.ToArray();
        }


        public string GetRMDBLOBPath(CompressInfo info)
        {
            return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Rmdtoc.rmdtocStream.Name), Rmdtoc.RMDBLOBPaths[info.FileIndex].Path));
        }

        public string GetRMDBLOBPath()
        {
            if (CompressInfos.Count == 0)
            {
                return "";
            }

            return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Rmdtoc.rmdtocStream.Name), Rmdtoc.RMDBLOBPaths[CompressInfos[0].FileIndex].Path));
        }
        public byte[] GetRow()
        {

            var DecompressBuffer = new MStream();

            foreach (var info in CompressInfos)
            {

                var stream = new FStream(GetRMDBLOBPath(info), FileMode.Open, FileAccess.Read);

                stream.Position = info.BufferOffset;
                DecompressBuffer.SetStringValue("Offset: " + stream.Position);
                DecompressBuffer.SetStringValue("Size: " + info.BufferSize);
                DecompressBuffer.SetStringValue("USize: " + info.BufferUSize);
                DecompressBuffer.SetStringValue("IsCompressed: " + info.isCompressed);
                DecompressBuffer.SetStringValue("Start");

                if (info.BufferSize == 0 || info.isCompressed == 0)
                {
                    DecompressBuffer.SetBytes(stream.GetBytes(info.BufferUSize));
                }
                else
                {
                    DecompressBuffer.SetBytes(stream.GetBytes(info.BufferSize));
                }
                DecompressBuffer.SetStringValue("End of File");

                stream.Close();
            }


            return DecompressBuffer.ToArray();



        }
        public string GetId()
        {
            if (CompressInfos.Count == 0)
            {
                return "";
            }

            var stream = new FStream(GetRMDBLOBPath(CompressInfos[0]), FileMode.Open, FileAccess.Read);
            string id;
            if (CompressInfos[0].BufferSize == 0)
            {
                stream.Position = CompressInfos[0].BufferOffset;
                id = stream.GetStringValue(4);
            }
            else
            {
                stream.Position = CompressInfos[0].BufferOffset + 1;
                id = stream.GetStringValue(4);
            }


            stream.Close();

            return id;
        }


        public void SetFile()
        {
            var info = CompressInfos[0];
            var stream = new FStream(GetRMDBLOBPath(info), FileMode.Open, FileAccess.ReadWrite);

            byte[] CompressedBytes;

            if (info.isCompressed != 0)
            {
                info.BufferUSize = NewFileBytes.Length;
                CompressedBytes = LZ4Codec.Encode(NewFileBytes, 0, info.BufferUSize);
                info.BufferSize = CompressedBytes.Length;
            }
            else
            {
                info.BufferUSize = NewFileBytes.Length;
                CompressedBytes = NewFileBytes;
                info.BufferSize = 0;
            }

            /////
            stream.Seek(0, SeekOrigin.End);
            info.BufferOffset = (int)stream.Position;
            stream.SetBytes(CompressedBytes);
            ////////

            CompressInfos.Clear();
            CompressInfos.Add(info);
            ///
            CompressionInfoTableSize = CompressInfo.Size * CompressInfos.Count;
            DecompressSize = info.BufferUSize;
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
        public IStream RMDBLOBPathBlock;
        public IStream Table1;
        public IStream Table2;
        public IStream Table3;
        public IStream Table4;
        public IStream Table5;
        public IStream NameTable;
        public List<FileInfo> Files;
        public List<FolderInfo> Folders;
        public List<RMDBLOBPath> RMDBLOBPaths;
        public TreeNode Root;
        void Load()
        {
            header = rmdtocStream.Get<rmdtocHeader>();

            BufferStream = new MStream(GetBuffer(rmdtocStream, header.CompressionInfoTableOffset, header.CompressionInfoTableSize));
            RMDBLOBPathBlock = new MStream(BufferStream.GetBytes(header.RMDBLOB_Path_Count * RMDBLOBPath.Size, SeekToOffset: header.RMDBLOB_Path_Offset));
            Table1 = new MStream(BufferStream.GetBytes(header.Table_1_Count * 0x1C, SeekToOffset: header.Table_1_Offset));
            Table2 = new MStream(BufferStream.GetBytes(header.Table_2_Count * 0x20, SeekToOffset: header.Table_2_Offset));
            Table3 = new MStream(BufferStream.GetBytes(header.Table_3_Count * 0x8, SeekToOffset: header.Table_3_Offset));
            Table4 = new MStream(BufferStream.GetBytes(header.Table_4_Size, SeekToOffset: header.Table_4_Offset));
            Table5 = new MStream(BufferStream.GetBytes(header.Table_5_Size, SeekToOffset: header.Table_5_Offset));
            NameTable = new MStream(BufferStream.GetBytes(header.Names_Size, SeekToOffset: header.Names_Offset));


            RMDBLOBPaths = new List<RMDBLOBPath>();
            for (int i = 0; i < header.RMDBLOB_Path_Count; i++)
            {
                var path = new RMDBLOBPath();
                path.Read(RMDBLOBPathBlock);
                path.Path = GetName(path.NameOffset, path.NameLength);
                RMDBLOBPaths.Add(path);
            }

            Files = new List<FileInfo>();
            for (int i = 0; i < header.Table_2_Count; i++)
            {
                var file = new FileInfo();
                file.Read(Table2);
                file.Rmdtoc = this;
                file.DMKPBlock = Table4.GetBytes(file.DMKPTableSize, false, file.DMKPTableOffset);
                file.CompressInfos = new List<CompressInfo>();

                Table5.Seek(file.CompressionInfoTableOffset);
                for (int j = 0; j < file.CompressionInfoTableSize / CompressInfo.Size; j++)
                {
                    var info = new CompressInfo();
                    info.Read(Table5);
                    file.CompressInfos.Add(info);
                }

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
            MakeNode(Folders, Root, Folders[0]);


        }

        private void MakeNode(List<FolderInfo> folders, TreeNode node, FolderInfo Folder)
        {
            for (int i = Folder.FolderIndex; i < Folder.FolderIndex + Folder.FolderCount; i++)
            {
               var treenode = new TreeNode(folders[i].Name);
                treenode.Tag = folders[i];
                node.Nodes.Add(treenode);
                MakeNode(folders, treenode, folders[i]);
            }
        }

        public byte[] GetBuffer(IStream stream, long offset, int BlockSize)
        {
            var DecompressBuffer = new MStream();

            var StoreOffset = stream.Position;
            stream.Position = offset;
            for (int i = 0; i < BlockSize / CompressInfo.Size; i++)
            {
                var info = new CompressInfo();
                info.Read(stream);

                var pos = stream.Position;
                stream.Seek(info.BufferOffset);
                DecompressBuffer.SetBytes(LZ4Codec.Decode(stream.GetBytes(info.BufferSize), 0, info.BufferSize, info.BufferUSize));

                stream.Position = pos;
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
            compressinfo.isCompressed = 0x10;
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
                    info.Write(Table5);
                }
            }

            Table2 = new MStream();
            foreach (var file in Files)
            {
                file.Write(Table2);
            }

            BufferStream = new MStream();

            header.RMDBLOB_Path_Offset = (int)BufferStream.Position;
            BufferStream.SetBytes(RMDBLOBPathBlock.ToArray());

            header.Table_1_Offset = (int)BufferStream.Position;
            BufferStream.SetBytes(Table1.ToArray());

            header.Table_2_Offset = (int)BufferStream.Position;
            BufferStream.SetBytes(Table2.ToArray());

            header.Names_Offset = (int)BufferStream.Position;
            BufferStream.SetBytes(NameTable.ToArray());

            header.Table_3_Offset = (int)BufferStream.Position;
            BufferStream.SetBytes(Table3.ToArray());

            header.Table_4_Offset = (int)BufferStream.Position;
            header.Table_4_Size = (int)Table4.GetSize();
            BufferStream.SetBytes(Table4.ToArray());

            header.Table_5_Offset = (int)BufferStream.Position;
            header.Table_5_Size = (int)Table5.GetSize();
            BufferStream.SetBytes(Table5.ToArray());


            rmdtocStream.SetSize(4096);
            rmdtocStream.SetPosition(4096);


            var infoTable = CompressFile(BufferStream.ToArray(), out byte[] CompressedBytes);

            rmdtocStream.SetBytes(CompressedBytes);

            rmdtocStream.Seek(0);
            rmdtocStream.SetBytes(new byte[4096], false);

            header.CompressionInfoTableSize = infoTable.Length * CompressInfo.Size;

            rmdtocStream.SetStructureValus(header);

            int offset = 4096;
            for (int i = 0; i < infoTable.Length; i++)
            {
                infoTable[i].BufferOffset = offset;
                infoTable[i].Write(rmdtocStream);
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
