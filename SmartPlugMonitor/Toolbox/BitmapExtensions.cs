using System;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;

namespace SmartPlugMonitor.Toolbox
{
    /// <summary>
    /// Conversion extensions for <see cref="Bitmap"/>"/>
    /// </summary>
    public static class BitmapExtensions
    {
        [StructLayout (LayoutKind.Sequential)]
        private struct IconDirEntry
        {
            internal byte width;
            internal byte height;
            internal byte colorCount;
            internal byte reserved;
            internal ushort planes;
            internal ushort bitCount;
            internal uint bytesInRes;
            internal uint imageOffset;
            internal bool ignore;
        };

        [StructLayout (LayoutKind.Sequential)]
        private struct IconDir
        {
            internal ushort idReserved;
            internal ushort idType;
            internal ushort idCount;
            internal IconDirEntry[] idEntries;
        };

        [StructLayout (LayoutKind.Sequential)]
        private struct BitmapInfoHeader
        {
            internal uint biSize;
            internal int biWidth;
            internal int biHeight;
            internal ushort biPlanes;
            internal ushort biBitCount;
            internal uint biCompression;
            internal uint biSizeImage;
            internal int biXPelsPerMeter;
            internal int biYPelsPerMeter;
            internal uint biClrUsed;
            internal uint biClrImportant;
        };

        [StructLayout (LayoutKind.Sequential)]
        private abstract class ImageData
        {
        };

        [StructLayout (LayoutKind.Sequential)]
        private class IconImage : ImageData
        {
            internal BitmapInfoHeader iconHeader;
            internal uint[] iconColors;
            internal byte[] iconXOR;
            internal byte[] iconAND;
        };

        [StructLayout (LayoutKind.Sequential)]
        private class IconDump : ImageData
        {
            internal byte[] data;
        };

        /// <summary>
        /// Converts this <see cref="Bitmap"/> to an <see cref="Icon"/>.
        /// </summary>
        /// <param name="bitmap">The bitmap to convert.</param>
        public static Icon ToIcon (this Bitmap bitmap)
        {
            var bih = new BitmapInfoHeader {
                biSize = (uint)Marshal.SizeOf (typeof(BitmapInfoHeader)),
                biWidth = bitmap.Width,
                biHeight = 2 * bitmap.Height, // include both XOR and AND images
                biPlanes = 1,
                biBitCount = 32,
                biCompression = 0,
                biSizeImage = 0,
                biXPelsPerMeter = 0,
                biYPelsPerMeter = 0,
                biClrUsed = 0,
                biClrImportant = 0
            };

            var xorSize = (((bih.biBitCount * bitmap.Width + 31) & ~31) >> 3) * bitmap.Height;
            var andLineSize = (((bitmap.Width + 31) & ~31) >> 3);    // must be a multiple of 4 bytes
            var andSize = andLineSize * bitmap.Height;

            var iconDirEntry = new IconDirEntry {
                width = (byte)bitmap.Width,
                height = (byte)bitmap.Height,
                colorCount = 0, // 32 bbp == 0, for palette size
                reserved = 0,
                planes = 0,
                bitCount = 32,
                imageOffset = 22,   // 22 is the first icon position (for single icon files)
                bytesInRes = (uint)(bih.biSize + xorSize + andSize)
            };

            var iconImage = new IconImage {
                iconHeader = bih,
                iconColors = new uint [0],   // no palette
                iconXOR = new byte [xorSize],
                iconAND = new byte [andSize]
            };

            var pixel = 0;
            for (var y = bitmap.Height - 1; y >= 0; y--) {
                for (var x = 0; x < bitmap.Width; x++) {
                    var color = bitmap.GetPixel (x, y);
                    iconImage.iconXOR [pixel++] = color.B;
                    iconImage.iconXOR [pixel++] = color.G;
                    iconImage.iconXOR [pixel++] = color.R;
                    iconImage.iconXOR [pixel++] = color.A;
                }
            }

            using (var outputStream = new MemoryStream ())
            using (var streamWriter = new BinaryWriter (outputStream)) {
                streamWriter.Write ((ushort)0);   // idReserved must be 0
                streamWriter.Write ((ushort)1);   // idType must be 1
                streamWriter.Write ((ushort)1);   // only one icon

                SaveIconDirEntry (streamWriter, iconDirEntry);
                SaveIconImage (streamWriter, iconImage);

                outputStream.Seek (0, SeekOrigin.Begin);
                return new Icon (outputStream);
            }
        }

        private static void SaveIconDirEntry (BinaryWriter writer, IconDirEntry iconDirEntry)
        {
            writer.Write (iconDirEntry.width);
            writer.Write (iconDirEntry.height);
            writer.Write (iconDirEntry.colorCount);
            writer.Write (iconDirEntry.reserved);
            writer.Write (iconDirEntry.planes);
            writer.Write (iconDirEntry.bitCount);
            writer.Write (iconDirEntry.bytesInRes);
            writer.Write (iconDirEntry.imageOffset);
        }

        private static void SaveIconImage (BinaryWriter writer, IconImage iconImage)
        {
            BitmapInfoHeader bih = iconImage.iconHeader;
            writer.Write (bih.biSize);
            writer.Write (bih.biWidth);
            writer.Write (bih.biHeight);
            writer.Write (bih.biPlanes);
            writer.Write (bih.biBitCount);
            writer.Write (bih.biCompression);
            writer.Write (bih.biSizeImage);
            writer.Write (bih.biXPelsPerMeter);
            writer.Write (bih.biYPelsPerMeter);
            writer.Write (bih.biClrUsed);
            writer.Write (bih.biClrImportant);

            for (var idx = 0; idx < iconImage.iconColors.Length; idx++)
                writer.Write (iconImage.iconColors [idx]);
            
            writer.Write (iconImage.iconXOR);
            writer.Write (iconImage.iconAND);
        }
    }
}
