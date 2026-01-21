using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using QuickTickLib;

namespace OddBit.FastConsole.GameEngine;

/// <summary>
/// Represents the main game class that handles game loop, rendering, and logic updates.
/// </summary>
public class Game
{
    /// <summary>
    /// Gets the singleton instance of the Game.
    /// </summary>
    public static Game? Instance { get; private set; }

    /// <summary>
    /// Gets or sets whether to display FPS counter on screen.
    /// </summary>
    public static bool ShowFps { get; set; }

    /// <summary>
    /// Gets the width of the game window.
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    /// Gets the height of the game window.
    /// </summary>
    public int Height { get; private set; }

    private readonly GameSettings _settings;
    private readonly Stopwatch _sw = new();
    private readonly Lock _flushLock = new();
    private CancellationTokenSource? _cts;
    private int _frameCounter;
    private float _fps;
    private Scene? _currentScene;

    /// <summary>
    /// Initializes a new instance of the Game class with optional settings.
    /// </summary>
    /// <param name="settings">The game settings to use. If null, default settings will be used.</param>
    public Game(GameSettings? settings = null)
    {
        _settings = settings ?? GameSettings.DefaultGameSettings;
        Console.OutputEncoding = Encoding.Unicode;
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
        Instance = this;
        Width = _settings.Width;
        Height = _settings.Height;
    }

    /// <summary>
    /// Starts the game loop with the specified scene.
    /// </summary>
    /// <param name="scene">The scene to run.</param>
    public void Run(Scene scene)
    {
        _frameCounter = 0;
        _fps = 0;
        _cts = new CancellationTokenSource();
        try
        {
            System.FastConsole.Initialize(_settings.Width, _settings.Height);

            using QuickTickTimer logicTimer = new QuickTickTimer(1f / _settings.LogicTimeStep);
            logicTimer.AutoReset = true;
            logicTimer.Elapsed += FixedUpdate;
            logicTimer.SkipMissedIntervals = false;

            using QuickTickTimer renderTimer = new QuickTickTimer(1f / _settings.RenderTimeStep);
            renderTimer.AutoReset = true;
            renderTimer.Elapsed += Update;
            renderTimer.SkipMissedIntervals = _settings.SkipMissedRenderIntervals;

            _sw.Restart();

            lock (_flushLock)
            {
                _currentScene = scene;
                _currentScene.Start();
            }

            logicTimer.Start();
            renderTimer.Start();

            _cts.Token.WaitHandle.WaitOne();
            logicTimer.Stop();
            renderTimer.Stop();

            lock (_flushLock)
            {
                _currentScene.Stop();
            }

            System.FastConsole.Reset();
        }
#if !DEBUG
        catch (Exception e)
        {
            System.FastConsole.Reset();
            Console.WriteLine(e.Message);
        }
#endif
        finally
        {
            _cts.Dispose();
        }
    }

    /// <summary>
    /// Stops the game loop and exits the application.
    /// </summary>
    public static void Quit()
    {
        Instance?._cts?.Cancel();
    }

    /// <summary>
    /// Updates the game state at fixed intervals (physics, logic updates).
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event arguments containing timing information.</param>
    private void FixedUpdate(object? sender, QuickTickElapsedEventArgs e)
    {
        Time.FixedDeltaTime = (float)e.TimeSinceLastInterval.TotalSeconds;

        lock (_flushLock)
        {
            _currentScene?.FixedUpdate();
        }
    }

    /// <summary>
    /// Updates the game display and handles rendering.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event arguments containing timing information.</param>
    private void Update(object? sender, QuickTickElapsedEventArgs e)
    {
        lock (_flushLock)
        {
            if (System.FastConsole.UpdateBufferSize())
            {
                Width = System.FastConsole.Width;
                Height = System.FastConsole.Height;
            }

            System.FastConsole.Clear();

            Time.DeltaTime = (float)e.TimeSinceLastInterval.TotalSeconds;
            _currentScene?.Update();

            if (ShowFps)
            {
                System.FastConsole.Write(0, 0, $"FPS: {_fps:F0}");
            }

            System.FastConsole.Flush();
        }

        _frameCounter++;
        var elapsed = _sw.ElapsedMilliseconds;
        if (elapsed >= 250)
        {
            _sw.Restart();

            _fps = _frameCounter * 1000f / elapsed;
            Debug.WriteLine($"FPS: {_fps:F2}");

            _frameCounter = 0;
        }
    }
}