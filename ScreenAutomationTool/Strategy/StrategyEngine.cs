using ScreenAutomationTool.Core;

namespace ScreenAutomationTool.Strategy;

public sealed class BuyDecision
{
    public required int    SlotIndex    { get; init; }
    public required string ChampionName { get; init; }
    public required int    Cost         { get; init; }
}

/// <summary>
/// Economy-aware decision engine.
/// Decides which shop champions to buy, whether to level up, and
/// whether to refresh the shop, respecting interest-gold thresholds.
/// </summary>
public sealed class StrategyEngine
{
    private readonly HashSet<string> _wanted;
    private readonly int  _econFloor;
    private readonly bool _aggressiveLeveling;

    public StrategyEngine(
        IEnumerable<string> wantedChampions,
        int  econFloor          = 50,
        bool aggressiveLeveling = false)
    {
        _wanted             = new HashSet<string>(wantedChampions, StringComparer.OrdinalIgnoreCase);
        _econFloor          = econFloor;
        _aggressiveLeveling = aggressiveLeveling;
    }

    public IReadOnlySet<string> WantedChampions => _wanted;

    public void AddWanted(string name)    => _wanted.Add(name);
    public void RemoveWanted(string name) => _wanted.Remove(name);

    // ── Buy decision ────────────────────────────────────────────────────

    /// <summary>
    /// Returns the shop-slot indices worth purchasing this tick.
    /// Won't spend below the economy floor unless health is critical.
    /// </summary>
    public List<BuyDecision> GetBuyDecisions(GameState state)
    {
        var decisions = new List<BuyDecision>();
        int goldLeft  = state.Gold;

        for (int i = 0; i < state.Shop.Length; i++)
        {
            var slot = state.Shop[i];
            if (slot.IsEmpty || slot.Cost <= 0)
                continue;

            if (!IsWanted(slot.ChampionName))
                continue;

            int after = goldLeft - slot.Cost;

            // Comfortable buy – stay above econ floor.
            if (after >= _econFloor)
            {
                decisions.Add(new BuyDecision
                {
                    SlotIndex    = i,
                    ChampionName = slot.ChampionName,
                    Cost         = slot.Cost,
                });
                goldLeft = after;
                continue;
            }

            // Desperation buy – only when health is critical.
            if (state.Health <= 30 && after >= 0)
            {
                decisions.Add(new BuyDecision
                {
                    SlotIndex    = i,
                    ChampionName = slot.ChampionName,
                    Cost         = slot.Cost,
                });
                goldLeft = after;
            }
        }

        return decisions;
    }

    // ── Level-up decision ───────────────────────────────────────────────

    /// <summary>Should the bot buy 4 XP this tick?</summary>
    public bool ShouldLevelUp(GameState state)
    {
        if (state.Gold < 4)
            return false;

        int target = GetTargetLevel(state.Stage, state.Level);

        if (state.Level < target && state.Gold >= LevelGoldThreshold(state.Level))
            return true;

        if (_aggressiveLeveling && state.Gold >= _econFloor + 4)
            return true;

        return false;
    }

    // ── Roll decision ───────────────────────────────────────────────────

    /// <summary>Should the bot spend 2 g to refresh the shop?</summary>
    public bool ShouldRefreshShop(GameState state)
    {
        // Don't roll early.
        if (state.Level < 6)
            return false;

        // Roll above econ floor + 2 (keep floor for interest).
        if (state.Gold >= _econFloor + 2)
            return true;

        // Desperation rolling at low HP.
        if (state.Health <= 30 && state.Gold >= 2)
            return true;

        return false;
    }

    // ── Internals ───────────────────────────────────────────────────────

    private bool IsWanted(string ocrName)
    {
        foreach (var w in _wanted)
        {
            if (ocrName.Contains(w, StringComparison.OrdinalIgnoreCase) ||
                w.Contains(ocrName, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Standard TFT leveling curve — returns the target level for
    /// the current stage.  Order matters: check higher rounds first.
    /// </summary>
    private static int GetTargetLevel(string stageText, int currentLevel)
    {
        var parts = stageText.Split('-', StringSplitOptions.TrimEntries);
        if (parts.Length != 2 ||
            !int.TryParse(parts[0], out int s) ||
            !int.TryParse(parts[1], out int r))
        {
            return currentLevel;  // Can't parse → don't force-level.
        }

        return s switch
        {
            2 when r >= 5 => 5,
            2             => 4,
            3 when r >= 5 => 7,
            3 when r >= 2 => 6,
            3             => 5,
            4 when r >= 5 => 8,
            4             => 7,
            >= 5          => 8,
            _             => currentLevel,
        };
    }

    /// <summary>
    /// Minimum gold the bot should have before buying XP at this level.
    /// </summary>
    private int LevelGoldThreshold(int level) => level switch
    {
        <= 4 => 4,
        5    => 8,
        6    => 12,
        7    => 24,
        8    => 36,
        _    => _econFloor,
    };
}
