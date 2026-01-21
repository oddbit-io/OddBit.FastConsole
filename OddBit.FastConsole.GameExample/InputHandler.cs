using System;
using OddBit.FastConsole.GameEngine;

namespace OddBit.FastConsole.GameExample;

public class InputHandler : GameObject
{
    public override void Update()
    {
        // Check if a key is available in the console input buffer
        if (!Console.KeyAvailable)
            return;
        
        // Read the next key from the console input buffer without displaying it
        switch (Console.ReadKey(true).Key)
        {
            case ConsoleKey.Spacebar:
                // Toggle the display of FPS counter
                Game.ShowFps = !Game.ShowFps;
                break;
            case ConsoleKey.Escape:
                // Quit the game
                Game.Quit();
                break;
        }
    }
}