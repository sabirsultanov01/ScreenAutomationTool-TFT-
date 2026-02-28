using System.Drawing;
using ScreenAutomationTool.Core;
using ScreenAutomationTool.Helpers;

namespace ScreenAutomationTool.Input;

/// <summary>
/// Async input controller that wraps user32.dll calls with randomised
/// pixel offsets and human-like delays to avoid detection.
/// </summary>
public sealed class InputController
{
    private static readonly Random Rng = new();

    private readonly int _minDelayMs;
    private readonly int _maxDelayMs;
    private readonly int _offsetPx;

    public InputController(int minDelayMs = 50, int maxDelayMs = 250, int offsetPixels = 5)
    {
        _minDelayMs = minDelayMs;
        _maxDelayMs = maxDelayMs;
        _offsetPx   = offsetPixels;
    }

    // ── Basic actions ───────────────────────────────────────────────────

    public async Task ClickAsync(Point target, CancellationToken ct = default)
    {
        var (x, y) = Jitter(target);
        NativeMethods.SetCursorPos(x, y);
        await RandomDelayAsync(ct);

        NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        await Task.Delay(Rng.Next(15, 35), ct);
        NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTUP,   0, 0, 0, 0);

        await RandomDelayAsync(ct);
    }

    public async Task RightClickAsync(Point target, CancellationToken ct = default)
    {
        var (x, y) = Jitter(target);
        NativeMethods.SetCursorPos(x, y);
        await RandomDelayAsync(ct);

        NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
        await Task.Delay(Rng.Next(15, 35), ct);
        NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_RIGHTUP,   0, 0, 0, 0);

        await RandomDelayAsync(ct);
    }

    public async Task DragAsync(Point from, Point to, CancellationToken ct = default)
    {
        var (fx, fy) = Jitter(from);
        var (tx, ty) = Jitter(to);

        NativeMethods.SetCursorPos(fx, fy);
        await Task.Delay(Rng.Next(30, 80), ct);

        NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);

        // Smooth interpolation so the drag looks human.
        int steps = Rng.Next(8, 15);
        for (int i = 1; i <= steps; i++)
        {
            ct.ThrowIfCancellationRequested();
            float t  = (float)i / steps;
            int   cx = (int)(fx + (tx - fx) * t);
            int   cy = (int)(fy + (ty - fy) * t);
            NativeMethods.SetCursorPos(cx, cy);
            await Task.Delay(Rng.Next(10, 25), ct);
        }

        await Task.Delay(Rng.Next(30, 80), ct);
        NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        await RandomDelayAsync(ct);
    }

    // ── TFT-specific shortcuts ──────────────────────────────────────────

    public async Task BuyChampionAsync(int slotIndex, CancellationToken ct = default)
    {
        if (slotIndex is < 0 or > 4) return;
        await ClickAsync(TFTRegions.ShopSlotCenters[slotIndex], ct);
    }

    public async Task BuyXpAsync(CancellationToken ct = default)
    {
        await ClickAsync(TFTRegions.BuyXpButton, ct);
    }

    public async Task RefreshShopAsync(CancellationToken ct = default)
    {
        await ClickAsync(TFTRegions.RefreshShopButton, ct);
    }

    public async Task SellUnitAsync(Point unitPosition, CancellationToken ct = default)
    {
        await ClickAsync(unitPosition, ct);
        await Task.Delay(Rng.Next(50, 100), ct);

        // Press 'E' to sell the selected unit.
        NativeMethods.keybd_event(0x45, 0, 0, 0);
        await Task.Delay(Rng.Next(15, 35), ct);
        NativeMethods.keybd_event(0x45, 0, NativeMethods.KEYEVENTF_KEYUP, 0);

        await RandomDelayAsync(ct);
    }

    public async Task PlaceUnitAsync(Point benchSlot, Point boardSlot, CancellationToken ct = default)
    {
        await DragAsync(benchSlot, boardSlot, ct);
    }

    // ── Helpers ─────────────────────────────────────────────────────────

    private (int x, int y) Jitter(Point p)
    {
        int dx = Rng.Next(-_offsetPx, _offsetPx + 1);
        int dy = Rng.Next(-_offsetPx, _offsetPx + 1);
        return (p.X + dx, p.Y + dy);
    }

    private async Task RandomDelayAsync(CancellationToken ct)
    {
        await Task.Delay(Rng.Next(_minDelayMs, _maxDelayMs + 1), ct);
    }
}
