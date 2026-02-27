using System.Drawing;
using ScreenAutomationTool.Models;

namespace ScreenAutomationTool.Interfaces
{
    public interface ITextFinderService : IDisposable
    {
        List<TextMatch> FindText(Bitmap screenshot, string keyword);
        string ReadAllText(Bitmap screenshot);
    }
}