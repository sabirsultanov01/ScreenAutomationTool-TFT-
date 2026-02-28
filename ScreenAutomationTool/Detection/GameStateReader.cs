using System.Drawing;
using ScreenAutomationTool.Core;

namespace ScreenAutomationTool.Detection;

/// <summary>
/// Reads the full TFT game state from a single 1920×1080 screenshot
/// by cropping HUD regions and running them through <see cref="OcrService"/>.
/// </summary>
public sealed class GameStateReader : IDisposable
{
    private readonly OcrService _ocr;

    public GameStateReader(string tessDataPath = "./tessdata", string language = "eng")
    {
        _ocr = new OcrService(tessDataPath, language);
    }

    public GameState Read(Bitmap screenshot)
    {
        return new GameState
        {
            Gold      = ReadInt(screenshot, TFTRegions.Gold),
            Level     = ReadInt(screenshot, TFTRegions.Level),
            Health    = ReadInt(screenshot, TFTRegions.Health),
            XpCurrent = ReadInt(screenshot, TFTRegions.XpCurrent),
            XpNeeded  = ReadInt(screenshot, TFTRegions.XpNeeded),
            Stage     = ReadStage(screenshot),
            Phase     = DetectPhase(screenshot),
            Shop      = ReadShop(screenshot),
            Timestamp = DateTime.UtcNow,
        };
    }

    // ── Debug / calibration ─────────────────────────────────────────────

    /// <summary>
    /// Saves an annotated screenshot (all regions drawn as coloured
    /// rectangles), individual crops, <b>and</b> a text file with the
    /// OCR results for every champion-name slot to <paramref name="outputDir"/>.
    /// Use this to verify that every region is aimed at the correct
    /// HUD element, then adjust <see cref="TFTRegions"/> as needed.
    /// </summary>
    public void SaveDebugCapture(Bitmap screenshot, string outputDir = "debug")
    {
        Directory.CreateDirectory(outputDir);

        // ── Annotated full screenshot ───────────────────────────────
        using (var annotated = new Bitmap(screenshot))
        {
            using var g = Graphics.FromImage(annotated);

            DrawRect(g, TFTRegions.Gold,      "Gold",  Color.Yellow);
            DrawRect(g, TFTRegions.Level,     "Level", Color.Cyan);
            DrawRect(g, TFTRegions.Health,    "HP",    Color.Red);
            DrawRect(g, TFTRegions.Stage,     "Stage", Color.Magenta);
            DrawRect(g, TFTRegions.XpCurrent, "XP",    Color.LimeGreen);
            DrawRect(g, TFTRegions.XpNeeded,  "XP2",   Color.LimeGreen);
            DrawRect(g, TFTRegions.PhaseRegion, "Phase", Color.White);

            for (int i = 0; i < 5; i++)
            {
                DrawRect(g, TFTRegions.ShopNames[i], $"Name{i + 1}", Color.Orange);
                DrawRect(g, TFTRegions.ShopCosts[i], $"Cost{i + 1}", Color.Orange);
            }

            annotated.Save(Path.Combine(outputDir, "annotated.png"));
        }

        // ── Individual region crops ─────────────────────────────────
        SaveCrop(screenshot, TFTRegions.Gold,      outputDir, "gold.png");
        SaveCrop(screenshot, TFTRegions.Level,     outputDir, "level.png");
        SaveCrop(screenshot, TFTRegions.Health,    outputDir, "health.png");
        SaveCrop(screenshot, TFTRegions.Stage,     outputDir, "stage.png");
        SaveCrop(screenshot, TFTRegions.XpCurrent, outputDir, "xp_current.png");
        SaveCrop(screenshot, TFTRegions.XpNeeded,  outputDir, "xp_needed.png");

        for (int i = 0; i < 5; i++)
        {
            SaveCrop(screenshot, TFTRegions.ShopNames[i], outputDir, $"shop_name_{i + 1}.png");
            SaveCrop(screenshot, TFTRegions.ShopCosts[i], outputDir, $"shop_cost_{i + 1}.png");
        }

        // ── OCR results text file ───────────────────────────────────
        var lines = new List<string>
        {
            $"Gold:      {ReadInt(screenshot, TFTRegions.Gold)}",
            $"Level:     {ReadInt(screenshot, TFTRegions.Level)}",
            $"Health:    {ReadInt(screenshot, TFTRegions.Health)}",
            $"XP:        {ReadInt(screenshot, TFTRegions.XpCurrent)} / {ReadInt(screenshot, TFTRegions.XpNeeded)}",
            $"Stage:     {ReadStage(screenshot)}",
            "",
        };

        for (int i = 0; i < 5; i++)
        {
            using var nameCrop = CropRegion(screenshot, TFTRegions.ShopNames[i]);
            using var costCrop = CropRegion(screenshot, TFTRegions.ShopCosts[i]);

            var name = _ocr.ReadChampionName(nameCrop);
            var cost = _ocr.ReadNumber(costCrop);
            lines.Add($"Shop slot {i + 1}:  \"{name}\"  (cost: {cost})");
        }

        File.WriteAllLines(Path.Combine(outputDir, "ocr_results.txt"), lines);
    }

    // ── Private helpers ─────────────────────────────────────────────────

    private int ReadInt(Bitmap screenshot, Rectangle region)
    {
        using var crop = CropRegion(screenshot, region);
        return _ocr.ReadNumber(crop);
    }

    private string ReadStage(Bitmap screenshot)
    {
        using var crop = CropRegion(screenshot, TFTRegions.Stage);
        return _ocr.ReadText(crop);
    }

    private GamePhase DetectPhase(Bitmap screenshot)
    {
        using var crop = CropRegion(screenshot, TFTRegions.PhaseRegion);
        var text = _ocr.ReadText(crop).ToLowerInvariant();

        if (text.Contains("carousel") || text.Contains("shared"))
            return GamePhase.Carousel;
        if (text.Contains("augment") || text.Contains("choose"))
            return GamePhase.Augment;
        if (text.Contains("plan"))
            return GamePhase.Planning;
        if (text.Contains("combat") || text.Contains("fight"))
            return GamePhase.Combat;

        // Fallback heuristic: if the shop area contains readable champion
        // names, we are very likely in the planning phase.
        for (int i = 0; i < TFTRegions.ShopNames.Length; i++)
        {
            using var nameCrop = CropRegion(screenshot, TFTRegions.ShopNames[i]);
            var name = _ocr.ReadChampionName(nameCrop);
            if (name.Length >= 3)
                return GamePhase.Planning;
        }

        return GamePhase.Unknown;
    }

    private ShopSlot[] ReadShop(Bitmap screenshot)
    {
        var shop = new ShopSlot[5];

        for (int i = 0; i < 5; i++)
        {
            using var nameCrop = CropRegion(screenshot, TFTRegions.ShopNames[i]);
            using var costCrop = CropRegion(screenshot, TFTRegions.ShopCosts[i]);

            var name = _ocr.ReadChampionName(nameCrop);
            var cost = _ocr.ReadNumber(costCrop);

            shop[i] = new ShopSlot
            {
                ChampionName = name,
                Cost         = cost > 0 ? cost : 0,
            };
        }

        return shop;
    }

    private static Bitmap CropRegion(Bitmap source, Rectangle region)
    {
        int x = Math.Clamp(region.X, 0, source.Width  - 1);
        int y = Math.Clamp(region.Y, 0, source.Height - 1);
        int w = Math.Min(region.Width,  source.Width  - x);
        int h = Math.Min(region.Height, source.Height - y);

        return source.Clone(new Rectangle(x, y, w, h), source.PixelFormat);
    }

    // ── Debug drawing helpers ───────────────────────────────────────────

    private static void DrawRect(Graphics g, Rectangle r, string label, Color color)
    {
        using var pen   = new Pen(color, 2);
        using var font  = new Font("Arial", 10, FontStyle.Bold);
        using var brush = new SolidBrush(color);

        g.DrawRectangle(pen, r);
        g.DrawString(label, font, brush, r.X, Math.Max(0, r.Y - 16));
    }

    private static void SaveCrop(Bitmap source, Rectangle region, string dir, string name)
    {
        int x = Math.Clamp(region.X, 0, source.Width  - 1);
        int y = Math.Clamp(region.Y, 0, source.Height - 1);
        int w = Math.Min(region.Width,  source.Width  - x);
        int h = Math.Min(region.Height, source.Height - y);

        using var crop = source.Clone(new Rectangle(x, y, w, h), source.PixelFormat);
        crop.Save(Path.Combine(dir, name));
    }

    public void Dispose()
    {
        _ocr.Dispose();
    }
}
