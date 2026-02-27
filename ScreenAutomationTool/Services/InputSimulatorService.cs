using ScreenAutomationTool.Helpers;
using ScreenAutomationTool.Interfaces;


namespace ScreenAutomationTool.Services
{
    public class InputSimulatorService : IInputSimulatorService
    {
        public void LeftClick(int x, int y, int preDelayMs = 30)
        {
            NativeMethods.SetCursorPos(x, y);
            Thread.Sleep(preDelayMs);
            NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            Thread.Sleep(20);
            NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        public void RightClick(int x, int y, int preDelayMs = 30)
        {
            NativeMethods.SetCursorPos(x, y);
            Thread.Sleep(preDelayMs);
            NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
            Thread.Sleep(20);
            NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
        }

        public void DoubleClick(int x, int y)
        {
            LeftClick(x, y);
            Thread.Sleep(80);
            LeftClick(x, y, preDelayMs: 10);
        }

        public void MoveTo(int x, int y)
        {
            NativeMethods.SetCursorPos(x, y);
        }

        public void PressKey(byte virtualKeyCode)
        {
            NativeMethods.keybd_event(virtualKeyCode, 0, 0, 0);
            Thread.Sleep(20);
            NativeMethods.keybd_event(virtualKeyCode, 0, NativeMethods.KEYEVENTF_KEYUP, 0);
        }

        public void TypeText(string text, int charDelayMs = 30)
        {
            foreach (char c in text)
            {
                SendKeys.SendWait(c.ToString());
                Thread.Sleep(charDelayMs);
            }
        }
    }
}