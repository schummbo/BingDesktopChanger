using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;

namespace BingDesktopChangerV2.Services
{
    [SupportedOSPlatform("windows")]
    public class ImageEditorService
    {
        public Bitmap AddTextToImage(Bitmap image, string textToAdd)
        {
            using var graphics = Graphics.FromImage(image);

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            bool fits;
            var size = 17;
            var offset = new PointF(20, 15);

            var copyrightContainer = CreateCopyrightContainer(image, graphics);

            Font? copyrightFont = null;

            do
            {
                copyrightFont?.Dispose();

                copyrightFont = new Font("Calibri", size);

                fits = DoesTextFitInContainer(textToAdd, graphics, copyrightFont, offset, copyrightContainer);

                size -= 1;

            }
            while (!fits);

            graphics.DrawString(textToAdd, copyrightFont, Brushes.White, offset);

            graphics.Flush();

            return image;
        }
        
        private static bool DoesTextFitInContainer(string textToAdd, Graphics graphics, Font copyrightFont, PointF offset, RectangleF copyrightContainer)
        {
            var copyrightSize = graphics.MeasureString(textToAdd, copyrightFont);

            bool fits = copyrightSize.Width + offset.Y < copyrightContainer.Width;

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
