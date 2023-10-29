using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;

namespace Helper
{


    public class FStream : FileStream, IStream
    {
        public FStream(string path, FileMode mode, FileAccess access) : base(path, mode, access)
        {
        }
        public FStream(string path, FileMode mode, FileAccess access, FileShare share) : base(path, mode, access, share)
        {
        }

        public FStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize) : base(path, mode, access, share, bufferSize)
        {
        }

        public FStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options) : base(path, mode, access, share, bufferSize, options)
        {
        }

        public FStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync) : base(path, mode, access, share, bufferSize, useAsync)
        {
        }

        public FStream(string path, FileMode mode, FileSystemRights rights, FileShare share, int bufferSize, FileOptions options, FileSecurity fileSecurity) : base(path, mode, rights, share, bufferSize, options, fileSecurity)
        {
        }



        #region Controls
        public static FStream Open(string path, FileMode mode, FileAccess access)
        {
            return new FStream(path, mode, access);
        }

        private T[] ConvertTo<T>(byte[] bytes, Endian endian = Endian.Little)
        {
            var array = new T[bytes.Length / Marshal.SizeOf(typeof(T))];
            for (int i = 0; i < array.Length; i++)
            {
                var Val = new byte[Marshal.SizeOf(typeof(T))];
                Array.Copy(bytes, i * Marshal.SizeOf(typeof(T)), Val, 0, Marshal.SizeOf(typeof(T)));
                if (endian == Endian.Big)
                {
                    Val.Reverse();
                }
                var handle = GCHandle.Alloc(Val, GCHandleType.Pinned);
                array[i] = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
                handle.Free();
            }
            return array;
        }

        public T[] GetArray<T>(int Count, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
        {
            return ConvertTo<T>(GetBytes(Count * Marshal.SizeOf(typeof(T)), SavePosition, SeekToOffset), endian);
        }

        public T Get<T>(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
        {
            return ConvertTo<T>(GetBytes(Marshal.SizeOf(typeof(T)), SavePosition, SeekToOffset), endian)[0];
        }

        public void Set<T>(T[] values, Endian endian = Endian.Little)
        {
            byte[] byteArray = new byte[values.Length * Marshal.SizeOf(typeof(T))];
            for (int i = 0; i < values.Length; i++)
            {
                byte[] valBytes = new byte[Marshal.SizeOf(typeof(T))];
                IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)));
                Marshal.StructureToPtr(values[i], ptr, false);
                Marshal.Copy(ptr, valBytes, 0, Marshal.SizeOf(typeof(T)));
                Marshal.FreeHGlobal(ptr);

                if (endian == Endian.Big)
                {
                    Array.Reverse(valBytes);
                }

                Array.Copy(valBytes, 0, byteArray, i * Marshal.SizeOf(typeof(T)), Marshal.SizeOf(typeof(T)));
            }

        }

        public override long Seek(long offset, SeekOrigin origin = SeekOrigin.Begin)
        {
            return base.Seek(offset, origin);
        }

        public void Skip(long Count)
        {
            this.Seek(Count, SeekOrigin.Current);
        }

        public long GetPosition()
        {
            return Position;
        }

        public long GetSize()
        {
            return Length;
        }

        public void SetPosition(long offset)
        {
            Position = offset;
        }

        public void SetSize(long Size)
        {
            SetLength(Size);
        }

        private void ExpandFile(long offset, int extraBytes)
        {
            const int SIZE = 4096;
            var buffer = new byte[SIZE];
            var length = Length;
            // Expand file
            SetLength(length + extraBytes);
            var pos = length;
            int to_read;
            while (pos > offset)
            {
                to_read = pos - SIZE >= offset ? SIZE : (int)(pos - offset);
                pos -= to_read;
                Position = pos;
                Read(buffer, 0, to_read);
                Position = pos + extraBytes;
                Write(buffer, 0, to_read);
            }
        }

        private void ReduceFile(long offset, int extraBytes)
        {
            const int SIZE = 4096;
            var buffer = new byte[SIZE];
            var length = Length;
            // Reduce file
            var pos = offset + extraBytes;
            int to_read;
            while (pos < length)
            {
                to_read = pos + SIZE <= length ? SIZE : (int)(length - pos);
                Position = pos;
                Read(buffer, 0, to_read);
                Position = pos - extraBytes;
                Write(buffer, 0, to_read);
                pos += to_read;
            }
            SetLength(length - extraBytes);
        }

        public byte[] ToArray()
        {
            return GetBytes(Convert.ToInt32(Length), false, 0);

        }
        public bool EndofFile()
        {
            return Position == Length;
        }

        public void WriteFile(string path)
        {
            File.WriteAllBytes(path, ToArray());
        }


        #endregion

        #region Boolean
        public bool GetBoolValue(bool SavePosition = true, int SeekToOffset = -1)
        {
            return Convert.ToBoolean(GetByteValue(SavePosition, SeekToOffset));
        }
        public void SetBoolValue(bool Value, bool SavePosition = true, int SeekToOffset = -1)
        {
            SetByteValue(Convert.ToByte(Value), SavePosition, SeekToOffset);
        }

        public void InsertBoolValue(bool Value, bool SavePosition = true, int SeekToOffset = -1)
        {
            InsertByteValue(Convert.ToByte(Value), SavePosition, SeekToOffset);
        }

        public void DeleteBoolValue(bool SavePosition = true, int SeekToOffset = -1)
        {
            DeleteByteValue(SavePosition, SeekToOffset);
        }

        #endregion


        #region int8,uint8

        public byte[] GetBytes(int Count, bool SavePosition = true, int SeekToOffset = -1)
        {
            long PositionBackup = this.Position;
            if (SeekToOffset != -1)
            {
                this.Seek(SeekToOffset, SeekOrigin.Begin);

            }
            byte[] buffer = new byte[Count];
            this.Read(buffer, 0, Count);
            if (!SavePosition)
            {
                this.Seek(PositionBackup, SeekOrigin.Begin);
            }
            return buffer;
        }

        public void SetBytes(byte[] buffer, bool SavePosition = true, int SeekToOffset = -1)
        {
            long PositionBackup = this.Position;
            if (SeekToOffset != -1)
            {
                this.Seek(SeekToOffset, SeekOrigin.Begin);
            }
            this.Write(buffer, 0, buffer.Length);
            if (!SavePosition)
            {
                this.Seek(PositionBackup, SeekOrigin.Begin);
            }
            Flush();
        }

        public void InsertBytes(byte[] buffer, bool SavePosition = true, int SeekToOffset = -1)
        {

            long PositionBackup = this.Position;
            if (SeekToOffset != -1)
            {
                this.Seek(SeekToOffset, SeekOrigin.Begin);
            }
            long NewPosition = this.Position;
            ExpandFile(NewPosition, buffer.Length);
            this.Position = NewPosition;
            SetBytes(buffer);
            if (!SavePosition)
            {
                this.Seek(PositionBackup, SeekOrigin.Begin);
            }
            Flush();
        }

        public void DeleteBytes(int Count, bool SavePosition = true, int SeekToOffset = -1)
        {
            long PositionBackup = this.Position;
            if (SeekToOffset != -1)
            {
                this.Seek(SeekToOffset, SeekOrigin.Begin);
            }
            long NewPosition = this.Position;
            ReduceFile(NewPosition, Count);
            Seek(NewPosition);
            if (!SavePosition)
            {
                this.Seek(PositionBackup, SeekOrigin.Begin);
            }
            Flush();
        }


        public void ReplaceBytes(int OldBytesLenght, byte[] NewBytes, bool SavePosition = true, int SeekToOffset = -1)
        {
            DeleteBytes(OldBytesLenght, SavePosition, SeekToOffset);
            InsertBytes(NewBytes, SavePosition, SeekToOffset);
        }

        public byte GetByteValue(bool SavePosition = true, int SeekToOffset = -1)
        {
            return GetBytes(sizeof(byte), SavePosition, SeekToOffset)[0];
        }

        public sbyte GetSByteValue(bool SavePosition = true, int SeekToOffset = -1)
        {
            return (sbyte)GetBytes(sizeof(sbyte), SavePosition, SeekToOffset)[0];
        }

        public void SetByteValue(byte value, bool SavePosition = true, int SeekToOffset = -1)
        {
            SetBytes(new byte[] { value }, SavePosition, SeekToOffset);
        }

        public void SetUByteValue(sbyte value, bool SavePosition = true, int SeekToOffset = -1)
        {
            SetBytes(new byte[] { (byte)value }, SavePosition, SeekToOffset);
        }
        public void InsertByteValue(byte value, bool SavePosition = true, int SeekToOffset = -1)
        {
            InsertBytes(new byte[] { value }, SavePosition, SeekToOffset);
        }
        public void InsertUByteValue(sbyte value, bool SavePosition = true, int SeekToOffset = -1)
        {
            InsertBytes(new byte[] { (byte)value }, SavePosition, SeekToOffset);
        }
        public void DeleteByteValue(bool SavePosition = true, int SeekToOffset = -1)
        {
            DeleteBytes(1, SavePosition, SeekToOffset);
        }
        public void DeleteUByteValue(bool SavePosition = true, int SeekToOffset = -1)
        {
            DeleteBytes(1, SavePosition, SeekToOffset);
        }

        #endregion


        #region int16,uint16
        public short GetShortValue(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
        {
            byte[] array = GetBytes(sizeof(short), SavePosition, SeekToOffset);
            if (endian == Endian.Big)
            {
                Array.Reverse(array);
            }
            return BitConverter.ToInt16(array, 0);
        }

        public ushort GetUShortValue(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
        {
            return (ushort)(GetShortValue(SavePosition, SeekToOffset, endian));
        }


        public void SetShortValue(short value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
        {
            byte[] array = BitConverter.GetBytes(value);

            if (endian == Endian.Big)
            {
                Array.Reverse(array);
            }
            SetBytes(array, SavePosition, SeekToOffset);
        }

        public void SetUShortValue(ushort value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
        {
            SetShortValue((short)value, SavePosition, SeekToOffset, endian);
        }

        public void InsertShortValue(short value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
        {

            byte[] array = BitConverter.GetBytes(value);

            if (endian == Endian.Big)
            {
                Array.Reverse(array);
            }
            InsertBytes(array, SavePosition, SeekToOffset);

        }
        public void InsertUShortValue(ushort value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
        {
            InsertShortValue((short)value, SavePosition, SeekToOffset, endian);
        }

        public void DeleteShortValue(bool SavePosition = true, int SeekToOffset = -1)
        {
            DeleteBytes(sizeof(short), SavePosition, SeekToOffset);
        }

        public void DeleteUShortValue(bool SavePosition = true, int SeekToOffset = -1)
        {
            DeleteShortValue(SavePosition, SeekToOffset);
        }
        #endregion


        #region int32,uint32
        public int GetIntValue(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
        {
            byte[] array = GetBytes(sizeof(int), SavePosition, SeekToOffset);
            if (endian == Endian.Big)
            {
                Array.Reverse(array);
            }
            return BitConverter.ToInt32(array, 0);
        }

        public uint GetUIntValue(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
        {
            return (uint)(GetIntValue(SavePosition, SeekToOffset, endian));
        }


        public void SetIntValue(int value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
        {
            byte[] array = BitConverter.GetBytes(value);

            if (endian == Endian.Big)
            {
                Array.Reverse(array);
            }
            SetBytes(array, SavePosition, SeekToOffset);
        }

        public void SetUIntValue(uint value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
        {
            SetIntValue((int)value, SavePosition, SeekToOffset, endian);
        }

        public void InsertIntValue(int value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
        {

            byte[] array = BitConverter.GetBytes(value);

            if (endian == Endian.Big)
            {
                Array.Reverse(array);
            }
            InsertBytes(array, SavePosition, SeekToOffset);

        }
        public void InsertUIntValue(uint value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
        {
            InsertIntValue((int)value, SavePosition, SeekToOffset, endian);
        }

        public void DeleteIntValue(bool SavePosition = true, int SeekToOffset = -1)
        {
            DeleteBytes(sizeof(int), SavePosition, SeekToOffset);
        }

        public void DeleteUIntValue(bool SavePosition = true, int SeekToOffset = -1)
        {
            DeleteIntValue(SavePosition, SeekToOffset);
        }
        #endregion


        #region int64,uint64
        public long GetInt64Value(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
        {
            byte[] array = GetBytes(sizeof(long), SavePosition, SeekToOffset);
            if (endian == Endian.Big)
            {
                Array.Reverse(array);
            }
            return BitConverter.ToInt64(array, 0);
        }

        public ulong GetUInt64Value(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
        {
            return (ulong)(GetInt64Value(SavePosition, SeekToOffset, endian));
        }


        public void SetInt64Value(long value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
        {
            byte[] array = BitConverter.GetBytes(value);

            if (endian == Endian.Big)
            {
                Array.Reverse(array);
            }
            SetBytes(array, SavePosition, SeekToOffset);
        }

        public void SetUInt64Value(ulong value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
        {
            SetInt64Value((long)value, SavePosition, SeekToOffset, endian);
        }

        public void InsertInt64Value(long value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
        {

            byte[] array = BitConverter.GetBytes(value);

            if (endian == Endian.Big)
            {
                Array.Reverse(array);
            }
            InsertBytes(array, SavePosition, SeekToOffset);

        }
        public void InsertUInt64Value(ulong value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
        {
            InsertInt64Value((long)value, SavePosition, SeekToOffset, endian);
        }

        public void DeleteInt64Value(bool SavePosition = true, int SeekToOffset = -1)
        {
            DeleteBytes(sizeof(long), SavePosition, SeekToOffset);
        }

        public void DeleteUInt64Value(bool SavePosition = true, int SeekToOffset = -1)
        {
            DeleteInt64Value(SavePosition, SeekToOffset);
        }
        #endregion


        #region Double
        public double GetDoubleValue(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
        {
            byte[] array = GetBytes(sizeof(double), SavePosition, SeekToOffset);
            if (endian == Endian.Big)
            {
                Array.Reverse(array);
            }
            return BitConverter.ToDouble(array, 0);
        }


        public void SetDoubleValue(double value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
        {
            byte[] array = BitConverter.GetBytes(value);

            if (endian == Endian.Big)
            {
                Array.Reverse(array);
            }
            SetBytes(array, SavePosition, SeekToOffset);
        }

        public void InsertDoubleValue(double value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
        {

            byte[] array = BitConverter.GetBytes(value);

            if (endian == Endian.Big)
            {
                Array.Reverse(array);
            }
            InsertBytes(array, SavePosition, SeekToOffset);

        }

        public void DeleteDoubleValue(bool SavePosition = true, int SeekToOffset = -1)
        {
            DeleteBytes(sizeof(double), SavePosition, SeekToOffset);
        }

        #endregion


        #region Float
        public float GetFloatValue(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
        {
            byte[] array = GetBytes(sizeof(float), SavePosition, SeekToOffset);
            if (endian == Endian.Big)
            {
                Array.Reverse(array);
            }
            return BitConverter.ToSingle(array, 0);
        }


        public void SetFloatValue(float value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
        {
            byte[] array = BitConverter.GetBytes(value);

            if (endian == Endian.Big)
            {
                Array.Reverse(array);
            }
            SetBytes(array, SavePosition, SeekToOffset);
        }

        public void InsertFloatValue(float value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little)
        {

            byte[] array = BitConverter.GetBytes(value);

            if (endian == Endian.Big)
            {
                Array.Reverse(array);
            }
            InsertBytes(array, SavePosition, SeekToOffset);

        }

        public void DeleteFloatValue(bool SavePosition = true, int SeekToOffset = -1)
        {
            DeleteBytes(sizeof(float), SavePosition, SeekToOffset);
        }
        #endregion


        #region String
        public string GetStringValue(int StringLenght, Encoding encoding) => GetStringValue(StringLenght, true, -1, encoding);
        public string GetStringValue(int StringLenght, bool SavePosition, Encoding encoding) => GetStringValue(StringLenght, SavePosition, -1, encoding);
        public string GetStringValue(int StringLenght, bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.ASCII;
            }
            byte[] array = GetBytes(StringLenght, SavePosition, SeekToOffset);
            return encoding.GetString(array);
        }


        public void SetStringValue(string String, Encoding encoding) => SetStringValue(String, true, -1, encoding);
        public void SetStringValue(string String, bool SavePosition, Encoding encoding) => SetStringValue(String, SavePosition, -1, encoding);
        public void SetStringValue(string String, bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.ASCII;
            }
            SetBytes(encoding.GetBytes(String), SavePosition, SeekToOffset);
        }


        public void InsertStringValue(string String, Encoding encoding) => InsertStringValue(String, true, -1, encoding);
        public void InsertStringValue(string String, bool SavePosition, Encoding encoding) => InsertStringValue(String, SavePosition, -1, encoding);
        public void InsertStringValue(string String, bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.ASCII;
            }
            InsertBytes(encoding.GetBytes(String), SavePosition, SeekToOffset);
        }


        private byte[] GetString(Encoding encoding)
        {
            List<byte> StringValues = new List<byte>();
            if (encoding != Encoding.Unicode)
            {
                while (true)
                {
                    StringValues.Add(GetByteValue());
                    if (StringValues[StringValues.Count - 1] == 0)
                    {
                        break;
                    }
                }
            }
            else
            {
                while (true)
                {
                    StringValues.Add(GetByteValue());
                    StringValues.Add(GetByteValue());
                    if (StringValues[StringValues.Count - 1] == 0 && StringValues[StringValues.Count - 2] == 0)
                    {
                        break;
                    }
                }

            }
            return StringValues.ToArray();
        }

        public string GetStringValueN(bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.ASCII;
            }

            long Position = this.Position;
            string Value;
            if (SeekToOffset != -1)
            {
                this.Seek(SeekToOffset, SeekOrigin.Begin);
            }
            Value = encoding.GetString(GetString(encoding)).TrimEnd('\0');
            if (!SavePosition)
            {
                this.Seek(Position, SeekOrigin.Begin);
            }
            return Value;
        }


        public void DeleteStringN(bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.ASCII;
            }

            long Position = this.Position;
            if (SeekToOffset != -1)
            {
                this.Seek(SeekToOffset, SeekOrigin.Begin);
            }
            DeleteBytes(GetString(encoding).Length, SavePosition, SeekToOffset);
            this.Seek(Position, SeekOrigin.Begin);
        }

        public void SetStringValueN(string String, bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.ASCII;
            }
            SetBytes(encoding.GetBytes(String + '\0'), SavePosition, SeekToOffset);
        }

        public void InsertStringValueN(string String, bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.ASCII;
            }
            InsertBytes(encoding.GetBytes(String + '\0'), SavePosition, SeekToOffset);
        }
        #endregion


        #region Struct

        public T GetStructureValues<T>(bool SavePosition = true, int SeekToOffset = -1)
        {
            var structureSize = Marshal.SizeOf(typeof(T));
            var buffer = GetBytes(structureSize, SavePosition, SeekToOffset);

            if (buffer.Length != structureSize)
            {
                throw new Exception("could not read all of data for structure");
            }

            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var structure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return structure;
        }


        public void SetStructureValus<T>(T structure, bool SavePosition = true, int SeekToOffset = -1)
        {
            var structureSize = Marshal.SizeOf(typeof(T));
            var buffer = new byte[structureSize];
            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            Marshal.StructureToPtr(structure, handle.AddrOfPinnedObject(), false);
            handle.Free();
            SetBytes(buffer, SavePosition, SeekToOffset);
        }
        #endregion

    }

}
