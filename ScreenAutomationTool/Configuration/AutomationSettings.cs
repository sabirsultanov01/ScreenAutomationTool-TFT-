namespace ScreenAutomationTool.Config
{
    public class AutomationSettings
    {
        public int MaxRetries { get; set; } = 20;
        public int ScanIntervalMs { get; set; } = 500;
        public int StepDelayMs { get; set; } = 1000;
        public string TessDataPath { get; set; } = "./tessdata";
        public string Language { get; set; } = "eng";
    }
}