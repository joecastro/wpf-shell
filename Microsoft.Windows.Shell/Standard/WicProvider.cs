// wincodec.idl
namespace Standard
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Text;

    using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;
    using STATSTG = System.Runtime.InteropServices.ComTypes.STATSTG;

    #region WIC Values
    
    internal static class WicValues
    {
        public const uint WINCODEC_SDK_VERSION = 0x0236;
        public static readonly Guid GUID_VendorMicrosoft = new Guid(0xf0e749ca, 0xedef, 0x4589, 0xa7, 0x3a, 0xee, 0xe, 0x62, 0x6a, 0x2a, 0x2b);
    }

    /// <summary>
    /// GUID Identifiers for the image container formats
    /// </summary>
    internal static class WicGUID
    {
        /// <summary>GUID_ContainerFormatBmp</summary>
        public const string ContainerFormatBmp =  "0af1d87e-fcfe-4188-bdeb-a7906471cbe3";
        /// <summary>GUID_ContainerFormatPng</summary>
        public const string ContainerFormatPng =  "1b7cfaf4-713f-473c-bbcd-6137425faeaf";
        /// <summary>GUID_ContainerFormatIco</summary>
        public const string ContainerFormatIco =  "a3a860c4-338f-4c17-919a-fba4b5628f21";
        /// <summary>GUID_ContainerFormatJpeg</summary>
        public const string ContainerFormatJpeg = "19e4a5aa-5662-4fc5-a0c0-1758028e1057";
        /// <summary>GUID_ContainerFormatTiff</summary>
        public const string ContainerFormatTiff = "163bcc30-e2e9-4f0b-961d-a3e9fdb788a3";
        /// <summary>GUID_ContainerFormatGif</summary>
        public const string ContainerFormatGif =  "1f8a5601-7d4d-4cbd-9c82-1bc8d4eeb9a5";
        /// <summary>GUID_ContainerFormatWmp</summary>
        public const string ContainerFormatWmp =  "57a37caa-367a-4540-916b-f183c5093a4b";
    }

    /// <summary>
    /// WIC Category Identifiers
    /// </summary>
    internal static class WicCATID
    {
        /// <summary>CATID_WICBitmapDecoders</summary>
        public const string WICBitmapDecoders   = "7ed96837-96f0-4812-b211-f13c24117ed3";
        /// <summary>CATID_WICBitmapEncoders</summary>
        public const string WICBitmapEncoders = "ac757296-3522-4e11-9862-c17be5a1767e";
        /// <summary>CATID_WICPixelFormats</summary>
        public const string WICPixelFormats = "2b46e70f-cda7-473e-89f6-dc9630a2390b";
        /// <summary>CATID_WICFormatConverters</summary>
        public const string WICFormatConverters = "7835eae8-bf14-49d1-93ce-533a407b2248";
        /// <summary>CATID_WICMetadataReader</summary>
        public const string WICMetadataReader = "05af94d8-7174-4cd2-be4a-4124b80ee4b8";
        /// <summary>CATID_WICMetadataWriter</summary>
        public const string WICMetadataWriter = "abe3b9a4-257d-4b97-bd1a-294af496222e";
    }

    internal static class WicCLSID
    {
        #region (WIC) GUID identifiers for the codecs

        /// <summary>CLSID_WICBmpDecoder</summary>
        public const string WICBmpDecoder = "6b462062-7cbf-400d-9fdb-813dd10f2778";
        /// <summary>CLSID_WICPngDecoder</summary>
        public const string WICPngDecoder = "389ea17b-5078-4cde-b6ef-25c15175c751";
        /// <summary>CLSID_WICIcoDecoder</summary>
        public const string WICIcoDecoder = "c61bfcdf-2e0f-4aad-a8d7-e06bafebcdfe";
        /// <summary>CLSID_WICJpegDecoder</summary>
        public const string WICJpegDecoder = "9456a480-e88b-43ea-9e73-0b2d9b71b1ca";
        /// <summary>CLSID_WICGifDecoder</summary>
        public const string WICGifDecoder = "381dda3c-9ce9-4834-a23e-1f98f8fc52be";
        /// <summary>CLSID_WICTiffDecoder</summary>
        public const string WICTiffDecoder = "b54e85d9-fe23-499f-8b88-6acea713752b";
        /// <summary>CLSID_WICWmpDecoder</summary>
        public const string WICWmpDecoder = "a26cec36-234c-4950-ae16-e34aace71d0d";

        /// <summary>CLSID_WICBmpEncoder</summary>
        public const string WICBmpEncoder = "69be8bb4-d66d-47c8-865a-ed1589433782";
        /// <summary>CLSID_WICPngEncoder</summary>
        public const string WICPngEncoder = "27949969-876a-41d7-9447-568f6a35a4dc";
        /// <summary>CLSID_WICJpegEncoder</summary>
        public const string WICJpegEncoder = "1a34f5c1-4a5a-46dc-b644-1f4567e7a676";
        /// <summary>CLSID_WICGifEncoder</summary>
        public const string WICGifEncoder = "114f5598-0b22-40a0-86a1-c83ea495adbd";
        /// <summary>CLSID_WICTiffEncoder</summary>
        public const string WICTiffEncoder = "0131be10-2001-4c5f-a9b0-cc88fab64ce8";
        /// <summary>CLSID_WICWmpEncoder</summary>
        public const string WICWmpEncoder = "ac4ce3cb-e1c1-44cd-8215-5a1665509ec2";

        #endregion

        #region Category Identifiers

        /// <summary>CLSID_WICImagingCategories</summary>
        public const string WICImagingCategories = "fae3d380-fea4-4623-8c75-c6b61110b681";

        #endregion
        
        #region Format converters

        /// <summary>CLSID_WICDefaultFormatConverter</summary>
        public const string WICDefaultFormatConverter = "1a3f11dc-b514-4b17-8c5f-2154513852f1";
        /// <summary>CLSID_WICFormatConverterNChannel</summary>
        public const string WICFormatConverterNChannel = "c17cabb2-d4a3-47d7-a557-339b2efbd4f1";
        /// <summary>CLSID_WICFormatConverterWMPhoto</summary>
        public const string WICFormatConverterWMPhoto = "9cb5172b-d600-46ba-ab77-77bb7e3a00d9";
        
        #endregion
    }

    /// <summary>Pixel format GUIDs.</summary>
    internal static class WICPixelFormat
    {
        /* Undefined formats */
        public static readonly Guid WICPixelFormatDontCare = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x00);
        public static readonly Guid WICPixelFormatUndefined = WICPixelFormatDontCare;

        /* Indexed formats */
        public static readonly Guid WICPixelFormat1bppIndexed = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x01);
        public static readonly Guid WICPixelFormat2bppIndexed = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x02);
        public static readonly Guid WICPixelFormat4bppIndexed = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x03);
        public static readonly Guid WICPixelFormat8bppIndexed = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x04);

        public static readonly Guid WICPixelFormatBlackWhite = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x05);
        public static readonly Guid WICPixelFormat2bppGray = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x06);
        public static readonly Guid WICPixelFormat4bppGray = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x07);
        public static readonly Guid WICPixelFormat8bppGray = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x08);

        /* sRGB formats (gamma is approx. 2.2) */
        /* For a full definition, see the sRGB spec */

        /* 16bpp formats */
        public static readonly Guid WICPixelFormat16bppBGR555 = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x09);
        public static readonly Guid WICPixelFormat16bppBGR565 = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x0a);
        public static readonly Guid WICPixelFormat16bppGray = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x0b);

        /* 24bpp formats */
        public static readonly Guid WICPixelFormat24bppBGR = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x0c);
        public static readonly Guid WICPixelFormat24bppRGB = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x0d);

        /* 32bpp format */
        public static readonly Guid WICPixelFormat32bppBGR = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x0e);
        public static readonly Guid WICPixelFormat32bppBGRA = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x0f);
        public static readonly Guid WICPixelFormat32bppPBGRA = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x10);
        public static readonly Guid WICPixelFormat32bppGrayFloat = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x11);

        /* 48bpp format */
        public static readonly Guid WICPixelFormat48bppRGBFixedPoint = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x12);

        /* scRGB formats. Gamma is 1.0 */
        /* For a full definition, see the scRGB spec */

        /* 16bpp format */
        public static readonly Guid WICPixelFormat16bppGrayFixedPoint = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x13);

        /* 32bpp format */
        public static readonly Guid WICPixelFormat32bppBGR101010 = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x14);

        /* 48bpp format */
        public static readonly Guid WICPixelFormat48bppRGB = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x15);

        /* 64bpp format */
        public static readonly Guid WICPixelFormat64bppRGBA = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x16);
        public static readonly Guid WICPixelFormat64bppPRGBA = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x17);

        /* 96bpp format */
        public static readonly Guid WICPixelFormat96bppRGBFixedPoint = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x18);

         /* Floating point scRGB formats */
        public static readonly Guid WICPixelFormat128bppRGBAFloat = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x19);
        public static readonly Guid WICPixelFormat128bppPRGBAFloat = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x1a);
        public static readonly Guid WICPixelFormat128bppRGBFloat = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x1b);

         /* CMYK formats. */
        public static readonly Guid WICPixelFormat32bppCMYK = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x1c);

         /* Photon formats */
        public static readonly Guid WICPixelFormat64bppRGBAFixedPoint = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x1d);
        public static readonly Guid WICPixelFormat64bppRGBFixedPoint = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x40);
        public static readonly Guid WICPixelFormat128bppRGBAFixedPoint = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x1e);
        public static readonly Guid WICPixelFormat128bppRGBFixedPoint = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x41);

        public static readonly Guid WICPixelFormat64bppRGBAHalf = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x3a);
        public static readonly Guid WICPixelFormat64bppRGBHalf = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x42);
        public static readonly Guid WICPixelFormat48bppRGBHalf = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x3b);

        public static readonly Guid WICPixelFormat32bppRGBE = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x3d);

        public static readonly Guid WICPixelFormat16bppGrayHalf = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x3e);
        public static readonly Guid WICPixelFormat32bppGrayFixedPoint = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x3f);

        /* More CMYK formats and n-Channel formats */
        public static readonly Guid WICPixelFormat64bppCMYK = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x1f);

        public static readonly Guid WICPixelFormat24bpp3Channels = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x20);
        public static readonly Guid WICPixelFormat32bpp4Channels = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x21);
        public static readonly Guid WICPixelFormat40bpp5Channels = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x22);
        public static readonly Guid WICPixelFormat48bpp6Channels = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x23);
        public static readonly Guid WICPixelFormat56bpp7Channels = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x24);
        public static readonly Guid WICPixelFormat64bpp8Channels = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x25);

        public static readonly Guid WICPixelFormat48bpp3Channels = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x26);
        public static readonly Guid WICPixelFormat64bpp4Channels = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x27);
        public static readonly Guid WICPixelFormat80bpp5Channels = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x28);
        public static readonly Guid WICPixelFormat96bpp6Channels = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x29);
        public static readonly Guid WICPixelFormat112bpp7Channels = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x2a);
        public static readonly Guid WICPixelFormat128bpp8Channels = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x2b);

        public static readonly Guid WICPixelFormat40bppCMYKAlpha = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x2c);
        public static readonly Guid WICPixelFormat80bppCMYKAlpha = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x2d);

        public static readonly Guid WICPixelFormat32bpp3ChannelsAlpha = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x2e);
        public static readonly Guid WICPixelFormat40bpp4ChannelsAlpha = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x2f);
        public static readonly Guid WICPixelFormat48bpp5ChannelsAlpha = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x30);
        public static readonly Guid WICPixelFormat56bpp6ChannelsAlpha = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x31);
        public static readonly Guid WICPixelFormat64bpp7ChannelsAlpha = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x32);
        public static readonly Guid WICPixelFormat72bpp8ChannelsAlpha = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x33);

        public static readonly Guid WICPixelFormat64bpp3ChannelsAlpha = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x34);
        public static readonly Guid WICPixelFormat80bpp4ChannelsAlpha = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x35);
        public static readonly Guid WICPixelFormat96bpp5ChannelsAlpha = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x36);
        public static readonly Guid WICPixelFormat112bpp6ChannelsAlpha = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x37);
        public static readonly Guid WICPixelFormat128bpp7ChannelsAlpha = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x38);
        public static readonly Guid WICPixelFormat144bpp8ChannelsAlpha = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x39);
    }

    [Flags]
    internal enum WICComponentType : int
    {
        WICDecoder              = 0x00000001,
        WICEncoder              = 0x00000002,
        WICPixelFormatConverter = 0x00000004,
        WICMetadataReader       = 0x00000008,
        WICMetadataWriter       = 0x00000010,
        WICPixelFormat          = 0x00000020,
        WICAllComponents        = 0x0000003F,
    }

    internal enum WICBitmapDitherType : int
    {
        None             = 0x00000000,
        Solid            = 0x00000000,
        Ordered4x4       = 0x00000001,
        Ordered8x8       = 0x00000002,
        Ordered16x16     = 0x00000003,
        Spiral4x4        = 0x00000004,
        Spiral8x8        = 0x00000005,
        DualSpiral4x4    = 0x00000006,
        DualSpiral8x8    = 0x00000007,
        ErrorDiffusion   = 0x00000008,
    }

    /// <summary>WICDecodeMetadata*, WICDecodeOptions</summary>
    internal enum WICDecodeMetadata : int
    {
        CacheOnDemand = 0x00000000,
        CacheOnLoad   = 0x00000001,
    }

    [Flags]
    internal enum WICComponentSigning : uint
    {
        WICComponentSigned             = 0x00000001,
        WICComponentUnsigned          = 0x00000002,
        WICComponentSafe               = 0x00000004,
        WICComponentDisabled           = 0x80000000,
    }

    /// <summary>
    /// WICBitmapPaletteType*
    /// </summary>
    internal enum WICBitmapPaletteType
    {
        /// <summary>Arbitrary custom palette provided by caller.</summary>
        Custom = 0x00000000,
        /// <summary>Optimal palette generated using a median-cut algorithm.</summary>
        MedianCut = 0x00000001,
        /// <summary>Black and white palette.</summary>
        FixedBW = 0x00000002,

        // Symmetric halftone palettes.
        // Each of these halftone palettes will be a superset of the system palette.
        // E.g. Halftone8 will have it's 8-color on-off primaries and the 16 system
        // colors added. With duplicates removed, that leaves 16 colors.

        FixedHalftone8   = 0x00000003, // 8-color, on-off primaries
        FixedHalftone27  = 0x00000004, // 3 intensity levels of each color
        FixedHalftone64  = 0x00000005, // 4 intensity levels of each color
        FixedHalftone125 = 0x00000006, // 5 intensity levels of each color
        FixedHalftone216 = 0x00000007, // 6 intensity levels of each color

        /// <summary>convenient web palette, same as WICBitmapPaletteTypeFixedHalftone216</summary>
        FixedWebPalette = FixedHalftone216,

        // Assymetric halftone palettes.
        // These are somewhat less useful than the symmetric ones, but are
        // included for completeness. These do not include all of the system
        // colors.

        FixedHalftone252 = 0x00000008, // 6-red, 7-green, 6-blue intensities
        FixedHalftone256 = 0x00000009, // 8-red, 8-green, 4-blue intensities

        FixedGray4       = 0x0000000A,//   4 shades of gray
        FixedGray16      = 0x0000000B,//  16 shades of gray
        FixedGray256     = 0x0000000C,// 256 shades of gray
    }

    /// <summary>
    /// WICBitmapTransform*, WICBitmapTransformOptions
    /// </summary>
    internal enum WICBitmapTransform : int
    {
        Rotate0             = 0x00000000,
        Rotate90            = 0x00000001,
        Rotate180           = 0x00000002,
        Rotate270           = 0x00000003,
        FlipHorizontal      = 0x00000008,
        FlipVertical        = 0x00000010,
    }

    /// <summary>
    /// WICBitmap*
    /// </summary>
    internal enum WICBitmapCreateCacheOption : int
    {
        NoCache       = 0x00000000,
        CacheOnDemand = 0x00000001,
        CacheOnLoad   = 0x00000002,
    }

    /// <summary>
    /// WICBitmap*
    /// </summary>
    internal enum WICBitmapAlphaChannelOption : int
    {
        UseAlpha                     = 0x00000000,
        UsePremultipliedAlpha        = 0x00000001,
        IgnoreAlpha                  = 0x00000002,
    }

    #endregion 

    #region WIC Structures

    [StructLayout(LayoutKind.Sequential)]
    internal struct WICRect 
    {
      public int X;
      public int Y;
      public int Width;
      public int Height;
    }

    #endregion

    #region WIC Interfaces

    [
        ComImport,
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        Guid(IID.WICBitmapDecoder),
    ]
    internal interface IWICBitmapDecoder
    {
        uint QueryCapability([In] IStream pIStream);

        void Initialize([In] IStream pIStream, WICDecodeMetadata cacheOptions);

        Guid GetContainerFormat();

        // returns IWICBitmapDecoderInfo
        IntPtr GetDecoderInfo();

        void CopyPalette([In] /*IWICPalette*/ IntPtr pIPalette);

        // returns IWICMetadataQueryReader 
        IntPtr GetMetadataQueryReader();

        // returns IWICBitmapSource
        IntPtr GetPreview();

        void GetColorContexts(int cCount, [In, Out] /*IWICColorContext*/ ref IntPtr ppIColorContexts, out int pcActualCount);

        // returns IWICBitmapSource
        IntPtr GetThumbnail();

        int GetFrameCount();

        IWICBitmapFrameDecode GetFrame(int index);
    }

    [
        ComImport,
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        Guid(IID.WICBitmapSource)
    ]
    internal interface IWICBitmapSource
    {
        void GetSize(out int puiWidth, out int puiHeight);

        Guid GetPixelFormat();

        void GetResolution(out double pDpiX, out double pDpiY);

        void CopyPalette(IntPtr /*IWICPalette*/ pIPalette);

        void CopyPixels(ref WICRect prc, int cbStride, int cbBufferSize, [In, Out] IntPtr pbBuffer);
    }

    [
        ComImport,
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        Guid(IID.WICFormatConverter)
    ]
    interface IWICFormatConverter : IWICBitmapSource
    {
        #region IWICBitmapSource redeclaration
        new void GetSize(out int puiWidth, out int puiHeight);
        new Guid GetPixelFormat();
        new void GetResolution(out double pDpiX, out double pDpiY);
        new void CopyPalette(IntPtr /*IWICPalette*/ pIPalette);
        new void CopyPixels(ref WICRect prc, int cbStride, int cbBufferSize, [In, Out] IntPtr pbBuffer);
        #endregion

        void Initialize(
            IWICBitmapSource pISource,
            [In] ref Guid dstFormat,
            WICBitmapDitherType dither,
            IntPtr pIPalette,
            double alphaThresholdPercent,
            WICBitmapPaletteType paletteTranslate);

        [return: MarshalAs(UnmanagedType.Bool)]
        bool CanConvert(
            [In] ref Guid srcPixelFormat,
            [In] ref Guid dstPixelFormat);
    }

    [
        ComImport,
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        Guid(IID.WICBitmap)
    ]
    internal interface IWICBitmap : IWICBitmapSource
    {
        #region IWICBitmapSource redeclaration
        new void GetSize(out int puiWidth, out int puiHeight);
        new Guid GetPixelFormat();
        new void GetResolution(out double pDpiX, out double pDpiY);
        new void CopyPalette(IntPtr /*IWICPalette*/ pIPalette);
        new void CopyPixels(ref WICRect prc, int cbStride, int cbBufferSize, [In, Out] IntPtr pbBuffer);
        #endregion

        // IWICBitmapLock
        IntPtr Lock([In] ref WICRect prcLock, int flags);

        void SetPalette(IntPtr pIPalette);

        void SetResolution(double dpiX, double dpiY);
    }

    [
        ComImport,
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        Guid(IID.WICBitmapFrameDecode)
    ]
    internal interface IWICBitmapFrameDecode : IWICBitmapSource
    {
        #region IWICBitmapSource redeclaration
        new void GetSize(out int puiWidth, out int puiHeight);
        new Guid GetPixelFormat();
        new void GetResolution(out double pDpiX, out double pDpiY);
        new void CopyPalette(IntPtr /*IWICPalette*/ pIPalette);
        new void CopyPixels(ref WICRect prc, int cbStride, int cbBufferSize, [In, Out] IntPtr pbBuffer);
        #endregion

        // IWICMetadataQueryReader
        IntPtr GetMetadataQueryReader();

        void GetColorContexts(int cCount, [In, Out] ref IntPtr /*IWICColorContext*/ ppIColorContexts, out int pcActualCount);

        IWICBitmapSource GetThumbnail();
    }

    [
        ComImport,
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        Guid(IID.WICBitmapFlipRotator)
    ]
    internal interface IWICBitmapFlipRotator : IWICBitmapSource
    {
        #region IWICBitmapSource redeclaration
        new void GetSize(out int puiWidth, out int puiHeight);
        new Guid GetPixelFormat();
        new void GetResolution(out double pDpiX, out double pDpiY);
        new void CopyPalette(IntPtr /*IWICPalette*/ pIPalette);
        new void CopyPixels(ref WICRect prc, int cbStride, int cbBufferSize, [In, Out] IntPtr pbBuffer);
        #endregion

        void Initialize(IWICBitmapSource pISource, WICBitmapTransform options);
    }

    [
        ComImport,
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        Guid(IID.WICImagingFactory)
    ]
    internal interface IWICImagingFactory
    {
        IWICBitmapDecoder CreateDecoderFromFilename(
            [MarshalAs(UnmanagedType.LPWStr)] string wzFileName,
            [In] ref Guid pguidVendor,
            uint dwDesiredAccess,
            WICDecodeMetadata metadataOptions);

        IWICBitmapDecoder CreateDecoderFromStream(
            [In] IStream pIStream,
            [In] ref Guid pguidVendor,
            WICDecodeMetadata metadataOptions);

        IWICBitmapDecoder CreateDecoderFromFileHandle(
            IntPtr hFile,
            [In] ref Guid pguidVendor,
            WICDecodeMetadata metadataOptions);

        // returns IWICComponentInfo
        IntPtr CreateComponentInfo([In] ref Guid clsidComponent);

        IWICBitmapDecoder CreateDecoder(
            [In] ref Guid guidContainerFormat,
            [In] ref Guid pguidVendor);

        // IWICBitmapEncoder
        IntPtr CreateEncoder(
            [In] ref Guid guidContainerFormat,
            [In] ref Guid pguidVendor);

        // IWICPalette
        IntPtr CreatePalette();

        IWICFormatConverter CreateFormatConverter();

        // IWICBitmapScaler
        IntPtr CreateBitmapScaler();

        // IWICBitmapClipper
        IntPtr CreateBitmapClipper();

        IWICBitmapFlipRotator CreateBitmapFlipRotator();

        IWICStream CreateStream();

        // IWICColorContext 
        IntPtr CreateColorContext();

        // IWICColorTransform 
        IntPtr CreateColorTransformer();

        /* Bitmap creation */

        IWICBitmap CreateBitmap(
            int uiWidth,
            int uiHeight,
            [In] ref Guid pixelFormat,
            WICBitmapCreateCacheOption option);

        IWICBitmap CreateBitmapFromSource(
            IWICBitmapSource pIBitmapSource,
            WICBitmapCreateCacheOption option);

        IWICBitmap CreateBitmapFromSourceRect(
            IWICBitmapSource pIBitmapSource,
            int x,
            int y,
            int width,
            int height);

        IWICBitmap CreateBitmapFromMemory(
            int uiWidth,
            int uiHeight,
            [In] ref Guid pixelFormat,
            int cbStride,
            int cbBufferSize,
            IntPtr pbBuffer);

        IWICBitmap CreateBitmapFromHBITMAP(
            IntPtr hBitmap,
            IntPtr hPalette,
            WICBitmapAlphaChannelOption options);

        IWICBitmap CreateBitmapFromHICON(IntPtr hIcon);

        // IEnumUnknown
        IntPtr CreateComponentEnumerator(
            int componentTypes, // WICComponentType
            int options); // WICComponentEnumerateOptions

        // IWICFastMetadataEncoder
        IntPtr CreateFastMetadataEncoderFromDecoder(IWICBitmapDecoder pIDecoder);

        // IWICFastMetadataEncoder 
        IntPtr CreateFastMetadataEncoderFromFrameDecode(IntPtr pIFrameDecoder);

        // IWICMetadataQueryWriter 
        IntPtr CreateQueryWriter(
            [In] ref Guid guidMetadataFormat,
            [In] ref Guid pguidVendor);

        // IWICMetadataQueryWriter
        IntPtr CreateQueryWriterFromReader(
            IntPtr pIQueryReader,
            [In] ref Guid pguidVendor);
    }

    [
       ComImport,
       InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
       Guid(IID.WICStream)
    ]
    internal interface IWICStream : IStream
    {
        #region IStream redeclaration

        new void Clone(out IStream ppstm);
        new void Commit(int grfCommitFlags);
        new void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten);
        new void LockRegion(long libOffset, long cb, int dwLockType);
        new void Read(byte[] pv, int cb, IntPtr pcbRead);
        new void Revert();
        new void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition);
        new void SetSize(long libNewSize);
        new void Stat(out STATSTG pstatstg, int grfStatFlag);
        new void UnlockRegion(long libOffset, long cb, int dwLockType);
        new void Write(byte[] pv, int cb, IntPtr pcbWritten);

        #endregion 

        void InitializeFromIStream(IStream pIStream);
        void InitializeFromFilename([In, MarshalAs(UnmanagedType.LPWStr)] string wzFileName, int dwDesiredAccess);
        void InitializeFromMemory(IntPtr pbBuffer, uint cbBufferSize);
        void InitializeFromIStreamRegion(IStream pIStream, ulong ulOffset, ulong ulMaxSize);
    }

    #endregion
}
