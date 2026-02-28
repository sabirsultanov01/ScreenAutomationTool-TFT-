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
    // The bottom HUD line sits at roughly y ≈ 876.
    // "Lvl. 3  4/6  75% …"  then gold in the center.
    //
    // Level number:  skip past "Lvl. " (~35 px) so OCR sees only the digit.
    // Gold number:   sits to the right of the gold-coin icon, centre-bottom.

    public static readonly Rectangle Gold       = new(942, 876, 50, 22);
    public static readonly Rectangle Level      = new(352, 876, 22, 22);
    public static readonly Rectangle XpCurrent  = new(382, 876, 20, 22);
    public static readonly Rectangle XpNeeded   = new(408, 876, 20, 22);
    public static readonly Rectangle Health     = new(180, 876, 40, 22);  // TODO: calibrate
    public static readonly Rectangle Stage      = new(770, 6, 65, 24);

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
