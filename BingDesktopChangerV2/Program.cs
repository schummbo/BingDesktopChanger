using BingDesktopChangerV2.Clients;
using BingDesktopChangerV2.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using var host = Host.CreateDefaultBuilder().Build();

if (host.Services.GetService(typeof(IConfiguration)) is not IConfiguration config)
{
    throw new Exception("Unable to get configuration");
}

string resolution = config["resolution"];

if (string.IsNullOrWhiteSpace(resolution))
{
    throw new Exception("Unable to get resolution from configuration");
}

var bingWallpaperMetadata = await new BingClient().GetWallPaperMetadataAsync();

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
var imageUrl = $"http://www.bing.com{imageUrlBase}_{resolution}.jpg";

var image = await new ImageDownloadService().GetImage(new Uri(imageUrl));

var imageWithCopyright = new ImageEditorService().AddTextToImage(image, copyright);

new WallpaperService().SetWallpaper(imageWithCopyright);
