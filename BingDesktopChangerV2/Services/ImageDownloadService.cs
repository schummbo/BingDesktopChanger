using System.Drawing;
using System.Net;
using System.Runtime.Versioning;

namespace BingDesktopChangerV2.Services
{
    public class ImageDownloadService
    {
        private readonly HttpClient httpClient;

        public ImageDownloadService()
        {
            httpClient = new HttpClient();
        }

        [SupportedOSPlatform("windows")]
        public async Task<Bitmap> GetImageAsync(Uri uri)
        {
            await using var stream = await httpClient.GetStreamAsync(uri);
            return (Bitmap)Image.FromStream(stream);
        }
    }
}
