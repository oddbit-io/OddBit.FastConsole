using System.Diagnostics;
using System.Runtime.InteropServices;

// ReSharper disable MemberCanBePrivate.Global

namespace System;

/// <summary>
///     Provides high-performance console output capabilities by directly writing to the Windows console buffer.
/// </summary>
/// <remarks>
///     Note: Although output functions can be used concurrently from multiple threads, 
///     the result will be undefined. If output from multiple threads is required, 
///     implement manual synchronization.
/// </remarks>
public static class FastConsole
{
    /// <summary>
    /// Gets the width of the console buffer in characters.
    /// </summary>
    public static int Width { get; private set; } = Console.WindowWidth;

    /// <summary>
    /// Gets the height of the console buffer in characters.
    /// </summary>
    public static int Height { get; private set; } = Console.WindowHeight;

    /// <summary>
    /// Gets a value indicating whether the current platform is supported (Windows only).
    /// </summary>
    public static bool IsPlatformSupported => Environment.OSVersion.Platform == PlatformID.Win32NT;

    private static CharInfo[]? _buffer;
    private static nint _hConsole;
    private static Rect _writeRegion;
    private static readonly Stopwatch UpdateBufferSizeTimeoutStopwatch = new();

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool WriteConsoleOutput(
        nint hConsoleOutput,
        CharInfo[] lpBuffer,
        Coord dwBufferSize,
        Coord dwBufferCoord,
        ref Rect lpWriteRegion);

    /// <summary>
    ///     Initializes the fast console with specified dimensions and validates platform compatibility.
    ///     Throws exceptions only during initialization; subsequent operations are safe and won't cause
    ///     program termination.
    /// </summary>
    /// <param name="width">The width of the console buffer in characters</param>
    /// <param name="height">The height of the console buffer in characters</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when width or height is negative</exception>
    /// <exception cref="InvalidOperationException">Thrown when already initialized</exception>
    /// <exception cref="PlatformNotSupportedException">Thrown when running on non-Windows platform</exception>
    public static void Initialize(int width = 80, int height = 25)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(width);
        ArgumentOutOfRangeException.ThrowIfNegative(height);

        if (!IsPlatformSupported)
            throw new PlatformNotSupportedException();

        Width = width;
        Height = height;

        _hConsole = GetStdHandle(-11); // STD_OUTPUT_HANDLE

        if (_hConsole == IntPtr.Zero || _hConsole == new IntPtr(-1))
            throw new InvalidOperationException("Failed to get console handle");

        Console.SetWindowSize(Width, Height);
        Console.SetBufferSize(Width, Height);

        _buffer = new CharInfo[Width * Height];
        _writeRegion = new Rect(0, 0, (short)(Width - 1), (short)(Height - 1));
    }

    /// <summary>
    ///     Resets the fast console state, releasing any allocated resources and 
    ///     preparing the system for re-initialization.
    /// </summary>
    public static void Reset()
    {
        _hConsole = IntPtr.Zero;
        _buffer = null;
    }

    private static void ThrowIfNotInitialized()
    {
        if (_buffer == null)
            throw new InvalidOperationException($"{nameof(FastConsole)} must be initialized before use");
    }

    private static ushort GetAttributes(ConsoleColor foregroundColor, ConsoleColor backgroundColor) =>
        (ushort)(((int)backgroundColor << 4) | (int)foregroundColor);

    private static bool IsInBounds(int left, int top) => left >= 0 && top >= 0 && left < Width && top < Height;

    /// <summary>
    ///     Attempts to update the FastConsole buffer size to match the current window dimensions.
    ///     This method is useful when the console window has been resized and the internal 
    ///     buffer needs to be realigned with the new dimensions.
    /// </summary>
    /// <param name="timeout">
    ///     Optional timeout duration for the resize operation. If null, defaults to 1000ms.
    ///     The method will keep attempting to read the console size until it succeeds or 
    ///     the timeout is exceeded.
    /// </param>
    /// <returns>
    ///     True if the buffer size was successfully updated to match the current window size;
    ///     false if no change was needed (i.e., dimensions already matched).
    /// </returns>
    /// <exception cref="TimeoutException">
    ///     Thrown when the console buffer size update operation exceeds the specified timeout.
    /// </exception>
    /// <remarks>
    ///     This method internally calls <see cref="Initialize"/> to reconfigure the buffer
    ///     with the new dimensions. It is designed to be safe for use during runtime and 
    ///     will not cause program termination unless initialization fails.
    /// </remarks>
    public static bool UpdateBufferSize(TimeSpan? timeout = null)
    {
        timeout ??= TimeSpan.FromMilliseconds(1000);
        UpdateBufferSizeTimeoutStopwatch.Restart();
        while (true)
        {
            try
            {
                var width = Console.WindowWidth;
                var height = Console.WindowHeight;
                if (width != Width || height != Height)
                {
                    Initialize(width, height);
                    Width = width;
                    Height = height;
                    return true;
                }

                break;
            }
            catch
            {
                // retry reading console size until it succeeds
            }

            if (UpdateBufferSizeTimeoutStopwatch.Elapsed >= timeout)
            {
                throw new TimeoutException("Console buffer size update timeout.");
            }
        }

        return false;
    }

    /// <summary>
    ///     Clears the entire console buffer by filling it with spaces using specified foreground
    ///     and background colors.
    /// </summary>
    /// <param name="ch">The character to write to the console buffer; default is space.</param>
    /// <param name="foregroundColor">The foreground color of the character</param>
    /// <param name="backgroundColor">The background color of the character</param>
    public static void Clear(char ch = ' ', ConsoleColor backgroundColor = ConsoleColor.Black,
        ConsoleColor foregroundColor = ConsoleColor.Gray)
    {
        ThrowIfNotInitialized();
        Debug.Assert(_buffer != null, nameof(_buffer) + " != null");

        for (var i = 0; i < _buffer.Length; i++)
        {
            _buffer[i].UnicodeChar = ch;
            _buffer[i].Attributes = GetAttributes(foregroundColor, backgroundColor);
        }
    }

    /// <summary>
    ///     Writes a single character to the specified position in the console buffer.
    /// </summary>
    /// <param name="left">The left coordinate (0-based)</param>
    /// <param name="top">The top coordinate (0-based)</param>
    /// <param name="ch">The character to write</param>
    /// <param name="foregroundColor">The foreground color of the character</param>
    /// <param name="backgroundColor">The background color of the character</param>
    /// <remarks>
    ///     If the specified position is outside the console buffer boundaries,
    ///     the operation is silently ignored (character is not written).
    /// </remarks>
    public static void Write(int left, int top, char ch,
        ConsoleColor foregroundColor = ConsoleColor.Gray,
        ConsoleColor backgroundColor = ConsoleColor.Black)
    {
        ThrowIfNotInitialized();
        Debug.Assert(_buffer != null, nameof(_buffer) + " != null");

        if (!IsInBounds(left, top))
            return;

        var i = top * Width + left;
        _buffer[i].UnicodeChar = ch;
        _buffer[i].Attributes = GetAttributes(foregroundColor, backgroundColor);
    }

    /// <summary>
    ///     Draws a horizontal line of characters to the specified position in the console buffer.
    /// </summary>
    /// <param name="left">The left coordinate (0-based)</param>
    /// <param name="top">The top coordinate (0-based)</param>
    /// <param name="ch">The character to write</param>
    /// <param name="count">The number of characters to write</param>
    /// <param name="foregroundColor">The foreground color of the character</param>
    /// <param name="backgroundColor">The background color of the character</param>
    /// <remarks>
    ///     If the specified position is outside the console buffer boundaries,
    ///     the operation is silently ignored (character is not written).
    /// </remarks>
    public static void DrawHLine(int left, int top, char ch, int count,
        ConsoleColor foregroundColor = ConsoleColor.Gray,
        ConsoleColor backgroundColor = ConsoleColor.Black)
    {
        ThrowIfNotInitialized();
        Debug.Assert(_buffer != null, nameof(_buffer) + " != null");

        for (var x = 0; x < count; x++)
        {
            if (!IsInBounds(left, top))
                continue;

            var i = top * Width + left + x;
            _buffer[i].UnicodeChar = ch;
            _buffer[i].Attributes = GetAttributes(foregroundColor, backgroundColor);
        }
    }

    /// <summary>
    ///     Draws a vertical line of characters to the specified position in the console buffer.
    /// </summary>
    /// <param name="left">The left coordinate (0-based)</param>
    /// <param name="top">The top coordinate (0-based)</param>
    /// <param name="ch">The character to write</param>
    /// <param name="count">The number of characters to write</param>
    /// <param name="foregroundColor">The foreground color of the character</param>
    /// <param name="backgroundColor">The background color of the character</param>
    /// <remarks>
    ///     If the specified position is outside the console buffer boundaries,
    ///     the operation is silently ignored (character is not written).
    /// </remarks>
    public static void DrawVLine(int left, int top, char ch, int count,
        ConsoleColor foregroundColor = ConsoleColor.Gray,
        ConsoleColor backgroundColor = ConsoleColor.Black)
    {
        ThrowIfNotInitialized();
        Debug.Assert(_buffer != null, nameof(_buffer) + " != null");

        for (var y = 0; y < count; y++)
        {
            if (!IsInBounds(left, top))
                continue;

            var i = (top + y) * Width + left;
            _buffer[i].UnicodeChar = ch;
            _buffer[i].Attributes = GetAttributes(foregroundColor, backgroundColor);
        }
    }

    /// <summary>
    ///     Writes a rectangle to the specified position in the console buffer.
    /// </summary>
    /// <param name="left">The left coordinate (0-based)</param>
    /// <param name="top">The top coordinate (0-based)</param>
    /// <param name="width">The width of the rectangle</param>
    /// <param name="height">The height of the rectangle</param>
    /// <param name="ch">The character to write</param>
    /// <param name="foregroundColor">The foreground color of the character</param>
    /// <param name="backgroundColor">The background color of the character</param>
    /// <remarks>
    ///     If the specified position is outside the console buffer boundaries,
    ///     the operation is silently ignored (character is not written).
    /// </remarks>
    public static void FillRect(int left, int top, int width, int height, char ch,
        ConsoleColor foregroundColor = ConsoleColor.Gray,
        ConsoleColor backgroundColor = ConsoleColor.Black)
    {
        ThrowIfNotInitialized();
        Debug.Assert(_buffer != null, nameof(_buffer) + " != null");

        if (left >= Width || top >= Height)
            return;

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                if (!IsInBounds(left + x, top + y))
                    continue;

                var i = (top + y) * Width + left + x;
                _buffer[i].UnicodeChar = ch;
                _buffer[i].Attributes = GetAttributes(foregroundColor, backgroundColor);
            }
        }
    }

    /// <summary>
    ///     Writes a span of characters to the specified position in the console buffer.
    /// </summary>
    /// <param name="left">The left coordinate (0-based)</param>
    /// <param name="top">The top coordinate (0-based)</param>
    /// <param name="span">The span of characters to write</param>
    /// <param name="foregroundColor">The foreground color of the characters</param>
    /// <param name="backgroundColor">The background color of the characters</param>
    /// <remarks>
    ///     If the specified position is outside the console buffer boundaries,
    ///     the operation is silently ignored (no characters are written).
    ///     If the span extends beyond the buffer width, writing is truncated.
    /// </remarks>
    public static void Write(int left, int top, ReadOnlySpan<char> span,
        ConsoleColor foregroundColor = ConsoleColor.Gray,
        ConsoleColor backgroundColor = ConsoleColor.Black)
    {
        ThrowIfNotInitialized();
        Debug.Assert(_buffer != null, nameof(_buffer) + " != null");

        if (span.Length == 0)
            return;

        foreach (var ch in span)
        {
            if (!IsInBounds(left, top))
                continue;

            var i = top * Width + left;
            _buffer[i].UnicodeChar = ch;
            _buffer[i].Attributes = GetAttributes(foregroundColor, backgroundColor);
            ++left;
        }
    }

    /// <summary>
    ///     Flushes the internal buffer to the console screen, updating the visible output.
    /// </summary>
    public static void Flush()
    {
        ThrowIfNotInitialized();
        Debug.Assert(_buffer != null, nameof(_buffer) + " != null");

        var success = WriteConsoleOutput(
            _hConsole,
            _buffer,
            new Coord((short)Width, (short)Height),
            new Coord(0, 0),
            ref _writeRegion);

        if (!success)
        {
            var errorCode = Marshal.GetLastWin32Error();
            Debug.WriteLine($"WriteConsoleOutput failed with error code: {errorCode}");
        }
    }

    #region Marshalling structures

    [StructLayout(LayoutKind.Sequential)]
    private struct Coord(short x, short y)
    {
        public short X = x;
        public short Y = y;
    }

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    private struct CharInfo
    {
        [FieldOffset(0)] public char UnicodeChar;
        [FieldOffset(0)] public byte AsciiChar;
        [FieldOffset(2)] public ushort Attributes;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect(short left, short top, short right, short bottom)
    {
        public short Left = left;
        public short Top = top;
        public short Right = right;
        public short Bottom = bottom;
    }

    #endregion Marshalling structures
}