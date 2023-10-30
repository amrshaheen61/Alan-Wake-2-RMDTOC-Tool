using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;


namespace Helper
{

    public enum DDSFormat : uint
    {
        unknown = 0,
        r32g32b32a32_typeless,
        r32g32b32a32_float,
        r32g32b32a32_uint,
        r32g32b32a32_sint,
        r32g32b32_typeless,
        r32g32b32_float,
        r32g32b32_uint,
        r32g32b32_sint,
        r16g16b16a16_typeless,
        r16g16b16a16_float,
        r16g16b16a16_unorm,
        r16g16b16a16_uint,
        r16g16b16a16_snorm,
        r16g16b16a16_sint,
        r32g32_typeless,
        r32g32_float,
        r32g32_uint,
        r32g32_sint,
        r32g8x24_typeless,
        d32_float_s8x24_uint,
        r32_float_x8x24_typeless,
        x32_typeless_g8x24_uint,
        r10g10b10a2_typeless,
        r10g10b10a2_unorm,
        r10g10b10a2_uint,
        r11g11b10_float,
        r8g8b8a8_typeless,
        r8g8b8a8_unorm,
        r8g8b8a8_unorm_srgb,
        r8g8b8a8_uint,
        r8g8b8a8_snorm,
        r8g8b8a8_sint,
        r16g16_typeless,
        r16g16_float,
        r16g16_unorm,
        r16g16_uint,
        r16g16_snorm,
        r16g16_sint,
        r32_typeless,
        d32_float,
        r32_float,
        r32_uint,
        r32_sint,
        r24g8_typeless,
        d24_unorm_s8_uint,
        r24_unorm_x8_typeless,
        x24_typeless_g8_uint,
        r8g8_typeless,
        [Description("A8L8")]
        r8g8_unorm,
        r8g8_uint,
        [Description("V8U8")]
        r8g8_snorm,
        r8g8_sint,
        r16_typeless,
        r16_float,
        d16_unorm,
        r16_unorm,
        r16_uint,
        r16_snorm,
        r16_sint,
        r8_typeless,
        [Description("L8")]
        r8_unorm,
        r8_uint,
        r8_snorm,
        r8_sint,
        [Description("A8")]
        a8_unorm,
        r1_unorm,
        r9g9b9e5_sharedexp,
        [Description("RGBG or GBGR")]
        r8g8_b8g8_unorm,
        g8r8_g8b8_unorm,
        bc1_typeless,
        [Description("DXT1")]
        bc1_unorm,
        [Description("DXT1")]
        bc1_unorm_srgb,
        bc2_typeless,
        [Description("DXT2")]
        bc2_unorm,
        [Description("DXT2")]
        bc2_unorm_srgb,
        bc3_typeless,
        [Description("DXT5")]
        bc3_unorm,
        [Description("DXT5")]
        bc3_unorm_srgb,
        bc4_typeless,
        [Description("BC4U")]
        bc4_unorm,
        [Description("BC4S")]
        bc4_snorm,
        bc5_typeless,
        [Description("ATI2 or BC5U")]
        bc5_unorm,
        [Description("BC5S")]
        bc5_snorm,
        b5g6r5_unorm,
        [Description("B5G5R5A1 A1R5G5B5")]
        b5g5r5a1_unorm,
        [Description("A8R8G8B8")]
        b8g8r8a8_unorm,
        [Description("X8R8G8B8")]
        b8g8r8x8_unorm,
        r10g10b10_xr_bias_a2_unorm,
        b8g8r8a8_typeless,
        [Description("A8B8G8R8")]
        b8g8r8a8_unorm_srgb,
        b8g8r8x8_typeless,
        [Description("X8B8G8R8")]
        b8g8r8x8_unorm_srgb,
        bc6h_typeless,
        bc6h_uf16,
        bc6h_sf16,
        bc7_typeless,
        bc7_unorm,
        bc7_unorm_srgb,
        ayuv,
        y410,
        y416,
        nv12,
        p010,
        p016,
        opaque,
        [Description("YUY2")]
        yuy2,
        y210,
        y216,
        nv11,
        ai44,
        ia44,
        p8,
        a8p8,
        [Description("A4R4G4B4")]
        b4g4r4a4_unorm,
        p208,
        v208,
        v408
    }
    public static class DDSCooker  //Todo 
    {



        public static uint BitsPerPixel(DDSFormat fmt)
        {
            switch (fmt)
            {
                case DDSFormat.r32g32b32a32_typeless:
                case DDSFormat.r32g32b32a32_float:
                case DDSFormat.r32g32b32a32_uint:
                case DDSFormat.r32g32b32a32_sint:
                    return 128;

                case DDSFormat.r32g32b32_typeless:
                case DDSFormat.r32g32b32_float:
                case DDSFormat.r32g32b32_uint:
                case DDSFormat.r32g32b32_sint:
                    return 96;

                case DDSFormat.r16g16b16a16_typeless:
                case DDSFormat.r16g16b16a16_float:
                case DDSFormat.r16g16b16a16_unorm:
                case DDSFormat.r16g16b16a16_uint:
                case DDSFormat.r16g16b16a16_snorm:
                case DDSFormat.r16g16b16a16_sint:
                case DDSFormat.r32g32_typeless:
                case DDSFormat.r32g32_float:
                case DDSFormat.r32g32_uint:
                case DDSFormat.r32g32_sint:
                case DDSFormat.r32g8x24_typeless:
                case DDSFormat.d32_float_s8x24_uint:
                case DDSFormat.r32_float_x8x24_typeless:
                case DDSFormat.x32_typeless_g8x24_uint:
                    return 64;

                case DDSFormat.r10g10b10a2_typeless:
                case DDSFormat.r10g10b10a2_unorm:
                case DDSFormat.r10g10b10a2_uint:
                case DDSFormat.r11g11b10_float:
                case DDSFormat.r8g8b8a8_typeless:
                case DDSFormat.r8g8b8a8_unorm:
                case DDSFormat.r8g8b8a8_unorm_srgb:
                case DDSFormat.r8g8b8a8_uint:
                case DDSFormat.r8g8b8a8_snorm:
                case DDSFormat.r8g8b8a8_sint:
                case DDSFormat.r16g16_typeless:
                case DDSFormat.r16g16_float:
                case DDSFormat.r16g16_unorm:
                case DDSFormat.r16g16_uint:
                case DDSFormat.r16g16_snorm:
                case DDSFormat.r16g16_sint:
                case DDSFormat.r32_typeless:
                case DDSFormat.d32_float:
                case DDSFormat.r32_float:
                case DDSFormat.r32_uint:
                case DDSFormat.r32_sint:
                case DDSFormat.r24g8_typeless:
                case DDSFormat.d24_unorm_s8_uint:
                case DDSFormat.r24_unorm_x8_typeless:
                case DDSFormat.x24_typeless_g8_uint:
                case DDSFormat.r9g9b9e5_sharedexp:
                case DDSFormat.r8g8_b8g8_unorm:
                case DDSFormat.g8r8_g8b8_unorm:
                case DDSFormat.b8g8r8a8_unorm:
                case DDSFormat.b8g8r8x8_unorm:
                case DDSFormat.r10g10b10_xr_bias_a2_unorm:
                case DDSFormat.b8g8r8a8_typeless:
                case DDSFormat.b8g8r8a8_unorm_srgb:
                case DDSFormat.b8g8r8x8_typeless:
                case DDSFormat.b8g8r8x8_unorm_srgb:
                    return 32;

                case DDSFormat.r8g8_typeless:
                case DDSFormat.r8g8_unorm:
                case DDSFormat.r8g8_uint:
                case DDSFormat.r8g8_snorm:
                case DDSFormat.r8g8_sint:
                case DDSFormat.r16_typeless:
                case DDSFormat.r16_float:
                case DDSFormat.d16_unorm:
                case DDSFormat.r16_unorm:
                case DDSFormat.r16_uint:
                case DDSFormat.r16_snorm:
                case DDSFormat.r16_sint:
                case DDSFormat.b5g6r5_unorm:
                case DDSFormat.b5g5r5a1_unorm:
                case DDSFormat.b4g4r4a4_unorm:
                    return 16;

                case DDSFormat.r8_typeless:
                case DDSFormat.r8_unorm:
                case DDSFormat.r8_uint:
                case DDSFormat.r8_snorm:
                case DDSFormat.r8_sint:
                case DDSFormat.a8_unorm:
                    return 8;

                case DDSFormat.r1_unorm:
                    return 1;

                case DDSFormat.bc1_typeless:
                case DDSFormat.bc1_unorm:
                case DDSFormat.bc1_unorm_srgb:
                case DDSFormat.bc4_typeless:
                case DDSFormat.bc4_unorm:
                case DDSFormat.bc4_snorm:
                    return 4;

                case DDSFormat.bc2_typeless:
                case DDSFormat.bc2_unorm:
                case DDSFormat.bc2_unorm_srgb:
                case DDSFormat.bc3_typeless:
                case DDSFormat.bc3_unorm:
                case DDSFormat.bc3_unorm_srgb:
                case DDSFormat.bc5_typeless:
                case DDSFormat.bc5_unorm:
                case DDSFormat.bc5_snorm:
                case DDSFormat.bc6h_typeless:
                case DDSFormat.bc6h_uf16:
                case DDSFormat.bc6h_sf16:
                case DDSFormat.bc7_typeless:
                case DDSFormat.bc7_unorm:
                case DDSFormat.bc7_unorm_srgb:
                    return 8;

                default:
                    return 0;
            }
        }


        public static (uint, uint, uint, uint) GetMask(DDSFormat format)
        {
            switch (format)
            {
                //RGBBitCount=32
                case DDSFormat.r8g8b8a8_unorm:
                    return (0x000000ff, 0x0000ff00, 0x00ff0000, 0xff000000);

                case DDSFormat.r10g10b10a2_unorm:
                    return (0x3ff00000, 0x000ffc00, 0x000003ff, 0xc0000000);

                case DDSFormat.r16g16_unorm:
                    return (0x0000ffff, 0xffff0000, 0x00000000, 0x00000000);

                case DDSFormat.r32_float:
                    return (0xffffffff, 0x00000000, 0x00000000, 0x00000000);

                case DDSFormat.b8g8r8a8_unorm:
                    return (0x00ff0000, 0x0000ff00, 0x000000ff, 0xff000000);

                case DDSFormat.b8g8r8x8_unorm:
                    return (0x00ff0000, 0x0000ff00, 0x000000ff, 0);

                case DDSFormat.b8g8r8x8_unorm_srgb:
                    return (0x000000ff, 0x0000ff00, 0x00ff0000, 0);




                //RGBBitCount=16
                case DDSFormat.b5g5r5a1_unorm:
                    return (0x7C00, 0x3E0, 0x1F, 0x8000);
                case DDSFormat.b5g6r5_unorm:
                    return (0xf8000000, 0x07e00000, 0x001f0000, 0x00000000);
                case DDSFormat.b4g4r4a4_unorm:
                    return (0x0f000000, 0x00f00000, 0x000f0000, 0xf0000000);
                case DDSFormat.r16_unorm:
                    return (0x0000ffff, 0x00000000, 0x00000000, 0x00000000);
                case DDSFormat.r8g8_unorm:
                    return (0xff, 0x00000000, 0x00000000, 0xFF00);


                //RGBBitCount=8

                case DDSFormat.r8_unorm:
                    return (0xff, 0x0, 0x0, 0x0);

                case DDSFormat.a8_unorm:
                    return (0x0, 0x0, 0x0, 0xff);

                default:
                    return (0x00000000, 0x00000000, 0x00000000, 0x00000000);


            }
        }




        [Flags]
        public enum DDSPixelFormatFlags
        {
            D3D10_RESOURCE_DIMENSION_UNKNOWN = 0,
            DDSCAPS_COMPLEX = 0x8,
            DDSCAPS_MIPMAP = 0x400000,
            DDSCAPS_TEXTURE = 0x1000
        }

        [Flags]
        enum D3D10_RESOURCE_DIMENSION
        {
            D3D10_RESOURCE_DIMENSION_UNKNOWN = 0,
            D3D10_RESOURCE_DIMENSION_BUFFER = 1,
            D3D10_RESOURCE_DIMENSION_TEXTURE1D = 2,
            D3D10_RESOURCE_DIMENSION_TEXTURE2D = 3,
            D3D10_RESOURCE_DIMENSION_TEXTURE3D = 4
        }


        [Flags]
        public enum HeaderFlags
        {
            /// <summary>
            /// Required in every .dds file.
            /// </summary>
            DDSD_CAPS = 0x1,
            /// <summary>
            /// Required in every .dds file.
            /// </summary>
            DDSD_HEIGHT = 0x2,
            /// <summary>
            /// Required in every .dds file.
            /// </summary>
            DDSD_WIDTH = 0x4,
            /// <summary>
            /// Required when pitch is provided for an uncompressed texture.
            /// </summary>
            DDSD_PITCH = 0x8,
            /// <summary>
            /// Required in every .dds file.
            /// </summary>
            DDSD_PIXELFORMAT = 0x1000,
            /// <summary>
            /// Required in a mipmapped texture.
            /// </summary>
            DDSD_MIPMAPCOUNT = 0x20000,
            /// <summary>
            /// Required when pitch is provided for a compressed texture.
            /// </summary>
            DDSD_LINEARSIZE = 0x80000,
            /// <summary>
            /// Required in a depth texture.
            /// </summary>
            DDSD_DEPTH = 0x800000
        }

        [Flags]
        public enum DDSCAPS
        {
            /// <summary>
            /// Optional; must be used on any file that contains more than one surface (a mipmap, a cubic environment map, or mipmapped volume texture).
            /// </summary>
            DDSCAPS_COMPLEX = 0x8,
            /// <summary>
            /// Required
            /// </summary>
            DDSCAPS_TEXTURE = 0x1000,
            /// <summary>
            /// Optional; should be used for a mipmap.	
            /// </summary>
            DDSCAPS_MIPMAP = 0x400000
        }

        [Flags]
        public enum DDSCAPS2
        {
            /// <summary>
            ///  Required for a cube map.
            /// </summary>
            DDSCAPS2_CUBEMAP = 0x200,
            /// <summary>
            /// Required when these surfaces are stored in a cube map.
            /// </summary>
            DDSCAPS2_CUBEMAP_POSITIVEX = 0x400,
            /// <summary>
            /// Required when these surfaces are stored in a cube map.
            /// </summary>
            DDSCAPS2_CUBEMAP_NEGATIVEX = 0x800,
            /// <summary>
            /// Required when these surfaces are stored in a cube map.
            /// </summary>
            DDSCAPS2_CUBEMAP_POSITIVEY = 0x1000,
            /// <summary>
            /// Required when these surfaces are stored in a cube map.
            /// </summary>
            DDSCAPS2_CUBEMAP_NEGATIVEY = 0x2000,
            /// <summary>
            /// Required when these surfaces are stored in a cube map.
            /// </summary>
            DDSCAPS2_CUBEMAP_POSITIVEZ = 0x4000,
            /// <summary>
            /// Required when these surfaces are stored in a cube map.
            /// </summary>
            DDSCAPS2_CUBEMAP_NEGATIVEZ = 0x8000,
            /// <summary>
            /// Required for a volume texture.
            /// </summary>
            DDSCAPS2_VOLUME = 0x200000
        }


        public enum DDPF
        {
            /// <summary>
            /// Texture contains alpha data; dwRGBAlphaBitMask contains valid data.
            /// </summary>
            DDPF_ALPHAPIXELS = 0x1,
            /// <summary>
            /// Used in some older DDS files for alpha channel only uncompressed data (dwRGBBitCount contains the alpha channel bitcount; dwABitMask contains valid data)
            /// </summary>
            DDPF_ALPHA = 0x2,
            /// <summary>
            /// Texture contains compressed RGB data; dwFourCC contains valid data.
            /// </summary>
            DDPF_FOURCC = 0x4,
            /// <summary>
            /// Texture contains uncompressed RGB data; dwRGBBitCount and the RGB masks(dwRBitMask, dwGBitMask, dwBBitMask) contain valid data.
            /// </summary>
            DDPF_RGB = 0x40,
            /// <summary>
            /// Used in some older DDS files for YUV uncompressed data(dwRGBBitCount contains the YUV bit count; dwRBitMask contains the Y mask, dwGBitMask contains the U mask, dwBBitMask contains the V mask)	
            /// </summary>
            DDPF_YUV = 0x200,
            /// <summary>
            /// Used in some older DDS files for single channel color uncompressed data(dwRGBBitCount contains the luminance channel bit count; dwRBitMask contains the channel mask). Can be combined with DDPF_ALPHAPIXELS for a two channel DDS file.
            /// </summary>
            DDPF_LUMINANCEA = 0x20001, //DDPF_LUMINANCE| DDPF_ALPHAPIXELS
            DDPF_LUMINANCE = 0x20000,


            DDPF_RGBA = 0x41,          //DDPF_RGB | DDPF_ALPHAPIXELS
            DDPF_PAL8 = 0x20,
            DDPF_PAL8A = 0x21,          //DDPF_PAL8 | DDPF_ALPHAPIXELS
            DDPF_BUMPDUDV = 0x80000
        }

        enum DDS_DIMENSION
        {
            /// <summary>
            /// (D3D10_RESOURCE_DIMENSION_TEXTURE1D) Resource is a 1D texture.The dwWidth member of DDS_HEADER specifies the size of the texture.Typically, you set the dwHeight member of DDS_HEADER to 1; you also must set the DDSD_HEIGHT flag in the dwFlags member of DDS_HEADER.  
            /// </summary>
            DDS_DIMENSION_TEXTURE1D = 2,
            /// <summary>
            /// (D3D10_RESOURCE_DIMENSION_TEXTURE2D)    Resource is a 2D texture with an area specified by the dwWidth and dwHeight members of DDS_HEADER.You can also use this type to identify a cube-map texture.For more information about how to identify a cube-map texture, see miscFlag and arraySize members.    
            /// </summary>
            DDS_DIMENSION_TEXTURE2D = 3,
            /// <summary>
            /// (D3D10_RESOURCE_DIMENSION_TEXTURE3D) Resource is a 3D texture with a volume specified by the dwWidth, dwHeight, and dwDepth members of DDS_HEADER.You also must set the DDSD_DEPTH flag in the dwFlags member of DDS_HEADER.   
            /// </summary>
            DDS_DIMENSION_TEXTURE3D = 4
        }


        /// <summary>
        /// Indicates a 2D texture is a cube-map texture.
        /// </summary>
        static uint DDS_RESOURCE_MISC_TEXTURECUBE = 0x4;



        enum DDS_ALPHA_MODE
        {
            /// <summary>
            /// Alpha channel content is unknown. This is the value for legacy files, which typically is assumed to be 'straight' alpha.	
            /// </summary>
            DDS_ALPHA_MODE_UNKNOWN = 0x0,
            /// <summary>
            /// Any alpha channel content is presumed to use straight alpha.
            /// </summary>
            DDS_ALPHA_MODE_STRAIGHT = 0x1,
            /// <summary>
            /// Any alpha channel content is using premultiplied alpha.The only legacy file formats that indicate this information are 'DX2' and 'DX4'.	
            /// </summary>
            DDS_ALPHA_MODE_PREMULTIPLIED = 0x2,
            /// <summary>
            /// Any alpha channel content is all set to fully opaque.
            /// </summary>
            DDS_ALPHA_MODE_OPAQUE = 0x3,
            /// <summary>
            ///  Any alpha channel content is being used as a 4th channel and is not intended to represent transparency (straight or premultiplied).
            /// </summary>
            DDS_ALPHA_MODE_CUSTOM = 0x4,
        }






        public static byte[] TexToDds(byte[] bytes, DDSFormat format, int height, int width, int mipmap = 0, int depth = 0, bool UseDX10 = false)
        {
            MStream Tex = new MStream(bytes);
            Tex.SetStringValue("DDS ");
            Tex.SetIntValue(124);//block size
            HeaderFlags flags = HeaderFlags.DDSD_CAPS | HeaderFlags.DDSD_HEIGHT | HeaderFlags.DDSD_WIDTH | HeaderFlags.DDSD_PIXELFORMAT;


            if (depth != 0)
            {
                flags |= HeaderFlags.DDSD_DEPTH;
            }

            if (mipmap > 1)
            {
                flags |= HeaderFlags.DDSD_MIPMAPCOUNT;
            }

            //if (format == Format.bc1_unorm || format == Format.bc2_unorm || format == Format.bc3_unorm)
            //{
            //    flags |= HeaderFlags.DDSD_LINEARSIZE;
            //}

            Tex.SetUIntValue((uint)flags);
            Tex.SetIntValue(height);
            Tex.SetIntValue(width);
            Tex.SetIntValue(0); //pitchOrLinearSize
            Tex.SetIntValue(depth);
            Tex.SetIntValue(mipmap);

            Tex.Skip(44); //reserved1

            Tex.SetIntValue(32); //block size        
            DDPF dwFlags = 0;
            if (format == DDSFormat.bc1_unorm || format == DDSFormat.bc2_unorm || format == DDSFormat.bc3_unorm || UseDX10)
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

            //   dwFlags//
            Tex.SetUIntValue((uint)dwFlags); //block size

            bool ISDX10 = false;
            if (format == DDSFormat.bc1_unorm)
            {
                Tex.SetStringValue("DXT1");
            }
            else if (format == DDSFormat.bc2_unorm)
            {
                Tex.SetStringValue("DXT3");
            }
            else if (format == DDSFormat.bc3_unorm)
            {
                Tex.SetStringValue("DXT5");
            }
            else if (format == DDSFormat.bc4_unorm)
            {
                Tex.SetStringValue("ATI1");
            }
            else if (format == DDSFormat.bc5_unorm)
            {
                Tex.SetStringValue("ATI2");
            }
            else
            {
                if (!UseDX10)
                {
                    Tex.SetIntValue(0);
                }
                else
                {
                    Tex.SetStringValue("DX10");
                    ISDX10 = true;
                }
            }

            uint RGBBitCount = 0;
            uint RBitMask = 0;
            uint GBitMask = 0;
            uint BBitMask = 0;
            uint ABitMask = 0;

            if (!ISDX10)
            {
                RGBBitCount = BitsPerPixel(format);
                (RBitMask, GBitMask, BBitMask, ABitMask) = GetMask(format);
            }

            Tex.SetUIntValue(RGBBitCount); //RGBBitCount
            Tex.SetUIntValue(RBitMask); //RBitMask
            Tex.SetUIntValue(GBitMask); //GBitMask
            Tex.SetUIntValue(BBitMask); //BBitMask
            Tex.SetUIntValue(ABitMask); //ABitMask

            DDSCAPS Caps = DDSCAPS.DDSCAPS_TEXTURE;

            if (mipmap > 1)
            {
                Caps |= DDSCAPS.DDSCAPS_MIPMAP;
            }

            if (mipmap > 1 || depth > 1)
            {
                Caps |= DDSCAPS.DDSCAPS_COMPLEX;
            }

            Tex.SetUIntValue((uint)Caps);

            DDSCAPS2 Caps2 = 0;
            Tex.SetUIntValue((uint)Caps2);
            Tex.SetUIntValue(0);//Caps3
            Tex.SetUIntValue(0);//Caps4
            Tex.SetUIntValue(0);//reserved2

            if (ISDX10)
            {
                Tex.SetUIntValue((uint)format);//DXGI_FORMAT
                Tex.SetUIntValue((uint)DDS_DIMENSION.DDS_DIMENSION_TEXTURE2D);//D3D10_RESOURCE_DIMENSION 
                Tex.SetUIntValue(0);//miscFlag 
                Tex.SetUIntValue(1);//arraySize //For a 3D texture, you must set this number to 1. 
                Tex.SetUIntValue((uint)DDS_ALPHA_MODE.DDS_ALPHA_MODE_UNKNOWN);//miscFlags2 
            }

            Tex.SetBytes(bytes);

            return Tex.ToArray();
        }




        public struct DDS_HEADER
        {
            public uint Magic;
            public uint size;
            [MarshalAs(UnmanagedType.U4)]
            public HeaderFlags flags;
            public int height;
            public int width;
            public int pitchOrLinearSize;
            public int depth;
            public int mipMapCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
            public uint[] reserved1;
            public DDS_PIXELFORMAT ddspf;
            [MarshalAs(UnmanagedType.U4)]
            public DDSCAPS caps;
            [MarshalAs(UnmanagedType.U4)]
            public DDSCAPS2 caps2;
            public uint caps3;
            public uint caps4;
            public uint reserved2;

        }


        public struct DDS_PIXELFORMAT
        {
            public int size;
            [MarshalAs(UnmanagedType.U4)]
            public DDPF flags;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] fourCC;
            public uint RGBBitCount;
            public uint RBitMask;
            public uint GBitMask;
            public uint BBitMask;
            public uint ABitMask;
        }


        public struct dxt10_header
        {
            [MarshalAs(UnmanagedType.U4)]
            public DDSFormat dxgiFormat;
            public uint resourceDimension;
            public uint miscFlag;
            public uint arraySize;
            public uint miscFlags2;
        }

        public struct DDS_Info
        {
            public DDSFormat Format { get; set; }
            public DDS_HEADER DDS_HEADER { get; set; }
            public dxt10_header dxt10_Header { get; set; }
            public byte[] Data { get; set; }
        }



        public static DDS_Info DdsToTex(byte[] bytes)
        {

            MStream memoryList = new MStream(bytes);

            if (memoryList.GetIntValue(false) != 0x20534444)
            {
                throw new Exception("Invalid dds file");
            }

            DDS_Info dds_info = new DDS_Info();
            dds_info.DDS_HEADER = memoryList.GetStructureValues<DDS_HEADER>();
            if (BitConverter.ToInt32(dds_info.DDS_HEADER.ddspf.fourCC, 0) == 0x30315844)
            {
                dds_info.dxt10_Header = memoryList.GetStructureValues<dxt10_header>();
                dds_info.Format = dds_info.dxt10_Header.dxgiFormat;
            }
            dds_info.Data = memoryList.GetBytes((int)(memoryList.GetSize() - memoryList.GetPosition()));


            if (dds_info.Format == default)
            {

                dds_info.Format = GetFormat(dds_info.DDS_HEADER.ddspf);
            }
#if false
            Console.WriteLine("dxgiFormat: " + dds_info.Format.ToString());
            Console.WriteLine("RGBBitCount: " + dds_info.DDS_HEADER.ddspf.RGBBitCount);
            Console.WriteLine("Has Flag: " + dds_info.DDS_HEADER.ddspf.flags.Equals(DDPF.DDPF_RGBA));
            Console.WriteLine("Has RBitMask: " + dds_info.DDS_HEADER.ddspf.RBitMask);
            Console.WriteLine("Has GBitMask: " + dds_info.DDS_HEADER.ddspf.GBitMask);
            Console.WriteLine("Has BBitMask: " + dds_info.DDS_HEADER.ddspf.BBitMask);
            Console.WriteLine("Has ABitMask: " + dds_info.DDS_HEADER.ddspf.ABitMask);
            Console.WriteLine("width: " + dds_info.DDS_HEADER.width);
            Console.WriteLine("height: " + dds_info.DDS_HEADER.height);
            Console.WriteLine("flags: " + dds_info.DDS_HEADER.ddspf.flags.ToString());
            Console.WriteLine("caps: " + dds_info.DDS_HEADER.caps.ToString());
            Console.WriteLine("caps2: " + dds_info.DDS_HEADER.caps2.ToString());
#endif
            return dds_info;
        }


        /// <summary>
        /// this function is for stream and return DDS_Info.Data
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static DDS_Info DdsToTex(IStream stream)
        {

            if (stream.GetIntValue(false) != 0x20534444)
            {
                throw new Exception("Invalid dds file");
            }

            DDS_Info dds_info = new DDS_Info();
            dds_info.DDS_HEADER = stream.GetStructureValues<DDS_HEADER>();
            if (BitConverter.ToInt32(dds_info.DDS_HEADER.ddspf.fourCC, 0) == 0x30315844)
            {
                dds_info.dxt10_Header = stream.GetStructureValues<dxt10_header>();
                dds_info.Format = dds_info.dxt10_Header.dxgiFormat;
            }

            if (dds_info.Format == default)
            {

                dds_info.Format = GetFormat(dds_info.DDS_HEADER.ddspf);
            }
#if false
            Console.WriteLine("dxgiFormat: " + dds_info.Format.ToString());
            Console.WriteLine("RGBBitCount: " + dds_info.DDS_HEADER.ddspf.RGBBitCount);
            Console.WriteLine("Has Flag: " + dds_info.DDS_HEADER.ddspf.flags.Equals(DDPF.DDPF_RGBA));
            Console.WriteLine("Has RBitMask: " + dds_info.DDS_HEADER.ddspf.RBitMask);
            Console.WriteLine("Has GBitMask: " + dds_info.DDS_HEADER.ddspf.GBitMask);
            Console.WriteLine("Has BBitMask: " + dds_info.DDS_HEADER.ddspf.BBitMask);
            Console.WriteLine("Has ABitMask: " + dds_info.DDS_HEADER.ddspf.ABitMask);
            Console.WriteLine("width: " + dds_info.DDS_HEADER.width);
            Console.WriteLine("height: " + dds_info.DDS_HEADER.height);
            Console.WriteLine("flags: " + dds_info.DDS_HEADER.ddspf.flags.ToString());
            Console.WriteLine("caps: " + dds_info.DDS_HEADER.caps.ToString());
            Console.WriteLine("caps2: " + dds_info.DDS_HEADER.caps2.ToString());
#endif
            return dds_info;
        }



        static bool ISSameMask(DDS_PIXELFORMAT ddspf, uint RBitMask, uint GBitMask, uint BBitMask, uint ABitMask)
        {
            return ddspf.RBitMask == RBitMask &&
                   ddspf.GBitMask == GBitMask &&
                   ddspf.BBitMask == BBitMask &&
                   ddspf.ABitMask == ABitMask;
        }


        private static DDSFormat GetFormat(DDS_PIXELFORMAT ddspf)
        {



            if (ddspf.flags > 0 && (ddspf.flags.Equals(DDPF.DDPF_RGB) || ddspf.flags.Equals(DDPF.DDPF_RGBA)))
            {

                switch (ddspf.RGBBitCount)
                {

                    case 32:
                        {
                            if (ISSameMask(ddspf, 0x000000ff, 0x0000ff00, 0x00ff0000, 0xff000000))
                            {
                                return DDSFormat.r8g8b8a8_unorm;
                            }

                            if (ISSameMask(ddspf, 0x3ff00000, 0x000ffc00, 0x000003ff, 0xc0000000))
                            {
                                return DDSFormat.r10g10b10a2_unorm;
                            }

                            if (ISSameMask(ddspf, 0x0000ffff, 0xffff0000, 0x00000000, 0x00000000))
                            {
                                return DDSFormat.r16g16_unorm;
                            }

                            if (ISSameMask(ddspf, 0xffffffff, 0x00000000, 0x00000000, 0x00000000))
                            {
                                return DDSFormat.r32_float;
                            }

                            if (ISSameMask(ddspf, 0x00ff0000, 0x0000ff00, 0x000000ff, 0))
                            {
                                return DDSFormat.b8g8r8x8_unorm;
                            }

                            if (ISSameMask(ddspf, 0x000000ff, 0x0000ff00, 0x00ff0000, 0))
                            {
                                return DDSFormat.b8g8r8x8_unorm_srgb;
                            }

                            if (ISSameMask(ddspf, 0x00ff0000, 0x0000ff00, 0x000000ff, 0xff000000))
                            {

                                return DDSFormat.b8g8r8a8_unorm;
                            }

                            if (ISSameMask(ddspf, 0x000000ff, 0x0000ff00, 0x00ff0000, 0xff000000))
                            {
                                return DDSFormat.b8g8r8a8_unorm_srgb;
                            }
                            break;
                        }

                    case 24:

                        if (ISSameMask(ddspf, 0xff0000, 0x00ff00, 0x0000ff, 0))
                        {
                            return DDSFormat.unknown;   //R8G8B8
                        }
                        break;
                    case 16:
                        {


                            if (ISSameMask(ddspf, 0x7C00, 0x3E0, 0x1F, 0x8000))
                            {
                                return DDSFormat.b5g5r5a1_unorm;
                            }

                            if (ISSameMask(ddspf, 0xf8000000, 0x07e00000, 0x001f0000, 0x00000000))
                            {
                                return DDSFormat.b5g6r5_unorm;
                            }

                            if (ISSameMask(ddspf, 0x0f000000, 0x00f00000, 0x000f0000, 0xf0000000))
                            {
                                return DDSFormat.b4g4r4a4_unorm;
                            }

                            if (ISSameMask(ddspf, 0x00ff, 0, 0, 0xff00))
                            {
                                return DDSFormat.r8g8_uint;
                            }
                            break;
                        }
                }
            }
            else if (ddspf.flags > 0 && (ddspf.flags.Equals(DDPF.DDPF_LUMINANCE) || ddspf.flags.Equals(DDPF.DDPF_LUMINANCEA)))
            {
                switch (ddspf.RGBBitCount)
                {
                    case 8:
                        {
                            if (ISSameMask(ddspf, 0x000000ff, 0x00000000, 0x00000000, 0x00000000))
                            {
                                return DDSFormat.r8_unorm;
                            }
                            if (ISSameMask(ddspf, 0xff, 0, 0, 0))
                            {
                                return DDSFormat.r8_uint;
                            }
                            break;
                        }
                    case 16:
                        {
                            if (ISSameMask(ddspf, 0x0000ffff, 0x00000000, 0x00000000, 0x00000000))
                            {
                                return DDSFormat.r16_unorm;
                            }

                            if (ISSameMask(ddspf, 0xFF, 0xFF00, 0x0, 0x0))
                            {
                                return DDSFormat.r8g8_unorm;
                            }

                            if (ISSameMask(ddspf, 0xff, 0x00000000, 0x00000000, 0xFF00))
                            {
                                return DDSFormat.r8g8_unorm;
                            }


                            if (ISSameMask(ddspf, 0xFF, 0xFF00, 0x0, 0x0))
                            {
                                return DDSFormat.r8g8_unorm;
                            }
                            break;
                        }


                }


            }
            else if (ddspf.flags > 0 && ddspf.flags.Equals(DDPF.DDPF_ALPHA))
            {
                if (ddspf.RGBBitCount == 8)
                {
                    return DDSFormat.a8_unorm;
                }
            }

            else if (ddspf.flags > 0 && ddspf.flags.Equals(DDPF.DDPF_BUMPDUDV))
            {

                switch (ddspf.RGBBitCount)
                {
                    case 16:
                        {
                            if (ISSameMask(ddspf, 0x00ff, 0xff00, 0, 0))
                            {
                                return DDSFormat.r8g8_snorm;
                            }

                            break;
                        }
                }
            }
            else if (ddspf.flags > 0 && ddspf.flags.Equals(DDPF.DDPF_FOURCC))
            {
                switch (Encoding.ASCII.GetString(ddspf.fourCC))
                {
                    case "DXT1":
                        return DDSFormat.bc1_unorm;
                    case "DXT3":
                        return DDSFormat.bc2_unorm;
                    case "DXT5":
                        return DDSFormat.bc3_unorm;
                    case "BC4U":
                        return DDSFormat.bc4_unorm;
                    case "BC4S":
                        return DDSFormat.bc4_snorm;
                    case "RGBG":
                    case "GBGR":
                        return DDSFormat.r8g8_b8g8_unorm;
                    case "BC5U":
                    case "ATI2":
                        return DDSFormat.r8g8_b8g8_unorm;
                    case "BC5S":
                        return DDSFormat.bc5_snorm;
                    case "YUY2":
                        return DDSFormat.yuy2;

                }
            }
            return DDSFormat.unknown;
        }
    }
}
