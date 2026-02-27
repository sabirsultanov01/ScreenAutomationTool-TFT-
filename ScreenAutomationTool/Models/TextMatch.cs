using System.Drawing;

namespace ScreenAutomationTool.Models
{
    public class TextMatch
    {
        public string Text { get; set; } = string.Empty;
        public Rectangle Bounds { get; set; }
        public float Confidence { get; set; }

        public Point Center => new(
            Bounds.X + Bounds.Width / 2,
            Bounds.Y + Bounds.Height / 2
        );
    }
}