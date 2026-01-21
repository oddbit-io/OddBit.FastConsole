namespace OddBit.FastConsole.GameEngine;

/// <summary>
/// Represents the settings for the console game, including dimensions, timing parameters, and rendering behavior.
/// </summary>
public class GameSettings
{
    /// <summary>
    /// Width of the game console in characters.
    /// </summary>
    public int Width { get; init; } = 80;

    /// <summary>
    /// Height of the game console in characters.
    /// </summary>
    public int Height { get; init; } = 25;

    /// <summary>
    /// Time step (in milliseconds) for rendering updates.
    /// </summary>
    public float RenderTimeStep { get; init; } = 0.0166666666666667f;

    /// <summary>
    /// Time step (in milliseconds) for game logic updates.
    /// </summary>
    public float LogicTimeStep { get; init; } = 0.0333333333333333f;

    /// <summary>
    /// Value indicating whether to skip missed render intervals when the system is lagging.
    /// </summary>
    public bool SkipMissedRenderIntervals { get; init; } = true;

    /// <summary>
    /// Default game settings instance.
    /// </summary>
    public static readonly GameSettings DefaultGameSettings = new();
}