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

    public void Dispose()
    {
        _ocr.Dispose();
    }
}
