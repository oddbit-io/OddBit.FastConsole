namespace OddBit.FastConsole.GameEngine;

/// <summary>
/// Provides access to time-related values for the game loop.
/// </summary>
public static class Time
{
    /// <summary>
    /// The time in seconds since the last Update call.
    /// </summary>
    public static float DeltaTime { get; internal set; }

    /// <summary>
    /// The time in seconds since the last FixedUpdate call.
    /// </summary>
    public static float FixedDeltaTime { get; internal set; }
}