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

        public static void WriteOnImage(Bitmap Image, string textToAdd)
        {
            var copyrightContainer = new RectangleF(0, 0, Image.Width, 60);
            
            using (Graphics g = Graphics.FromImage(Image))
            {
                Color transparentGray = Color.FromArgb(150, Color.Gray);
                SolidBrush copyrightContainerBrush = new SolidBrush(transparentGray);
                g.FillRectangles(copyrightContainerBrush, new[] { copyrightContainer });

                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawString(textToAdd, new Font("Calibri", 17), Brushes.White, new PointF(20, 15));

                g.Flush();
            };
        }
    }
}
