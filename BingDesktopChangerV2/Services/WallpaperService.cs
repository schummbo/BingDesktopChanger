using Microsoft.Win32;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace BingDesktopChangerV2.Services
{
    [SupportedOSPlatform("windows")]
    public class WallpaperService
    {
        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public void SetWallpaper(Bitmap img)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
            img.Save(tempPath, System.Drawing.Imaging.ImageFormat.Bmp);

            RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

            if (key == null)
            {
                throw new Exception("Unable to find desktop registry key");
            }

            key.SetValue(@"WallpaperStyle", 2.ToString());
            key.SetValue(@"TileWallpaper", 0.ToString());


            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                tempPath,
                SPIF_UPDATEINIFILE);
        }
    }
}
