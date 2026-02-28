using ScreenAutomationTool.Core;
using ScreenAutomationTool.Detection;
using ScreenAutomationTool.Input;
using ScreenAutomationTool.Services;
using ScreenAutomationTool.Strategy;

namespace ScreenAutomationTool;

/// <summary>
/// Main game loop: capture → read → decide → act.
/// Runs at ~800 ms during planning and ~2 000 ms during combat.
/// </summary>
public sealed class BotEngine : IDisposable
{
    private readonly ScreenCaptureService _capture;
    private readonly GameStateReader      _reader;
    private readonly InputController      _input;
    private readonly StrategyEngine       _strategy;
    private CancellationTokenSource?      _cts;

    public event Action<string>?    OnLog;
    public event Action<GameState>? OnStateUpdate;

    public bool IsRunning => _cts is { IsCancellationRequested: false };

    public BotEngine(
        IEnumerable<string> wantedChampions,
        string tessDataPath       = "./tessdata",
        int    econFloor          = 50,
        bool   aggressiveLeveling = false)
    {
        _capture  = new ScreenCaptureService();
        _reader   = new GameStateReader(tessDataPath);
        _input    = new InputController();
        _strategy = new StrategyEngine(wantedChampions, econFloor, aggressiveLeveling);
    }

    // ── Lifecycle ───────────────────────────────────────────────────────

    public async Task RunAsync()
    {
        _cts = new CancellationTokenSource();
        var ct = _cts.Token;

        Log("Bot started — entering main loop.");

        try
        {
            while (!ct.IsCancellationRequested)
            {
                using var screenshot = _capture.CaptureScreen();
                var state = _reader.Read(screenshot);
                OnStateUpdate?.Invoke(state);

                Log($"[{state.Stage}] Gold:{state.Gold} Lv:{state.Level} " +
                    $"HP:{state.Health} Phase:{state.Phase}");

                switch (state.Phase)
                {
                    case GamePhase.Planning:
                        await HandlePlanningAsync(state, ct);
                        await Task.Delay(800, ct);
                        break;

                    case GamePhase.Combat:
                        await Task.Delay(2000, ct);
                        break;

                    case GamePhase.Carousel:
                        Log("Carousel detected — idle.");
                        await Task.Delay(3000, ct);
                        break;

                    case GamePhase.Augment:
                        Log("Augment selection detected — idle.");
                        await Task.Delay(5000, ct);
                        break;

                    default:
                        await Task.Delay(1000, ct);
                        break;
                }
            }
        }
        catch (OperationCanceledException) { /* expected on Stop */ }

        Log("Bot session ended.");
    }

    public void Stop()
    {
        _cts?.Cancel();
        Log("Stop requested.");
    }

    // ── Planning phase logic ────────────────────────────────────────────

    private async Task HandlePlanningAsync(GameState state, CancellationToken ct)
    {
        // 1. Buy wanted champions.
        var buys = _strategy.GetBuyDecisions(state);
        foreach (var buy in buys)
        {
            ct.ThrowIfCancellationRequested();
            Log($"  Buy: {buy.ChampionName} (slot {buy.SlotIndex + 1}, {buy.Cost}g)");
            await _input.BuyChampionAsync(buy.SlotIndex, ct);
        }

        // 2. Level up if appropriate.
        if (_strategy.ShouldLevelUp(state))
        {
            Log($"  Level up (Lv{state.Level}, {state.Gold}g)");
            await _input.BuyXpAsync(ct);
        }

        // 3. Refresh shop if appropriate.
        if (_strategy.ShouldRefreshShop(state))
        {
            Log($"  Refresh shop ({state.Gold}g left)");
            await _input.RefreshShopAsync(ct);
        }
    }

    // ── Helpers ─────────────────────────────────────────────────────────

    private void Log(string message)
    {
        OnLog?.Invoke($"[{DateTime.Now:HH:mm:ss}] {message}");
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _reader.Dispose();
    }
}
