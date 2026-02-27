using System.Drawing;
using System.Drawing.Imaging;
using ScreenAutomationTool.Interfaces;

namespace ScreenAutomationTool.Services
{
    public class ScreenCaptureService : IScreenCaptureService
    {
        public Bitmap CaptureScreen()
        {
            var bounds = Screen.PrimaryScreen!.Bounds;
            var bmp = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
            using var g = Graphics.FromImage(bmp);
            g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            return bmp;
        }

        public Bitmap CaptureRegion(Rectangle region)
        {
            var bmp = new Bitmap(region.Width, region.Height, PixelFormat.Format32bppArgb);
            using var g = Graphics.FromImage(bmp);
            g.CopyFromScreen(region.Location, Point.Empty, region.Size);
            return bmp;
        }

        public Bitmap CaptureLowerHalf()
        {
            var bounds = Screen.PrimaryScreen!.Bounds;
            var region = new Rectangle(
                0,
                bounds.Height / 2,
                bounds.Width,
                bounds.Height / 2
            );
            return CaptureRegion(region);
        }
    }
}