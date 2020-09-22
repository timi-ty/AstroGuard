//In Progress
using UnityEngine;

public abstract class PowerUpBase : MonoBehaviour
{
    public bool isActive { get; set; }

    public virtual void Activate()
    {
        isActive = true;

        enabled = true;
    }

    public virtual void Deactivate()
    {
        isActive = false;

        enabled = false;
    }
}
