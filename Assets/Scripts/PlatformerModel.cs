using UnityEngine;

/// <summary>
/// The main model containing needed data to implement a platformer style
/// game. This class should only contain data, and methods that operate
/// on the data. It is initialised with data in the GameController class.
/// </summary>
[System.Serializable]
public class PlatformerModel
{
    public float jumpModifier = 1.5f;

    public float jumpDeceleration = 0.5f;
}