//In Progress
using UnityEngine;
using System.Collections.Generic;

public enum PowerType { MissileLauncher, Attractor, Shield, SlowMo }

public class PowerUps : MonoBehaviour
{
    #region PowerUp Extensions
    private MissileLauncher missileLauncher { get; set; }
    private Attractor attractor { get; set; }
    private Shield shield { get; set; }
    private SlowMo slowMo { get; set; }
    private DarkRush darkRush { get; set; }
    #endregion

    #region PowerUp States
    public bool isMachineGunActive { get { return missileLauncher.isActive; } }
    public bool isAttractorActive { get { return attractor.isActive; } }
    public bool isShieldActive { get { return shield.isActive; } }
    public bool isSlowMoActive { get { return slowMo.isActive; } }
    public bool isDarkRushActive { get { return darkRush.isActive; } }
    #endregion

    #region Properties
    [Header("Sound Effects")]
    public AudioClip powerUpClip;
    #endregion

    private void Start()
    {
        FindPowerUpExtensions();
    }

    public void ActivatePowerUp(PowerUpOrb powerUpOrb)
    {
        if (powerUpOrb.isCollected) return;

        switch (powerUpOrb.powerType)
        {
            case PowerType.Attractor:
                attractor.Activate();
                break;
            case PowerType.MissileLauncher:
                missileLauncher.Activate();
                break;
            case PowerType.SlowMo:
                slowMo.Activate();
                break;
            case PowerType.Shield:
                shield.Activate();
                break;
        }

        Metrics.LogPowerUpCollection(powerUpOrb.powerType);

        ObjectiveManager.Refresh();

        AudioManager.PlayGameClip(powerUpClip);
    }

    public void ActivateShield()
    {
        shield.Activate();
    }

    public void DeactivateAll()
    {
        attractor.Deactivate();
        missileLauncher.Deactivate();
        shield.Deactivate();
        slowMo.Deactivate();
        darkRush.Deactivate();
    }

    private void ActivateDarkRush()
    {
        darkRush.Activate();

        Metrics.LogDarkRush();
    }

    private void FindPowerUpExtensions()
    {
        missileLauncher = GetComponentInChildren<MissileLauncher>();
        attractor = GetComponentInChildren<Attractor>();
        shield = GetComponentInChildren<Shield>();
        slowMo = GetComponentInChildren<SlowMo>();
        darkRush = GetComponentInChildren<DarkRush>();
    }
}
