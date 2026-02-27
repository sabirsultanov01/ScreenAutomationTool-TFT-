namespace ScreenAutomationTool.Interfaces
{
    public interface IInputSimulatorService
    {
        void LeftClick(int x, int y, int preDelayMs = 30);
        void RightClick(int x, int y, int preDelayMs = 30);
        void DoubleClick(int x, int y);
        void MoveTo(int x, int y);
        void PressKey(byte virtualKeyCode);
        void TypeText(string text, int charDelayMs = 30);
    }
}