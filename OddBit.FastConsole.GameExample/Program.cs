using System;
using OddBit.FastConsole.GameEngine;

namespace OddBit.FastConsole.GameExample;

public static class Program
{
    public static void Main()
    {
        // Check if the current platform is supported by FastConsole
        if (!System.FastConsole.IsPlatformSupported)
        {
            Console.WriteLine("Platform not supported!");
            Environment.ExitCode = 1;
            return;
        }

        try
        {
            // Hide the console cursor for better visual experience
            Console.CursorVisible = false;
            Console.Clear();

            // Create a new game instance with specific timing settings
            var game = new Game(new GameSettings()
            {
                // Set render time step to achieve 60 frames per second (approximately 16.67ms)
                RenderTimeStep = 0.0166666666666667f,
                // Set logic time step to achieve 30 frames per second (approximately 33.33ms)
                LogicTimeStep = 0.0333333333333333f,
                // Enable skipping of missed render intervals for smoother gameplay
                SkipMissedRenderIntervals = true,
            });

            // Create a new instance of the BallsScene
            var scene = new BallsScene();

            // Start running the game with the specified scene
            game.Run(scene);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Environment.ExitCode = 2;
        }
        finally
        {
            // Restore cursor visibility when program exits
            Console.CursorVisible = true;
            Console.Clear();
        }

        Environment.ExitCode = 0;
    }
}