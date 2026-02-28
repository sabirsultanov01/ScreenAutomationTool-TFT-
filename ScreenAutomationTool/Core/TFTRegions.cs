using System.Drawing;

namespace ScreenAutomationTool.Core;

/// <summary>
/// Hardcoded screen regions and click targets for TFT at 1920×1080.
/// All coordinates assume native resolution with 100 % display scaling.
/// Tweak values if your HUD layout differs (different TFT set / patch).
/// </summary>
public static class TFTRegions
{
    // ── HUD readouts (OCR source rectangles) ────────────────────────────

    public static readonly Rectangle Gold       = new(870, 878, 55, 24);
    public static readonly Rectangle Level      = new(318, 878, 30, 24);
    public static readonly Rectangle XpCurrent  = new(356, 878, 24, 24);
    public static readonly Rectangle XpNeeded   = new(382, 878, 24, 24);
    public static readonly Rectangle Health     = new(180, 878, 40, 24);
    public static readonly Rectangle Stage      = new(780, 8, 100, 24);

    // ── Phase / timer region at top-center ──────────────────────────────

    public static readonly Rectangle PhaseRegion = new(754, 0, 160, 36);

    // ── Shop champion-name text regions (5 slots, left → right) ─────────

    public static readonly Rectangle[] ShopNames =
    [
        new(378,  1026, 140, 22),
        new(608,  1026, 140, 22),
        new(838,  1026, 140, 22),
        new(1068, 1026, 140, 22),
        new(1298, 1026, 140, 22),
    ];

    // ── Shop champion-cost text regions ─────────────────────────────────

    public static readonly Rectangle[] ShopCosts =
    [
        new(425,  1050, 30, 18),
        new(655,  1050, 30, 18),
        new(885,  1050, 30, 18),
        new(1115, 1050, 30, 18),
        new(1345, 1050, 30, 18),
    ];

    // ── Click targets: center of each shop card ─────────────────────────

    public static readonly Point[] ShopSlotCenters =
    [
        new(448,  1000),
        new(678,  1000),
        new(908,  1000),
        new(1138, 1000),
        new(1368, 1000),
    ];

    // ── Buttons ─────────────────────────────────────────────────────────

    public static readonly Point BuyXpButton       = new(312, 966);
    public static readonly Point RefreshShopButton = new(312, 1040);

    // ── Bench slots (9 slots above the shop bar) ────────────────────────

    public static readonly Point[] BenchSlots =
    [
        new(418, 774),
        new(488, 774),
        new(558, 774),
        new(628, 774),
        new(698, 774),
        new(768, 774),
        new(838, 774),
        new(908, 774),
        new(978, 774),
    ];
}
