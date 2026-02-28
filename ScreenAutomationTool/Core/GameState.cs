namespace ScreenAutomationTool.Core;

public enum GamePhase
{
    Unknown,
    Planning,
    Combat,
    Carousel,
    Augment,
}

public sealed class ShopSlot
{
    public string ChampionName { get; init; } = string.Empty;
    public int Cost { get; init; }
    public bool IsEmpty => string.IsNullOrWhiteSpace(ChampionName);
}

public sealed class GameState
{
    public int Gold { get; init; }
    public int Level { get; init; }
    public int Health { get; init; }
    public int XpCurrent { get; init; }
    public int XpNeeded { get; init; }
    public string Stage { get; init; } = string.Empty;
    public GamePhase Phase { get; init; }
    public ShopSlot[] Shop { get; init; } = [new(), new(), new(), new(), new()];
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
