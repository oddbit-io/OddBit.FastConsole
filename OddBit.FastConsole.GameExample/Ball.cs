using System;
using System.Linq;
using System.Numerics;
using OddBit.FastConsole.GameEngine;

namespace OddBit.FastConsole.GameExample;

public class Ball : GameObject
{
    private Vector2 _position;
    private Vector2 _velocity;
    private ConsoleColor _color;
    private static readonly char[] Characters;
    private char _ch;

    static Ball()
    {
        // Initialize the array of characters that will be used for the ball display
        Characters = Enumerable.Range(32, 128).Select(i => (char)i).ToArray();
    }

    public override void Start()
    {
        // Initialize the ball's position to a random location within the screen bounds
        _position = new Vector2(Random.Shared.Next(3, Screen.Width - 2), Random.Shared.Next(3, Screen.Height - 2));

        // Initialize the ball's velocity with a random direction and speed
        _velocity = new Vector2(Random.Shared.NextSingle() * 8 - 4, Random.Shared.NextSingle() * 8 - 4);

        // Set a random color for the ball from available console colors
        _color = (ConsoleColor)Random.Shared.Next(1, 16);

        // Select a random character to display for the ball
        _ch = Characters[Random.Shared.Next(Characters.Length)];
    }

    public override void FixedUpdate()
    {
        // Update the ball's position based on its velocity and fixed time step
        _position += _velocity * Time.FixedDeltaTime * 4;

        // Check for collision with top or bottom screen boundaries and reverse vertical velocity
        if (_position.Y >= Screen.Height - 2 || _position.Y <= 2)
        {
            _velocity.Y = -_velocity.Y;
        }

        // Check for collision with left or right screen boundaries and reverse horizontal velocity
        if (_position.X >= Screen.Width - 2 || _position.X <= 2)
        {
            _velocity.X = -_velocity.X;
        }

        // Clamp the position to ensure the ball stays within screen bounds
        _position.X = Math.Clamp(_position.X, 1, Screen.Width - 2);
        _position.Y = Math.Clamp(_position.Y, 1, Screen.Height - 2);
    }

    public override void Update()
    {
        // Render the ball at its current position with the assigned character and color
        System.FastConsole.Write((int)_position.X, (int)_position.Y, _ch, _color, ConsoleColor.DarkBlue);
    }
}