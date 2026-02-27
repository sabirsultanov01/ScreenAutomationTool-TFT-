using System.Drawing;

namespace ScreenAutomationTool.Interfaces
{
    public interface IScreenCaptureService
    {
        Bitmap CaptureScreen();
        Bitmap CaptureRegion(Rectangle region);
        Bitmap CaptureLowerHalf();
    }
}