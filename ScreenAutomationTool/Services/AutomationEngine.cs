using ScreenAutomationTool.Config;
using ScreenAutomationTool.Interfaces;

namespace ScreenAutomationTool.Services
{
    public class AutomationEngine : IAutomationEngine
    {
        private readonly IScreenCaptureService _capture;
        private readonly IInputSimulatorService _input;
        private readonly ITextFinderService _textFinder;
        private readonly AutomationSettings _settings;
        private CancellationTokenSource? _cts;

        public event Action<string>? OnLog;
        public bool IsRunning => _cts != null && !_cts.IsCancellationRequested;

        // Y offset for lower-half capture
        private readonly int _yOffset;

        public AutomationEngine(AutomationSettings settings)
        {
            _settings = settings;
            _capture = new ScreenCaptureService();
            _input = new InputSimulatorService();
            _textFinder = new TextFinderService(settings.TessDataPath, settings.Language);
            _yOffset = Screen.PrimaryScreen!.Bounds.Height / 2;
        }

        public async Task RunScript(List<string> keywords)
        {
            _cts = new CancellationTokenSource();
            var ct = _cts.Token;

            Log($"Script started — scanning lower half for {keywords.Count} keyword(s).");

            try
            {
                while (!ct.IsCancellationRequested)
                {
                    using var screenshot = _capture.CaptureLowerHalf();

                    foreach (var keyword in keywords)
                    {
                        ct.ThrowIfCancellationRequested();

                        var matches = _textFinder.FindText(screenshot, keyword);

                        if (matches.Count > 0)
                        {
                            var best = matches.OrderByDescending(m => m.Confidence).First();

                            // Offset Y to get actual screen position
                            int clickX = best.Center.X;
                            int clickY = best.Center.Y + _yOffset;

                            Log($"Found \"{best.Text}\" at ({clickX}, {clickY}) " +
                                $"[confidence: {best.Confidence:F1}%]");

                            _input.LeftClick(clickX, clickY);

                            await Task.Delay(_settings.StepDelayMs, ct);
                        }
                    }

                    await Task.Delay(_settings.ScanIntervalMs, ct);
                }
            }
            catch (OperationCanceledException)
            {
                Log("Script stopped by user.");
            }

            Log("Session ended.");
        }

        public void Stop()
        {
            _cts?.Cancel();
            Log("Stop requested.");
        }

        private void Log(string message)
        {
            OnLog?.Invoke($"[{DateTime.Now:HH:mm:ss}] {message}");
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _textFinder?.Dispose();
        }
    }
}