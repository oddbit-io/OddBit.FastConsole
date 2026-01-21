namespace OddBit.FastConsole.GameEngine;

/// <summary>
/// Base class for all game objects.
/// </summary>
public abstract class GameObject
{
    /// <summary>
    /// Called when the scene is started.
    /// Override this method to perform initialization logic.
    /// </summary>
    public virtual void Start()
    {
    }

    /// <summary>
    /// Called when the scene is stopped.
    /// Override this method to perform cleanup logic.
    /// </summary>
    public virtual void Stop()
    {
    }

    /// <summary>
    /// Called every frame to update the game object's state.
    /// Override this method to implement update logic.
    /// </summary>
    public virtual void Update()
    {
    }

    /// <summary>
    /// Called at fixed intervals to update the game object's physics and state.
    /// Override this method to implement fixed update logic.
    /// </summary>
    public virtual void FixedUpdate()
    {
    }
}