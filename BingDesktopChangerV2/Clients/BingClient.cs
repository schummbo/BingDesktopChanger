using BingDesktopChangerV2.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace BingDesktopChangerV2.Clients
{
    public class BingClient
    {
        private const string WallpaperUrl = "https://www.bing.com/hpimagearchive.aspx?format=js&idx=0&n=1&mbl=1&mkt=en-ww";
        private readonly HttpClient httpClient;

        public BingClient()
        {
            httpClient = new HttpClient();
        }

        public async Task<BingWallpaperMetadata?> GetWallPaperMetadataAsync()
        {
            var response = await httpClient.GetStringAsync(WallpaperUrl);

            var metadata = JsonSerializer.Deserialize<BingWallpaperMetadata>(response);

            return metadata;
        }
    }
}
