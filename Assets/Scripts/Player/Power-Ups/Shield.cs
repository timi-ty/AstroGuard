//In Progress
using UnityEngine;
using System.Collections.Generic;

public class Shield : PowerUpBase
{
    #region Shieldables
    [Tooltip("Only gameobjects with Monobehaviours that implement IShieldable can know when the shield is active.")]
    public List<GameObject> shieldableObjects = new List<GameObject>();
    #endregion

    #region Inspector Parameters
    [Header("Effects")]
    public ParticleSystem shieldEffect;
    #endregion

    #region Properties
    private bool shieldsActive { get; set; }
    #endregion

    public override void Activate()
    {
        base.Activate();

        Instantiate(shieldEffect, transform.position, Quaternion.identity, transform);

        float baseDuration = 3.0f;
        float durationUpgrade = 5.0f * (PlayerStats.Instance.Upgradables[PowerType.Shield].upgradeProgress / (float)Upgradable.FULL_UPGRADE);

        ControlShields(baseDuration + durationUpgrade);
    }

    public override void Deactivate()
    {
        base.Deactivate();

        DisableShields();
    }

    private void ControlShields(float duration)
    {
        EnableShields();

        CancelInvoke("DisableShields");

        Invoke("DisableShields", duration);
    }

    private void EnableShields()
    {
        if (shieldsActive) return;

        foreach (GameObject shieldableObject in shieldableObjects)
        {
            IShieldable shieldable = shieldableObject.GetComponent<IShieldable>();
            if (shieldable != null)
            {
                shieldable.OnRecievedShield();
            }
        }

        shieldsActive = true;
    }

    private void DisableShields()
    {
        if (!shieldsActive) return;

        foreach (GameObject shieldableObject in shieldableObjects)
        {
            IShieldable shieldable = shieldableObject.GetComponent<IShieldable>();
            if (shieldable != null)
            {
                shieldable.OnLostShield();
            }
        }

        shieldsActive = false;
    }
}
