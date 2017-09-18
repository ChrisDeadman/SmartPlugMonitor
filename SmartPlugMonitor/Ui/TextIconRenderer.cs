using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using SmartPlugMonitor.Toolbox;

namespace SmartPlugMonitor.Ui
{
    public class TextIconRenderer
    {
        private readonly Font bigFont;
        private readonly Font smallFont;
        private readonly Bitmap bitmap;
        private readonly Graphics graphics;

        public TextIconRenderer ()
        {
            // get the default dpi to create an icon with the correct size
            float dpiX, dpiY;
            using (Bitmap b = new Bitmap (1, 1, PixelFormat.Format32bppArgb)) {
                dpiX = b.HorizontalResolution;
                dpiY = b.VerticalResolution;
            }

            // adjust the size of the icon to current dpi (default is 16x16 at 96 dpi) 
            var width = (int)Math.Round (16 * dpiX / 96);
            var height = (int)Math.Round (16 * dpiY / 96);

            // make sure it does never get smaller than 16x16
            width = width < 16 ? 16 : width;
            height = height < 16 ? 16 : height;

            // adjust the font size to the icon size
            var fontFamily = "Tahoma";
            var baseSize = 12;

            bigFont = new Font (fontFamily, baseSize * width / 16.0f, GraphicsUnit.Pixel);
            smallFont = new Font (fontFamily, 0.75f * baseSize * width / 16.0f, GraphicsUnit.Pixel);

            bitmap = new Bitmap (width, height, PixelFormat.Format32bppArgb);      
            graphics = Graphics.FromImage (bitmap);

            if (Environment.OSVersion.Version.Major > 5) {
                graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
            }
        }

        public Icon Render (string text)
        {
            var useSmallFont = text.Length > 2;
            var x = useSmallFont ? -3 : -2;
            var y = useSmallFont ? 3 : 0;
            var font = useSmallFont ? smallFont : bigFont;

            graphics.Clear (Color.Transparent);
            TextRenderer.DrawText (graphics, text, font, new Point (x, y), Color.White, Color.Transparent);

            return bitmap.ToIcon ();
        }
    }
}
