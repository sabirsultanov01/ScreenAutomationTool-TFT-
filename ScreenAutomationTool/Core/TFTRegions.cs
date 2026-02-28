using System.Drawing;

namespace ScreenAutomationTool.Core;

/// <summary>
/// Hardcoded screen regions and click targets for TFT at 1920×1080.
/// All coordinates assume native resolution with 100 % display scaling.
/// Run the bot once — it saves debug images to "debug/" on the first tick.
/// Use those images to fine-tune any region that reads incorrectly.
/// </summary>
public static class TFTRegions
{
    // ── HUD readouts (OCR source rectangles) ────────────────────────────
    //
    // LEVEL — we use a WIDE region covering "Lvl. X  XP/XP" and parse
    //   the level out with a regex, avoiding the pixel-exact-crop problem.
    // GOLD  — sits right of the coin icon, centre-bottom of the HUD.

    public static readonly Rectangle LevelWide = new(240, 868, 170, 28);
    public static readonly Rectangle Gold      = new(920, 868, 65, 28);
    public static readonly Rectangle Health    = new(180, 868, 40, 28);  // TODO: calibrate
    public static readonly Rectangle Stage     = new(770, 6, 65, 24);

    // ── Phase / timer region at top-center ──────────────────────────────

    public static readonly Rectangle PhaseRegion = new(754, 0, 160, 36);

    // ── Shop champion-name text regions (5 slots, left → right) ─────────
    //   y shifted to 1048 to align with actual name text at bottom of cards.

    public static readonly Rectangle[] ShopNames =
    [
        new(378,  1048, 150, 26),
        new(608,  1048, 150, 26),
        new(838,  1048, 150, 26),
        new(1068, 1048, 150, 26),
        new(1298, 1048, 150, 26),
    ];

    // ── Shop champion-cost text regions ─────────────────────────────────

    public static readonly Rectangle[] ShopCosts =
    [
        new(520,  1052, 25, 20),
        new(660,  1052, 25, 20),
        new(900,  1052, 25, 20),
        new(1130, 1052, 25, 20),
        new(1360, 1052, 25, 20),
    ];

    // ── Click targets: center of each shop card ─────────────────────────

    public static readonly Point[] ShopSlotCenters =
    [
        new(448,  990),
        new(678,  990),
        new(908,  990),
        new(1138, 990),
        new(1368, 990),
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
