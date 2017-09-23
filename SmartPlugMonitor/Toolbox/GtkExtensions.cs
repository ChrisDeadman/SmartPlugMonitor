using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

using Gdk;

namespace SmartPlugMonitor
{
    public static class GtkExtensions
    {
        public static Pixbuf ToPixbuf (this Bitmap bitmap)
        {
            using (var stream = new MemoryStream ()) {
                bitmap.Save (stream, ImageFormat.Png);
                stream.Seek (0, SeekOrigin.Begin);
                return new Pixbuf (stream);
            }
        }
    }
}
