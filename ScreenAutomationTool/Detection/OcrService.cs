using System.Drawing;
using OpenCvSharp;
using Tesseract;

using SdImageFormat = System.Drawing.Imaging.ImageFormat;

namespace ScreenAutomationTool.Detection;

/// <summary>
/// Tesseract OCR wrapper with OpenCV pre-processing tuned for the TFT HUD.
/// All public methods are designed for sequential (single-threaded) use;
/// the char-whitelist is swapped before every Process() call.
/// </summary>
public sealed class OcrService : IDisposable
{
    private readonly TesseractEngine _engine;

    public OcrService(string tessDataPath = "./tessdata", string language = "eng")
    {
        _engine = new TesseractEngine(tessDataPath, language, EngineMode.Default);
    }

    // ── Public API ──────────────────────────────────────────────────────

    /// <summary>Reads an integer from a small HUD region (gold, level, hp …).</summary>
    public int ReadNumber(Bitmap region)
    {
        using var preprocessed = PreprocessForNumbers(region);
        using var pix = BitmapToPix(preprocessed);

        _engine.SetVariable("tessedit_char_whitelist", "0123456789");
        using var page = _engine.Process(pix, PageSegMode.SingleLine);

        var text = page.GetText().Trim();
        return int.TryParse(text, out var v) ? v : -1;
    }

    /// <summary>Reads a champion name from a shop-card text strip.</summary>
    public string ReadChampionName(Bitmap region)
    {
        using var preprocessed = PreprocessForText(region);
        using var pix = BitmapToPix(preprocessed);

        _engine.SetVariable("tessedit_char_whitelist",
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz '.-");
        using var page = _engine.Process(pix, PageSegMode.SingleLine);

        return page.GetText().Trim();
    }

    /// <summary>General-purpose text read (stage indicator, phase text …).</summary>
    public string ReadText(Bitmap region)
    {
        using var preprocessed = PreprocessForText(region);
        using var pix = BitmapToPix(preprocessed);

        _engine.SetVariable("tessedit_char_whitelist", "");   // all characters
        using var page = _engine.Process(pix, PageSegMode.SingleLine);

        return page.GetText().Trim();
    }

    // ── Pre-processing pipelines ────────────────────────────────────────

    /// <summary>
    /// Numbers pipeline: grayscale → 3× upscale → high threshold (180)
    /// → invert (black-on-white for Tesseract).
    /// </summary>
    private static Bitmap PreprocessForNumbers(Bitmap source)
    {
        using var mat    = BitmapToMat(source);
        using var gray   = new Mat();
        using var scaled = new Mat();
        using var binary = new Mat();
        using var inv    = new Mat();

        Cv2.CvtColor(mat, gray, ColorConversionCodes.BGR2GRAY);
        Cv2.Resize(gray, scaled,
            new OpenCvSharp.Size(gray.Width * 3, gray.Height * 3),
            interpolation: InterpolationFlags.Cubic);
        Cv2.Threshold(scaled, binary, 180, 255, ThresholdTypes.Binary);
        Cv2.BitwiseNot(binary, inv);

        return MatToBitmap(inv);
    }

    /// <summary>
    /// Text pipeline: grayscale → 3× upscale → moderate threshold (150)
    /// → invert.
    /// </summary>
    private static Bitmap PreprocessForText(Bitmap source)
    {
        using var mat    = BitmapToMat(source);
        using var gray   = new Mat();
        using var scaled = new Mat();
        using var binary = new Mat();
        using var inv    = new Mat();

        Cv2.CvtColor(mat, gray, ColorConversionCodes.BGR2GRAY);
        Cv2.Resize(gray, scaled,
            new OpenCvSharp.Size(gray.Width * 3, gray.Height * 3),
            interpolation: InterpolationFlags.Cubic);
        Cv2.Threshold(scaled, binary, 150, 255, ThresholdTypes.Binary);
        Cv2.BitwiseNot(binary, inv);

        return MatToBitmap(inv);
    }

    // ── Format conversions ──────────────────────────────────────────────

    private static Mat BitmapToMat(Bitmap bitmap)
    {
        using var ms = new MemoryStream();
        bitmap.Save(ms, SdImageFormat.Png);
        return Cv2.ImDecode(ms.ToArray(), ImreadModes.Color);
    }

    private static Bitmap MatToBitmap(Mat mat)
    {
        Cv2.ImEncode(".png", mat, out var buf);
        using var ms = new MemoryStream(buf);
        return new Bitmap(ms);
    }

    private static Pix BitmapToPix(Bitmap bitmap)
    {
        using var ms = new MemoryStream();
        bitmap.Save(ms, SdImageFormat.Png);
        return Pix.LoadFromMemory(ms.ToArray());
    }

    public void Dispose()
    {
        _engine.Dispose();
    }
}
