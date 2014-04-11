using System;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BingDesktopChanger
{
    public class Wallpaper
    {
        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public enum Style
        {
            Tiled,
            Centered,
            Stretched
        }

        public static void Set(string uri, Style style, string copyrightText)
        {
            Stream s = new System.Net.WebClient().OpenRead(uri);

            Bitmap img = (Bitmap)Image.FromStream(s);

            if (ConfigurationManager.AppSettings["RenderCopyright"] == "true" && !string.IsNullOrWhiteSpace(copyrightText))
            {
                WriteOnImage(img, copyrightText);
            }

            string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
            img.Save(tempPath, System.Drawing.Imaging.ImageFormat.Bmp);

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            if (style == Style.Stretched)
            {
                key.SetValue(@"WallpaperStyle", 2.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == Style.Centered)
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == Style.Tiled)
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 1.ToString());
            }

            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                tempPath,
                SPIF_UPDATEINIFILE);
        }

        public static void WriteOnImage(Bitmap image, string textToAdd)
        {
            using (var graphics = Graphics.FromImage(image))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                bool fits;
                var size = 17;
                var offset = new PointF(20, 15);

                var copyrightContainer = CreateCopyrightContainer(image, graphics);

                Font copyrightFont = null;

                do
                {
                    if (copyrightFont != null)
                    {
                        copyrightFont.Dispose();
                    }

                    copyrightFont = new Font("Calibri", size);

                    fits = DoesTextFitInContainer(textToAdd, graphics, copyrightFont, offset, copyrightContainer);

                    size -= 1;

                }
                while (!fits);

                graphics.DrawString(textToAdd, copyrightFont, Brushes.White, offset);

                graphics.Flush();
            }
        }

        private static bool DoesTextFitInContainer(string textToAdd, Graphics graphics, Font copyrightFont, PointF offset, RectangleF copyrightContainer)
        {
            var copyrightSize = graphics.MeasureString(textToAdd, copyrightFont);

            var fits = copyrightSize.Width + offset.Y < copyrightContainer.Width;

            return fits;
        }

        private static RectangleF CreateCopyrightContainer(Bitmap image, Graphics graphics)
        {
            var copyrightContainer = new RectangleF(0, 0, image.Width, 60);
            Color transparentGray = Color.FromArgb(150, Color.Gray);
            SolidBrush copyrightContainerBrush = new SolidBrush(transparentGray);
            graphics.FillRectangle(copyrightContainerBrush, copyrightContainer);
            return copyrightContainer;
        }
    }
}
