using System;
using OddBit.FastConsole.GameEngine;

namespace OddBit.FastConsole.GameExample;

public class BallsScene : Scene
{
    public BallsScene()
    {
        // Add input handler to process user input
        AddGameObject(new InputHandler());

        // Create 200 ball game objects
        for (int i = 0; i < 200; i++)
        {
            AddGameObject(new Ball());
        }
    }

    public override void Update()
    {
        // Draw a bordered rectangle that fills the entire screen
        FastConsoleHelper.DrawRectangle(0, 0, Screen.Width, Screen.Height, true, FastConsoleHelper.LineWidth.Double,
            ConsoleColor.Cyan, ConsoleColor.DarkBlue);

        // Call base Update method to handle scene updates
        base.Update();
    }
}