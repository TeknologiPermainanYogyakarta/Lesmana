using System;
using UnityEngine;

/// <summary>
/// Represebts the current vital statistics of some game entity.
/// </summary>
public class Health : MonoBehaviour
{
    /// <summary>
    /// The maximum hit points for the entity.
    /// </summary>
    public int maxHP;

    /// <summary>
    /// Indicates if the entity should be considered 'alive'.
    /// </summary>
    public bool IsAlive => currentHP > 0;

    public int currentHP;

    /// <summary>
    /// Increment the HP of the entity.
    /// </summary>
    public void Update()
    {
    }

    [ContextMenu("decrement")]
    public void lol()
    {
        Decrement(2);
    }

    public void Increment(int healing)
    {
        currentHP = Mathf.Clamp(currentHP + healing, 0, maxHP);
    }

    /// <summary>
    /// Decrement the HP of the entity. Will trigger a HealthIsZero event when
    /// current HP reaches 0.
    /// </summary>
    public void Decrement(int takeDamage)
    {
        currentHP = Mathf.Clamp(currentHP - takeDamage, 0, maxHP);
        if (currentHP == 0)
        {
            //var ev = Schedule<HealthIsZero>();
            //ev.health = this;
        }
    }

    /// <summary>
    /// Decrement the HP of the entitiy until HP reaches 0.
    /// </summary>
    [ContextMenu("Death")]
    public void Die()
    {
        Destroy(gameObject);
    }

    private void Awake()
    {
        currentHP = maxHP;
    }
}