using log4net;
using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using SmartPlugMonitor.Toolbox;

namespace SmartPlugMonitor.Ui
{
    public class TrayIconText
    {
        private static readonly ILog Log = LogManager.GetLogger (typeof(TrayIconText));

        private readonly Font bigFont;
        private readonly Font smallFont;
        private readonly Bitmap bitmap;
        private readonly Graphics graphics;

        public TrayIconText ()
        {
            // get the default dpi to create an icon with the correct size
            float dpiX, dpiY;
            using (Bitmap b = new Bitmap (1, 1, PixelFormat.Format32bppArgb)) {
                dpiX = b.HorizontalResolution;
                dpiY = b.VerticalResolution;
            }

            // adjust the size of the icon to current dpi (default is 16x16 at 96 dpi) 
            int width = (int)Math.Round (16 * dpiX / 96);
            int height = (int)Math.Round (16 * dpiY / 96);

            // make sure it does never get smaller than 16x16
            width = width < 16 ? 16 : width;
            height = height < 16 ? 16 : height;

            // adjust the font size to the icon size
            FontFamily family = SystemFonts.MessageBoxFont.FontFamily;
            float baseSize;
            switch (family.Name) {
            case "Segoe UI":
                baseSize = 12;
                break;
            case "Tahoma":
                baseSize = 11;
                break;
            default:
                baseSize = 12;
                break;
            }

            bigFont = new Font (family, baseSize * width / 16.0f, GraphicsUnit.Pixel);
            smallFont = new Font (family, 0.7f * baseSize * width / 16.0f, GraphicsUnit.Pixel);

            bitmap = new Bitmap (width, height, PixelFormat.Format32bppArgb);      
            graphics = Graphics.FromImage (bitmap);

            if (Environment.OSVersion.Version.Major > 5) {
                graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
            }
        }

        public void DrawText (NotifyIcon trayIcon, String text)
        {
            bool useSmallFont = text.Length > 2;

            var x = -2;
            var y = useSmallFont ? 3 : 2;
            var font = useSmallFont ? smallFont : bigFont;

            graphics.Clear (Color.Transparent);
            TextRenderer.DrawText (graphics, text, font, new Point (x, y), Color.White, Color.Transparent);

            trayIcon.Icon = bitmap.ToIcon ();

            if (Log.IsDebugEnabled) {
                using (var fs = File.Create ("trayIcon.ico")) {
                    trayIcon.Icon.Save (fs);
                }
            }
        }
    }
}
