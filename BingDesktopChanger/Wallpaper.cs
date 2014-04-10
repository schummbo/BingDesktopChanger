using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BingDesktopChanger
{
    public sealed class Wallpaper
    {
        Wallpaper() { }

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public enum Style : int
        {
            Tiled,
            Centered,
            Stretched
        }

        public static void Set(string uri, Style style, string copyrightText)
        {
            Stream s = new System.Net.WebClient().OpenRead(uri);

            System.Drawing.Bitmap img = (Bitmap)System.Drawing.Bitmap.FromStream(s);

            if (ConfigurationManager.AppSettings["RenderCopyright"] == "true")
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
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        public static string WriteOnImage(Bitmap Image, string textToAdd)
        {
            string Message = "OK";
            try
            {
                Bitmap bitMapImage = new Bitmap(Image);

                RectangleF rectf = new RectangleF(0, 0, Image.Width, 75);


                using (Graphics g = Graphics.FromImage(Image))
                {

                    Color customColor = Color.FromArgb(150, Color.Gray);
                    SolidBrush shadowBrush = new SolidBrush(customColor);
                    g.FillRectangles(shadowBrush, new RectangleF[] { rectf });

                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.DrawString(textToAdd, new Font("Tahoma", 20), Brushes.White, new PointF(20, 20));

                    g.Flush();

                };

                return Message;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
