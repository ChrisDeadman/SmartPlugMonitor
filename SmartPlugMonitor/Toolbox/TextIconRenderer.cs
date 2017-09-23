using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Drawing.Drawing2D;

namespace SmartPlugMonitor.Toolbox
{
    public class TextIconRenderer
    {
        private static readonly Brush TextBrush = new SolidBrush (Color.White);

        private readonly Font bigFont;
        private readonly Font smallFont;
        private readonly int width;
        private readonly int height;

        public TextIconRenderer ()
        {
            // get the default dpi to create an icon with the correct size
            float dpiX, dpiY;
            using (var bitmap = new Bitmap (1, 1, PixelFormat.Format32bppArgb))
            using (var graphics = Graphics.FromImage (bitmap)) {
                dpiX = graphics.DpiX;
                dpiY = graphics.DpiY;
            }

            // adjust the size of the icon to current dpi (default is 24x24 at 96 dpi) 
            width = (int)Math.Round (32 * (dpiX / 96));
            height = (int)Math.Round (32 * (dpiY / 96));

            // make sure it does never get smaller than 16x16
            width = (width < 16) ? 16 : width;
            height = (height < 16) ? 16 : height;

            // adjust the font size to the icon size
            var fontFamily = "Tahoma";
            var baseSize = 10;

            bigFont = new Font (fontFamily, baseSize * width / 16.0f, FontStyle.Bold, GraphicsUnit.Pixel);
            smallFont = new Font (fontFamily, 0.75f * baseSize * width / 16.0f, FontStyle.Bold, GraphicsUnit.Pixel);
        }

        public Bitmap Render (string text)
        {
            var useSmallFont = text.Length > 2;
            var x = useSmallFont ? 2 : 3;
            var y = useSmallFont ? 7 : 4;
            var font = useSmallFont ? smallFont : bigFont;

            var bitmap = new Bitmap (width, height, PixelFormat.Format32bppArgb);      
            using (var graphics = Graphics.FromImage (bitmap)) {
                graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.Clear (Color.Transparent);
                graphics.DrawString (text, font, TextBrush, new RectangleF (x, y, bitmap.Width, bitmap.Height), StringFormat.GenericTypographic);

                return bitmap;
            }
        }
    }
}
