using BingDesktopChangerV2.Clients;
using BingDesktopChangerV2.Factories;
using BingDesktopChangerV2.Services;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
string resolution = config["resolution"];

if (string.IsNullOrWhiteSpace(resolution))
{
    throw new Exception("Unable to get resolution from configuration");
}

var httpClient = HttpClientFactory.CreateHttpClient();

var bingWallpaperMetadata = await new BingClient(httpClient).GetWallPaperMetadataAsync();

if (bingWallpaperMetadata == null)
{
    throw new Exception("Unable to get bing metadata.");
}

var imageMetadata = bingWallpaperMetadata.images.FirstOrDefault();

if (imageMetadata == null)
{
    throw new Exception("Unable to get image metadata from bing metadata");
}

var copyright = imageMetadata.copyright;
var imageUrlBase = imageMetadata.urlbase;
var imageUrl = $"https://www.bing.com{imageUrlBase}_{resolution}.jpg";

var image = await new ImageDownloadService(httpClient).GetImageAsync(new Uri(imageUrl));

var imageWithCopyright = new ImageEditorService().AddTextToImage(image, copyright);

new WallpaperService().SetWallpaper(imageWithCopyright);
