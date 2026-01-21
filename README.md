# OddBit.FastConsole

An extremely simple and fast .NET console drawing library, designed for creating TUI applications and games.

## Why?

System.Console methods in .NET are slow and inefficient for real-time applications like games or animated interfaces. FastConsole bypasses the overhead of standard console operations by using WinAPI directly.

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

## License

MIT License - see LICENSE file for details.
