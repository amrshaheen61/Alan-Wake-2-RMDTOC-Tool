using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using static Helper.DDSCooker;

namespace Helper
{
    public class DDSToBitmap
    {
        private static DDS_Info info { get; set; }

        //from http://code.google.com/p/kprojects
        #region DXT1
        static private Bitmap UncompressDXT1(IStream stream, int w, int h)
        {
            Bitmap res = new Bitmap((w < 4) ? 4 : w, (h < 4) ? 4 : h);

            for (int j = 0; j < h; j += 4)
            {
                for (int i = 0; i < w; i += 4)
                {
                    DecompressBlockDXT1(i, j, stream.GetBytes(8), res);
                }
            }
            return res;
        }

        static private void DecompressBlockDXT1(int x, int y, byte[] blockStorage, Bitmap image)
        {
            ushort color0 = (ushort)(blockStorage[0] | blockStorage[1] << 8);
            ushort color1 = (ushort)(blockStorage[2] | blockStorage[3] << 8);

            int temp;

            temp = (color0 >> 11) * 255 + 16;
            byte r0 = (byte)((temp / 32 + temp) / 32);
            temp = ((color0 & 0x07E0) >> 5) * 255 + 32;
            byte g0 = (byte)((temp / 64 + temp) / 64);
            temp = (color0 & 0x001F) * 255 + 16;
            byte b0 = (byte)((temp / 32 + temp) / 32);

            temp = (color1 >> 11) * 255 + 16;
            byte r1 = (byte)((temp / 32 + temp) / 32);
            temp = ((color1 & 0x07E0) >> 5) * 255 + 32;
            byte g1 = (byte)((temp / 64 + temp) / 64);
            temp = (color1 & 0x001F) * 255 + 16;
            byte b1 = (byte)((temp / 32 + temp) / 32);

            uint code = (uint)(blockStorage[4] | blockStorage[5] << 8 | blockStorage[6] << 16 | blockStorage[7] << 24);

            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    Color finalColor = Color.FromArgb(0);
                    byte positionCode = (byte)((code >> 2 * (4 * j + i)) & 0x03);

                    if (color0 > color1)
                    {
                        switch (positionCode)
                        {
                            case 0:
                                finalColor = Color.FromArgb(255, r0, g0, b0);
                                break;
                            case 1:
                                finalColor = Color.FromArgb(255, r1, g1, b1);
                                break;
                            case 2:
                                finalColor = Color.FromArgb(255, (2 * r0 + r1) / 3, (2 * g0 + g1) / 3, (2 * b0 + b1) / 3);
                                break;
                            case 3:
                                finalColor = Color.FromArgb(255, (r0 + 2 * r1) / 3, (g0 + 2 * g1) / 3, (b0 + 2 * b1) / 3);
                                break;
                        }
                    }
                    else
                    {
                        switch (positionCode)
                        {
                            case 0:
                                finalColor = Color.FromArgb(255, r0, g0, b0);
                                break;
                            case 1:
                                finalColor = Color.FromArgb(255, r1, g1, b1);
                                break;
                            case 2:
                                finalColor = Color.FromArgb(255, (r0 + r1) / 2, (g0 + g1) / 2, (b0 + b1) / 2);
                                break;
                            case 3:
                                finalColor = Color.FromArgb(255, 0, 0, 0);
                                break;
                        }
                    }

                    image.SetPixel(x + i, y + j, finalColor);
                }
            }
        }
        #endregion
        //from http://code.google.com/p/kprojects
        #region DXT5
        static private Bitmap UncompressDXT5(IStream stream, int w, int h)
        {
            Bitmap res = new Bitmap((w < 4) ? 4 : w, (h < 4) ? 4 : h);

            for (int j = 0; j < h; j += 4)
            {
                for (int i = 0; i < w; i += 4)
                {
                    DecompressBlockDXT5(i, j, stream.GetBytes(16), res);
                }
            }
            return res;
        }

        static void DecompressBlockDXT5(int x, int y, byte[] blockStorage, Bitmap image)
        {
            byte alpha0 = blockStorage[0];
            byte alpha1 = blockStorage[1];

            int bitOffset = 2;
            uint alphaCode1 = (uint)(blockStorage[bitOffset + 2] | (blockStorage[bitOffset + 3] << 8) | (blockStorage[bitOffset + 4] << 16) | (blockStorage[bitOffset + 5] << 24));
            ushort alphaCode2 = (ushort)(blockStorage[bitOffset + 0] | (blockStorage[bitOffset + 1] << 8));

            ushort color0 = (ushort)(blockStorage[8] | blockStorage[9] << 8);
            ushort color1 = (ushort)(blockStorage[10] | blockStorage[11] << 8);

            int temp;

            temp = (color0 >> 11) * 255 + 16;
            byte r0 = (byte)((temp / 32 + temp) / 32);
            temp = ((color0 & 0x07E0) >> 5) * 255 + 32;
            byte g0 = (byte)((temp / 64 + temp) / 64);
            temp = (color0 & 0x001F) * 255 + 16;
            byte b0 = (byte)((temp / 32 + temp) / 32);

            temp = (color1 >> 11) * 255 + 16;
            byte r1 = (byte)((temp / 32 + temp) / 32);
            temp = ((color1 & 0x07E0) >> 5) * 255 + 32;
            byte g1 = (byte)((temp / 64 + temp) / 64);
            temp = (color1 & 0x001F) * 255 + 16;
            byte b1 = (byte)((temp / 32 + temp) / 32);

            uint code = (uint)(blockStorage[12] | blockStorage[13] << 8 | blockStorage[14] << 16 | blockStorage[15] << 24);

            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    int alphaCodeIndex = 3 * (4 * j + i);
                    int alphaCode;

                    if (alphaCodeIndex <= 12)
                    {
                        alphaCode = (alphaCode2 >> alphaCodeIndex) & 0x07;
                    }
                    else if (alphaCodeIndex == 15)
                    {
                        alphaCode = (int)((alphaCode2 >> 15) | ((alphaCode1 << 1) & 0x06));
                    }
                    else
                    {
                        alphaCode = (int)((alphaCode1 >> (alphaCodeIndex - 16)) & 0x07);
                    }

                    byte finalAlpha;
                    if (alphaCode == 0)
                    {
                        finalAlpha = alpha0;
                    }
                    else if (alphaCode == 1)
                    {
                        finalAlpha = alpha1;
                    }
                    else
                    {
                        if (alpha0 > alpha1)
                        {
                            finalAlpha = (byte)(((8 - alphaCode) * alpha0 + (alphaCode - 1) * alpha1) / 7);
                        }
                        else
                        {
                            if (alphaCode == 6)
                                finalAlpha = 0;
                            else if (alphaCode == 7)
                                finalAlpha = 255;
                            else
                                finalAlpha = (byte)(((6 - alphaCode) * alpha0 + (alphaCode - 1) * alpha1) / 5);
                        }
                    }

                    byte colorCode = (byte)((code >> 2 * (4 * j + i)) & 0x03);

                    Color finalColor = new Color();
                    switch (colorCode)
                    {
                        case 0:
                            finalColor = Color.FromArgb(finalAlpha, r0, g0, b0);
                            break;
                        case 1:
                            finalColor = Color.FromArgb(finalAlpha, r1, g1, b1);
                            break;
                        case 2:
                            finalColor = Color.FromArgb(finalAlpha, (2 * r0 + r1) / 3, (2 * g0 + g1) / 3, (2 * b0 + b1) / 3);
                            break;
                        case 3:
                            finalColor = Color.FromArgb(finalAlpha, (r0 + 2 * r1) / 3, (g0 + 2 * g1) / 3, (b0 + 2 * b1) / 3);
                            break;
                    }
                    image.SetPixel(x + i, y + j, finalColor);
                }
            }
        }
        #endregion
        //from https://github.com/ME3Tweaks/ME3ExplorerLib/blob/c9144346af32d9cab674f2cd0ad1bcb3358ed34f/Helpers/DDSImage.cs
        #region DXT3
        private static Bitmap UncompressDXT3(IStream stream, int w, int h)
        {
            const int bufferSize = 16;
            byte[] blockStorage = new byte[bufferSize];
            using (MemoryStream bitmapStream = new MemoryStream(w * h * 2))
            {
                using (BinaryWriter bitmapBW = new BinaryWriter(bitmapStream))
                {
                    for (int s = 0; s < h; s += 4)
                    {
                        for (int t = 0; t < w; t += 4)
                        {
                            blockStorage = stream.GetBytes(bufferSize);
                            {
                                int color0 = blockStorage[8] | blockStorage[9] << 8;
                                int color1 = blockStorage[10] | blockStorage[11] << 8;

                                int temp;

                                temp = (color0 >> 11) * 255 + 16;
                                int r0 = ((temp >> 5) + temp) >> 5;
                                temp = ((color0 & 0x07E0) >> 5) * 255 + 32;
                                int g0 = ((temp >> 6) + temp) >> 6;
                                temp = (color0 & 0x001F) * 255 + 16;
                                int b0 = ((temp >> 5) + temp) >> 5;

                                temp = (color1 >> 11) * 255 + 16;
                                int r1 = ((temp >> 5) + temp) >> 5;
                                temp = ((color1 & 0x07E0) >> 5) * 255 + 32;
                                int g1 = ((temp >> 6) + temp) >> 6;
                                temp = (color1 & 0x001F) * 255 + 16;
                                int b1 = ((temp >> 5) + temp) >> 5;

                                int code = blockStorage[12] | blockStorage[13] << 8 | blockStorage[14] << 16 | blockStorage[15] << 24;

                                for (int j = 0; j < 4; j++)
                                {
                                    bitmapStream.Seek(((s + j) * w * 4) + (t * 4), SeekOrigin.Begin);
                                    for (int i = 0; i < 4; i++)
                                    {
                                        byte alpha = (byte)((blockStorage[(j * i) < 8 ? 0 : 1] >> (((i * j) % 8) * 4)) & 0xFF);
                                        alpha = (byte)((alpha << 4) | alpha);
                                        if (!IsAlpha())
                                            alpha = 0xFF;

                                        int fCol = 0;
                                        int colorCode = (code >> 2 * (4 * j + i)) & 0x03;

                                        switch (colorCode)
                                        {
                                            case 0:
                                                fCol = b0 | g0 << 8 | r0 << 16 | 0xFF << alpha;
                                                break;
                                            case 1:
                                                fCol = b1 | g1 << 8 | r1 << 16 | 0xFF << alpha;
                                                break;
                                            case 2:
                                                fCol = (2 * b0 + b1) / 3 | (2 * g0 + g1) / 3 << 8 | (2 * r0 + r1) / 3 << 16 | 0xFF << alpha;
                                                break;
                                            case 3:
                                                fCol = (b0 + 2 * b1) / 3 | (g0 + 2 * g1) / 3 << 8 | (r0 + 2 * r1) / 3 << 16 | 0xFF << alpha;
                                                break;
                                        }

                                        bitmapBW.Write(fCol);
                                    }
                                }
                            }
                        }
                    }


                    var bmp = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    {
                        BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0,
                                                            bmp.Width,
                                                            bmp.Height),
                                              ImageLockMode.WriteOnly,
                                              bmp.PixelFormat);

                        Marshal.Copy(bitmapStream.ToArray(), 0, bmpData.Scan0, (int)bitmapStream.Length);
                        bmp.UnlockBits(bmpData);
                    }

                    return bmp;
                }
            }
        }
        #endregion

        #region   A8R8G8B8 or X8R8G8B8 or A8B8G8R8 or X8B8G8R8
        static private Bitmap Uncompress8_8_8_8(IStream stream, int w, int h)
        {
            uint RBitMask = info.DDS_HEADER.ddspf.RBitMask;
            uint GBitMask = info.DDS_HEADER.ddspf.GBitMask;
            uint BBitMask = info.DDS_HEADER.ddspf.BBitMask;
            uint ABitMask = info.DDS_HEADER.ddspf.ABitMask;


            if (info.DDS_HEADER.ddspf.RGBBitCount == 0)
            {
                (RBitMask, GBitMask, BBitMask, ABitMask) = DDSCooker.GetMask(info.Format);
            }

            Bitmap bitmap = new Bitmap(w, h);
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    uint RGBABits = stream.GetUIntValue();
                    int b = GetMaskValue(BBitMask, RGBABits);
                    int g = GetMaskValue(GBitMask, RGBABits);
                    int r = GetMaskValue(RBitMask, RGBABits);
                    int a = GetMaskValue(ABitMask, RGBABits);
                    if (!IsAlpha())
                    {
                        a = 255;
                    }
                    bitmap.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                }

            }
            return bitmap;
        }
        #endregion

        #region   A8L8
        static private Bitmap UncompressA8L8(IStream stream, int w, int h)
        {
            Bitmap bitmap = new Bitmap(w, h);

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    uint RGBABits = stream.GetUShortValue();
                    int b = GetMaskValue(info.DDS_HEADER.ddspf.BBitMask, RGBABits);
                    int g = GetMaskValue(info.DDS_HEADER.ddspf.GBitMask, RGBABits);
                    int r = GetMaskValue(info.DDS_HEADER.ddspf.RBitMask, RGBABits);
                    int a = GetMaskValue(info.DDS_HEADER.ddspf.ABitMask, RGBABits);
                    if (!IsAlpha())
                    {
                        a = 255;
                    }
                    bitmap.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                }

            }
            return bitmap;
        }
        #endregion

        #region   V8U8
        static private Bitmap UncompressV8U8(IStream stream, int w, int h)
        {
            Bitmap bitmap = new Bitmap(w, h);
            int Step = 0;
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    uint RGBABits = stream.GetUShortValue();
                    int r = 0x7F + (sbyte)GetMaskValue(info.DDS_HEADER.ddspf.RBitMask, RGBABits);
                    int g = 0x7F + (sbyte)GetMaskValue(info.DDS_HEADER.ddspf.GBitMask, RGBABits);
                    int b = 0xff;
                    int a = 0xff;

                    bitmap.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                }

            }
            return bitmap;
        }
        #endregion

        #region   B4G4R4A4
        static private Bitmap UncompressB4G4R4A4(IStream stream, int w, int h)
        {
            Bitmap bitmap = new Bitmap(w, h);
            int Step = 0;
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    uint RGBABits = stream.GetUShortValue();
                    int r = (sbyte)GetMaskValue(info.DDS_HEADER.ddspf.RBitMask, RGBABits) << 4;
                    int g = (sbyte)GetMaskValue(info.DDS_HEADER.ddspf.GBitMask, RGBABits) << 4;
                    int b = (sbyte)GetMaskValue(info.DDS_HEADER.ddspf.BBitMask, RGBABits) << 4;
                    int a = (sbyte)GetMaskValue(info.DDS_HEADER.ddspf.ABitMask, RGBABits) << 4;
                    bitmap.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                }

            }
            return bitmap;
        }
        #endregion

        #region   B5G5R5A1
        static private Bitmap UncompressB5G5R5A1(IStream stream, int w, int h)
        {
            Bitmap bitmap = new Bitmap(w, h);
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    uint RGBABits = stream.GetUShortValue();
                    int r = (sbyte)GetMaskValue(info.DDS_HEADER.ddspf.RBitMask, RGBABits) << 3;
                    int g = (sbyte)GetMaskValue(info.DDS_HEADER.ddspf.GBitMask, RGBABits) << 3;
                    int b = (sbyte)GetMaskValue(info.DDS_HEADER.ddspf.BBitMask, RGBABits) << 3;
                    int a = (sbyte)GetMaskValue(info.DDS_HEADER.ddspf.ABitMask, RGBABits) == 1 ? 255 : 0;
                    bitmap.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                }

            }
            return bitmap;
        }
        #endregion

        #region L8 or A8
        static private Bitmap Uncompress8bits(IStream stream, int w, int h)
        {
            Bitmap bitmap = new Bitmap(w, h);
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    uint RGBABits = stream.GetByteValue();
                    int r = 0xff;
                    int g = 0xff;
                    int b = 0xff;
                    int a = 0xff;
                    if (info.Format == DDSFormat.r8_unorm)
                    {
                        r = (int)RGBABits;
                        g = (int)RGBABits;
                        b = (int)RGBABits;
                        a = 0xff;
                    }
                    if (info.Format == DDSFormat.a8_unorm)
                    {
                        r = 0xff;
                        g = 0xff;
                        b = 0xff;
                        a = (int)RGBABits;
                    }

                    bitmap.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                }

            }
            return bitmap;
        }
        #endregion

        private static bool IsAlpha()
        {
            if (info.DDS_HEADER.ddspf.flags.Equals(DDPF.DDPF_PAL8A)
             || info.DDS_HEADER.ddspf.flags.Equals(DDPF.DDPF_ALPHA)
             || info.DDS_HEADER.ddspf.flags.Equals(DDPF.DDPF_RGBA))
            {
                return true;
            }

            return false;
        }

        private static byte GetMaskValue(uint Mask, uint Bits)
        {

            BitArray Valuebits = new BitArray(BitConverter.GetBytes(Bits & Mask));
            BitArray Maskbits = new BitArray(BitConverter.GetBytes(Mask));
            int t = 0;
            for (int n = 0; n < Maskbits.Length; n++)
            {
                if (Maskbits[n] == true)
                {
                    bool value = Valuebits[n];
                    Valuebits[n] = false;
                    Valuebits[t++] = value;
                }

            }

            byte[] bytes = new byte[(Valuebits.Length / 8) + 1];
            Valuebits.CopyTo(bytes, 0);

            return bytes[0];
        }

        public static Bitmap Convert(byte[] DDSBytes)
        {

            info = DDSCooker.DdsToTex(DDSBytes);

            var stream = new MStream(info.Data);

            return GetBitMapFromDDS(info.Format, stream);
        }


        public static Bitmap Convert(IStream stream)
        {
            info = DDSCooker.DdsToTex(stream);
            return GetBitMapFromDDS(info.Format, stream);
        }



        public static Bitmap Convert(byte[] DDSBytes, DDSFormat format, int w, int h)
        {
            DDPF dwFlags = 0;
            if (format == DDSFormat.bc1_unorm || format == DDSFormat.bc2_unorm || format == DDSFormat.bc3_unorm)
            {
                dwFlags = DDPF.DDPF_FOURCC;
            }
            else if (format == DDSFormat.r8_unorm || format == DDSFormat.r16_unorm || format == DDSFormat.r8g8_unorm)
            {
                dwFlags = DDPF.DDPF_LUMINANCE;
            }
            else if (format == DDSFormat.a8_unorm)
            {
                dwFlags = DDPF.DDPF_ALPHA;
            }
            else
            {
                dwFlags = DDPF.DDPF_ALPHAPIXELS | DDPF.DDPF_RGB;
            }
            (uint RBitMask, uint GBitMask, uint BBitMask, uint ABitMask) = DDSCooker.GetMask(format);
            info = new DDS_Info()
            {
                DDS_HEADER = new DDS_HEADER()
                {
                    width = w,
                    height = h,
                    ddspf = new DDS_PIXELFORMAT()
                    {
                        flags = dwFlags,
                        RGBBitCount = BitsPerPixel(format),
                        RBitMask = RBitMask,
                        GBitMask = GBitMask,
                        BBitMask = BBitMask,
                        ABitMask = ABitMask
                    }
                },
                Format = format,
                Data = DDSBytes
            };

            var stream = new MStream(info.Data);
            return GetBitMapFromDDS(format, stream);

        }

        private static Bitmap GetBitMapFromDDS(DDSFormat format, IStream stream)
        {
            switch (format)
            {
                case DDSFormat.bc1_unorm:
                case DDSFormat.bc1_unorm_srgb:
                    return UncompressDXT1(stream, info.DDS_HEADER.width, info.DDS_HEADER.height);

                case DDSFormat.bc2_unorm:
                case DDSFormat.bc2_unorm_srgb:
                    return UncompressDXT3(stream, info.DDS_HEADER.width, info.DDS_HEADER.height);

                case DDSFormat.bc3_unorm:
                case DDSFormat.bc3_unorm_srgb:
                    return UncompressDXT5(stream, info.DDS_HEADER.width, info.DDS_HEADER.height);

                case DDSFormat.b8g8r8a8_unorm:
                case DDSFormat.b8g8r8x8_unorm:
                case DDSFormat.b8g8r8a8_unorm_srgb:
                case DDSFormat.b8g8r8x8_unorm_srgb:
                case DDSFormat.r8g8b8a8_unorm:
                case DDSFormat.r8g8b8a8_unorm_srgb:
                    return Uncompress8_8_8_8(stream, info.DDS_HEADER.width, info.DDS_HEADER.height);

                case DDSFormat.r8g8_unorm:
                    return UncompressA8L8(stream, info.DDS_HEADER.width, info.DDS_HEADER.height);

                case DDSFormat.r8g8_snorm:
                    return UncompressV8U8(stream, info.DDS_HEADER.width, info.DDS_HEADER.height);

                case DDSFormat.r8_unorm:
                case DDSFormat.a8_unorm:
                    return Uncompress8bits(stream, info.DDS_HEADER.width, info.DDS_HEADER.height);

                case DDSFormat.b4g4r4a4_unorm:
                    return UncompressB4G4R4A4(stream, info.DDS_HEADER.width, info.DDS_HEADER.height);

                case DDSFormat.b5g5r5a1_unorm:
                    return UncompressB5G5R5A1(stream, info.DDS_HEADER.width, info.DDS_HEADER.height);

                default:
                    throw new Exception("Unsupported DDS Format");


            }
        }
    }
}
