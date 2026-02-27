namespace ScreenAutomationTool.Interfaces
{
    public interface IAutomationEngine : IDisposable
    {
        event Action<string>? OnLog;
        bool IsRunning { get; }
        Task RunScript(List<string> keywords);
        void Stop();
    }
}