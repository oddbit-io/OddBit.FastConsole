using System;

namespace OddBit.FastConsole.GameExample;

public static class FastConsoleHelper
{
    private static readonly char[] SingleLineRectChars = ['┌', '┐', '└', '┘', '─', '│'];
    private static readonly char[] DoubleLineRectChars = ['╔', '╗', '╚', '╝', '═', '║'];
    private static readonly char[] BoldLineRectChars = ['┏', '┓', '┗', '┛', '━', '┃'];

    public enum LineWidth
    {
        Single,
        Double,
        Bold
    }

    private static char[] GetLineChars(LineWidth lineWidth)
    {
        return lineWidth switch
        {
            LineWidth.Single => SingleLineRectChars,
            LineWidth.Double => DoubleLineRectChars,
            LineWidth.Bold => BoldLineRectChars,
            _ => throw new ArgumentOutOfRangeException(nameof(lineWidth), lineWidth, null)
        };
    }
    
    /// <summary>
    ///     Draws a vertical line in the console buffer.
    /// </summary>
    /// <param name="left">The left coordinate (0-based)</param>
    /// <param name="top">The top coordinate (0-based)</param>
    /// <param name="length">The length of the line</param>
    /// <param name="lineWidth">The width of the line</param>
    /// <param name="foregroundColor">The foreground color of the character</param>
    /// <param name="backgroundColor">The background color of the character</param>
    public static void DrawVerticalLine(int left, int top, int length, LineWidth lineWidth = LineWidth.Single,
        ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black)
    {
        var chars = GetLineChars(lineWidth);
        System.FastConsole.DrawVLine(left, top, chars[5], length, foregroundColor, backgroundColor);
    }

    /// <summary>
    ///     Draws a horizontal line in the console buffer.
    /// </summary>
    /// <param name="left">The left coordinate (0-based)</param>
    /// <param name="top">The top coordinate (0-based)</param>
    /// <param name="length">The length of the line</param>
    /// <param name="lineWidth">The width of the line</param>
    /// <param name="foregroundColor">The foreground color of the character</param>
    /// <param name="backgroundColor">The background color of the character</param>
    public static void DrawHorizontalLine(int left, int top, int length, LineWidth lineWidth = LineWidth.Single,
        ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black)
    {
        var chars = GetLineChars(lineWidth);
        System.FastConsole.DrawHLine(left, top, chars[4], length, foregroundColor, backgroundColor);
    }
    
    /// <summary>
    ///     Draws a rectangle in the console buffer.
    /// </summary>
    /// <param name="left">The left coordinate (0-based)</param>
    /// <param name="top">The top coordinate (0-based)</param>
    /// <param name="width">The width of the rectangle</param>
    /// <param name="height">The height of the rectangle</param>
    /// <param name="filled">Indicates whether to fill the rectangle with spaces</param>
    /// <param name="lineWidth">The width of the rectangle's border</param>
    /// <param name="foregroundColor">The foreground color of the character</param>
    /// <param name="backgroundColor">The background color of the character</param>
    public static void DrawRectangle(int left, int top, int width, int height, bool filled = true,
        LineWidth lineWidth = LineWidth.Single,
        ConsoleColor foregroundColor = ConsoleColor.White,
        ConsoleColor backgroundColor = ConsoleColor.Black)
    {
        var chars = GetLineChars(lineWidth);

        System.FastConsole.Write(left, top, chars[0], foregroundColor, backgroundColor);
        System.FastConsole.Write(left + width - 1, top, chars[1], foregroundColor, backgroundColor);
        System.FastConsole.Write(left, top + height - 1, chars[2], foregroundColor, backgroundColor);
        System.FastConsole.Write(left + width - 1, top + height - 1, chars[3], foregroundColor, backgroundColor);

        System.FastConsole.DrawHLine(left + 1, top, chars[4], width - 2, foregroundColor, backgroundColor);
        System.FastConsole.DrawVLine(left, top + 1, chars[5], height - 2, foregroundColor, backgroundColor);
        System.FastConsole.DrawVLine(left + width - 1, top + 1, chars[5], height - 2, foregroundColor, backgroundColor);
        System.FastConsole.DrawHLine(left + 1, top + height - 1, chars[4], width - 2, foregroundColor, backgroundColor);

        if (filled)
        {
            System.FastConsole.FillRect(left + 1, top + 1, width - 2, height - 2, ' ', foregroundColor, backgroundColor);
        }
    }
}