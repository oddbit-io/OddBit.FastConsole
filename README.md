# OddBit.FastConsole

An extremely simple and fast .NET console drawing library, designed for creating TUI applications and games.

![](https://github.com/oddbit-io/OddBit.FastConsole/blob/main/Oddbit.FastConsole.GameExample.gif)

## Why?

System.Console methods in .NET are slow and inefficient for real-time applications like games or animated interfaces. FastConsole bypasses the overhead of standard console operations by using WinAPI directly.

![](https://github.com/oddbit-io/OddBit.FastConsole/blob/main/Oddbit.FastConsole.GameExample1.gif)

## Features

- Fast console rendering (thousands frames per second)
- Simple API for drawing text and basic primitives

## Platform Support

✅ Tested on Windows 11 24H2<br>
⚠️ Should work with older Windows versions<br>
❌ Linux and macOS are not currently supported due to the reliance on WinAPI.

## Project Structure

This solution consists of 3 projects:

1. **OddBit.FastConsole** - The core library providing fast console drawing capabilities
2. **OddBit.FastConsole.GameEngine** - A simple game engine built on top of the library
3. **OddBit.FastConsole.GameExample** - An example game demonstrating library capabilities

## Example

```csharp
public static class Program
{
    public static void Main()
    {
        var chars = Enumerable.Range(32, 64).Select(x => (char)x).ToArray();
        var stopWatch = new Stopwatch();
        var frameCounter = 0;
        var fps = 0f;

        System.FastConsole.Initialize();
        stopWatch.Start();

        while (!Console.KeyAvailable)
        {
            for (var y = 0; y < System.FastConsole.Height; y++)
            {
                for (var x = 0; x < System.FastConsole.Width; x++)
                {
                    System.FastConsole.Write(x, y, chars[Random.Shared.Next(chars.Length)],
                        (ConsoleColor)Random.Shared.Next(0, 16),
                        (ConsoleColor)Random.Shared.Next(0, 16));
                }
            }

            System.FastConsole.Write(0, 0, $"FPS: {fps:F0}");
            System.FastConsole.Flush();

            frameCounter++;
            var elapsed = stopWatch.ElapsedMilliseconds;
            if (elapsed >= 1000f)
            {
                stopWatch.Restart();
                fps = frameCounter * 1000f / elapsed;
                frameCounter = 0;
            }
        }

        Console.ReadKey(true);
        Console.Clear();

        return;
    }
}
```

## License

MIT License - see LICENSE file for details.
