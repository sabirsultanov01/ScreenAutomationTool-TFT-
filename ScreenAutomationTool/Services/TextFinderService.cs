using ScreenAutomationTool.Interfaces;
using ScreenAutomationTool.Interfaces;
using ScreenAutomationTool.Models;
using System.Drawing;
using System.Drawing.Imaging;
using Tesseract;

namespace ScreenAutomationTool.Services
{
    public class TextFinderService : ITextFinderService
    {
        private readonly TesseractEngine _engine;

        public TextFinderService(string tessDataPath = "./tessdata", string language = "eng")
        {
            _engine = new TesseractEngine(tessDataPath, language, EngineMode.Default);
        }

        public List<TextMatch> FindText(Bitmap screenshot, string keyword)
        {
            var matches = new List<TextMatch>();

            using var pix = BitmapToPix(screenshot);
            using var page = _engine.Process(pix);
            using var iter = page.GetIterator();

            iter.Begin();
            do
            {
                if (iter.TryGetBoundingBox(PageIteratorLevel.Word, out var rect))
                {
                    string word = iter.GetText(PageIteratorLevel.Word)?.Trim() ?? "";
                    float confidence = iter.GetConfidence(PageIteratorLevel.Word);

                    if (word.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        matches.Add(new TextMatch
                        {
                            Text = word,
                            Confidence = confidence,
                            Bounds = new Rectangle(rect.X1, rect.Y1,
                                                   rect.X2 - rect.X1, rect.Y2 - rect.Y1)
                        });
                    }
                }
            } while (iter.Next(PageIteratorLevel.Word));

            return matches;
        }

        public string ReadAllText(Bitmap screenshot)
        {
            using var pix = BitmapToPix(screenshot);
            using var page = _engine.Process(pix);
            return page.GetText();
        }

        private static Pix BitmapToPix(Bitmap bitmap)
        {
            using var ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;
            return Pix.LoadFromMemory(ms.ToArray());
        }

        public void Dispose()
        {
            _engine?.Dispose();
        }
    }
}