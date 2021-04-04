using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Simulation class implements the discrete event simulator pattern.
/// Events are pooled, with a default capacity of 4 instances.
/// </summary>
public static partial class Simulation
{
    /// <typeparam name="T"></typeparam>
    static public T GetModel<T>() where T : class, new()
    {
        return InstanceRegister<T>.instance;
    }
}