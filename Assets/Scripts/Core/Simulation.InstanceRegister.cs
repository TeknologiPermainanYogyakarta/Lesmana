using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Simulation
{
    /// <typeparam name="T"></typeparam>
    private static class InstanceRegister<T> where T : class, new()
    {
        public static T instance = new T();
    }
}