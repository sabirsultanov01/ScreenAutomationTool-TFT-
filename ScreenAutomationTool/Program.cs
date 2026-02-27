using ScreenAutomation.Forms;
using ScreenAutomationTool;

namespace ScreenAutomationTool
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}