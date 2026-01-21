using System.Collections.Generic;

namespace OddBit.FastConsole.GameEngine;

/// <summary>
/// Base class for all scenes in the game.
/// Scenes manage a collection of game objects and handle their lifecycle events.
/// </summary>
public abstract class Scene
{
    private readonly List<GameObject> _gameObjects = new();

    /// <summary>
    /// Adds a game object to this scene.
    /// </summary>
    /// <param name="gameObject">The game object to add.</param>
    protected void AddGameObject(GameObject gameObject)
    {
        _gameObjects.Add(gameObject);
    }

    /// <summary>
    /// Called when the scene starts. Initializes all game objects in the scene.
    /// </summary>
    public virtual void Start()
    {
        foreach (var gameObject in _gameObjects)
        {
            gameObject.Start();
        }
    }
    
    /// <summary>
    /// Called when the scene stops. Cleans up all game objects in the scene.
    /// </summary>
    public virtual void Stop()
    {
        foreach (var gameObject in _gameObjects)
        {
            gameObject.Stop();
        }
    }

    /// <summary>
    /// Called every frame. Updates all game objects in the scene.
    /// </summary>
    public virtual void Update()
    {
        foreach (var gameObject in _gameObjects)
        {
            gameObject.Update();
        }
    }

    /// <summary>
    /// Called at fixed intervals. Updates all game objects in the scene with fixed timing.
    /// </summary>
    public virtual void FixedUpdate()
    {
        foreach (var gameObject in _gameObjects)
        {
            gameObject.FixedUpdate();
        }
    }
}