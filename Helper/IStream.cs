using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helper
{
    public enum Endian
    {
        Little,
        Big
    }
    public interface IStream
    {

        bool CanRead { get; }

        bool CanWrite { get; }

        bool CanSeek { get; }

        long Length { get; }


        string Name { get; }

        long Position { get; set; }

        void Flush();

        void SetLength(long value);

        int Read(byte[] array, int offset, int count);

        void Write(byte[] array, int offset, int count);

        int ReadByte();

        void WriteByte(byte value);

        bool EndofFile();
        void WriteFile(string path);
        Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken);

        Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken);

        Task FlushAsync(CancellationToken cancellationToken);

        void Close();
        void Dispose();
        void DeleteBoolValue(bool SavePosition = true, int SeekToOffset = -1);

        void DeleteBytes(int Count, bool SavePosition = true, int SeekToOffset = -1);
        void DeleteByteValue(bool SavePosition = true, int SeekToOffset = -1);

        void DeleteDoubleValue(bool SavePosition = true, int SeekToOffset = -1);

        void DeleteFloatValue(bool SavePosition = true, int SeekToOffset = -1);

        void DeleteInt64Value(bool SavePosition = true, int SeekToOffset = -1);

        void DeleteIntValue(bool SavePosition = true, int SeekToOffset = -1);
        void DeleteShortValue(bool SavePosition = true, int SeekToOffset = -1);


        void DeleteStringN(bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null);
        void DeleteUByteValue(bool SavePosition = true, int SeekToOffset = -1);

        void DeleteUInt64Value(bool SavePosition = true, int SeekToOffset = -1);

        void DeleteUIntValue(bool SavePosition = true, int SeekToOffset = -1);

        void DeleteUShortValue(bool SavePosition = true, int SeekToOffset = -1);

        T Get<T>(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);
        void Set<T>(T[] values, Endian endian = Endian.Little);
        T[] GetArray<T>(int Count, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

        bool GetBoolValue(bool SavePosition = true, int SeekToOffset = -1);

        byte[] GetBytes(int Count, bool SavePosition = true, int SeekToOffset = -1);

        byte GetByteValue(bool SavePosition = true, int SeekToOffset = -1);


        double GetDoubleValue(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);



        float GetFloatValue(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);


        long GetInt64Value(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);


        int GetIntValue(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

        long GetPosition();

        sbyte GetSByteValue(bool SavePosition = true, int SeekToOffset = -1);


        short GetShortValue(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

        long GetSize();

        string GetStringValue(int StringLenght, Encoding encoding);
        string GetStringValue(int StringLenght, bool SavePosition, Encoding encoding);
        string GetStringValue(int StringLenght, bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null);

        string GetStringValueN(bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null);



        T GetStructureValues<T>(bool SavePosition = true, int SeekToOffset = -1);

        ulong GetUInt64Value(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

        uint GetUIntValue(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

        ushort GetUShortValue(bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

        void InsertBoolValue(bool Value, bool SavePosition = true, int SeekToOffset = -1);

        void InsertBytes(byte[] buffer, bool SavePosition = true, int SeekToOffset = -1);
        void InsertByteValue(byte value, bool SavePosition = true, int SeekToOffset = -1);

        void InsertDoubleValue(double value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

        void InsertFloatValue(float value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

        void InsertInt64Value(long value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);


        void InsertIntValue(int value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

        void InsertShortValue(short value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);


        void InsertStringValue(string String, Encoding encoding);
        void InsertStringValue(string String, bool SavePosition, Encoding encoding);
        void InsertStringValue(string String, bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null);

        void InsertStringValueN(string String, bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null);
        void InsertUByteValue(sbyte value, bool SavePosition = true, int SeekToOffset = -1);
        void InsertUInt64Value(ulong value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);
        void InsertUIntValue(uint value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);
        void InsertUShortValue(ushort value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);


        void ReplaceBytes(int OldBytesLenght, byte[] NewBytes, bool SavePosition = true, int SeekToOffset = -1);

        long Seek(long offset, SeekOrigin origin = SeekOrigin.Begin);
        void SetBoolValue(bool Value, bool SavePosition = true, int SeekToOffset = -1);

        void SetBytes(byte[] buffer, bool SavePosition = true, int SeekToOffset = -1);

        void SetByteValue(byte value, bool SavePosition = true, int SeekToOffset = -1);


        void SetDoubleValue(double value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);


        void SetFloatValue(float value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);


        void SetInt64Value(long value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);


        void SetIntValue(int value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

        void SetPosition(long offset);

        void SetShortValue(short value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

        void SetSize(long Size);

        void SetStringValue(string String, Encoding encoding);
        void SetStringValue(string String, bool SavePosition, Encoding encoding);
        void SetStringValue(string String, bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null);

        void SetStringValueN(string String, bool SavePosition = true, int SeekToOffset = -1, Encoding encoding = null);


        void SetStructureValus<T>(T structure, bool SavePosition = true, int SeekToOffset = -1);

        void SetUByteValue(sbyte value, bool SavePosition = true, int SeekToOffset = -1);

        void SetUInt64Value(ulong value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

        void SetUIntValue(uint value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

        void SetUShortValue(ushort value, bool SavePosition = true, int SeekToOffset = -1, Endian endian = Endian.Little);

        void Skip(long Count);

        byte[] ToArray();
    }
}
