using UnityEngine;

/// <summary>
/// Interface to be implemented by all objects that can be affected by explosions.
/// </summary>
public interface IExplosionImpactible
{
    void OnExplosionImpact(Transform explosion);
}
