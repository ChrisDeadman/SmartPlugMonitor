using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using log4net;

namespace SmartPlugMonitor.Toolbox
{
    public class TextIconRenderer
    {
        private static readonly StringFormat TextFormat = new StringFormat (StringFormat.GenericTypographic) {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.NoClip
        };

        private readonly string FontFamily;
        private readonly int FontSize;
        private readonly int IconSize;

        private readonly Font BigFont;
        private readonly Font SmallFont;

        public TextIconRenderer (string fontFamily, int fontSize, int iconSize)
        {
            this.FontFamily = fontFamily;
            this.FontSize = ScaleToDpi (fontSize);
            this.IconSize = iconSize;

            this.BigFont = new Font (FontFamily, FontSize, FontStyle.Regular, GraphicsUnit.Point);
            this.SmallFont = new Font (FontFamily, (int)(FontSize * 0.75f), FontStyle.Regular, GraphicsUnit.Point);
        }

        public Bitmap Render (string text, Color color)
        {
            var useSmallFont = text.Length > 2;
            var font = useSmallFont ? SmallFont : BigFont;
            var textBrush = new SolidBrush (color);

            var bitmap = new Bitmap (IconSize, IconSize, PixelFormat.Format32bppArgb);      
            using (var graphics = Graphics.FromImage (bitmap)) {
                graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.Clear (Color.Transparent);
                graphics.DrawString (text, font, textBrush, new RectangleF (0, 0, bitmap.Width, bitmap.Height), TextFormat);
            }
            return bitmap;
        }

        private static int ScaleToDpi (int fontSize)
        {
            float dpiX, dpiY;
            using (var bitmap = new Bitmap (1, 1, PixelFormat.Format32bppArgb))
            using (var graphics = Graphics.FromImage (bitmap)) {
                dpiX = graphics.DpiX;
                dpiY = graphics.DpiY;
            }

            return (int)(fontSize * ((96f * 96f) / (dpiX * dpiY)));
        }
    }
}
