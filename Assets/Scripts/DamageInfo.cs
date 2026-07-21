using System;
using UnityEngine;

/// <summary>Contains information about a damage event.</summary>
[Serializable]
public struct DamageInfo
{
    /// <summary>Amount of damage dealt.</summary>
    public float amount;

    /// <summary>World position where damage was applied.</summary>
    public Vector3 position;

    /// <summary>Direction the damage came from (normalized).</summary>
    public Vector3 direction;

    /// <summary>GameObject that caused the damage (optional).</summary>
    public GameObject source;

    /// <summary>Type of damage for gameplay reactions (optional).</summary>
    public string damageType;

    public DamageInfo(float amount, Vector3 position, Vector3 direction = default, GameObject source = null, string damageType = "Generic")
    {
        this.amount = amount;
        this.position = position;
        this.direction = direction != default ? direction.normalized : Vector3.zero;
        this.source = source;
        this.damageType = damageType;
    }
}