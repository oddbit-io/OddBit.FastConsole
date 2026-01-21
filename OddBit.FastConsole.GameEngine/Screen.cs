namespace OddBit.FastConsole.GameEngine;

/// <summary>
/// Provides access to the game screen dimensions.
/// </summary>
public static class Screen
{
    /// <summary>
    /// Gets the width of the game screen.
    /// </summary>
    public static int Width => Game.Instance?.Width ?? 0;

    /// <summary>
    /// Gets the height of the game screen.
    /// </summary>
    public static int Height => Game.Instance?.Height ?? 0;
}