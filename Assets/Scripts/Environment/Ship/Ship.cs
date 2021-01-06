﻿using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour, IShieldable, IRepeller
{
    #region Constants
    public const int FULL_HEALTH = 3;
    #endregion

    #region Components
    public SpriteRenderer shieldSpriteRenderer;
    public SpriteRenderer repellerSpriteRenderer;
    public DamageLayer damageLayer;
    public Transform particleFloor;
    #endregion

    #region Properties
    private bool isShielded { get; set; }
    private bool isInvincible { get; set; }
    private int health
    {
        get => Session.Instance.ShipHealth; 
        set
        {
            Session.Instance.ShipHealth = value;

            UpdateState(value);
        }
    }
    private bool isDestroyed { get; set; }
    public bool isRepelling { get; set; }
    #endregion

    #region Unity Runtime
    private void Start()
    {
        StartShip();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameManager.currentLevel <= 0) return;

        if (collision.GetContact(0).point.x < ScreenBounds.min.x || collision.GetContact(0).point.x > ScreenBounds.max.x) return;

        OnHit(collision.transform);
    }
    #endregion

    #region Game Events
    public void OnPlay()
    {
        StartShip();
    }

    public void OnContinue()
    {
        Respawn();
    }

    private void ReportLevelFailed()
    {
        GameManager.instance.OnFailed();
    }
    #endregion

    #region Behaviour Methods
    private void OnHit(Transform hitter)
    {
        if (isInvincible) return;

        TakeDamage(hitter);
    }

    private void TakeDamage(Transform hitter)
    {
        Bomb bomb = hitter.GetComponent<Bomb>();

        if (bomb)
        {
            health -= 2;
        }
        else if(hitter.CompareTag("Enemy"))
        {
            health--;
        }

        Metrics.LogShipHit();
    }

    public void ActivateProximityDefense()
    {
        if (!GameManager.IsPlayerAlive()) return;

        GameManager.instance.player.powerUps.ActivatePowerUp(PowerType.SlowMo);
    }

    private void Destroy()
    {
        damageLayer.ShowDestruction();
        ReportLevelFailed();

        isDestroyed = true;
    }

    private void Respawn()
    {
        damageLayer.RemoveAllDamage();
        StartShip();
    }
    #endregion

    #region Utility Methods
    private void UpdateState(int health)
    {
        if(health <= 0)
        {
            health = 0;
            
            if(!isDestroyed) Destroy();

            isDestroyed = true;
        }
        else
        {
            isDestroyed = false;
        }

        damageLayer.ShowDamage(health);
    }

    private void StartShip()
    {
        if(health <= 0)
        {
            health = FULL_HEALTH;
        }

        UpdateState(health);
    }
    #endregion

    #region Shieldable Interface Methods
     
    public void OnLostShield()
    {
        isShielded = false;

        StartCoroutine(
            HideShield(
            onShieldHidden: () => 
            { 
                isInvincible = isShielded || isRepelling; 
            }
            ));
    }

    public void OnRecievedShield()
    {
        isShielded = true;
        isInvincible = true;

        StartCoroutine(ShowShield());
    }

    private IEnumerator ShowShield()
    {
        Color color = shieldSpriteRenderer.color;

        while(color.a < 1 && isShielded)
        {
            color.a = Mathf.Lerp(color.a, 1.2f, Time.deltaTime);

            shieldSpriteRenderer.color = color;

            yield return null;
        }
    }

    private IEnumerator HideShield(System.Action onShieldHidden)
    {
        Color color = shieldSpriteRenderer.color;

        while (color.a > 0 && !isShielded)
        {
            color.a = Mathf.Lerp(color.a, -0.2f, Time.deltaTime);

            shieldSpriteRenderer.color = color;

            yield return null;
        }

        onShieldHidden?.Invoke();
    }

    public bool IsShielded()
    {
        return isInvincible && !isRepelling;
    }
    #endregion

    #region Repeller Interface Methods
    public void OnStartRepelling()
    {
        isRepelling = true;
        isInvincible = true;

        StartCoroutine(ShowRepeller());
    }

    public void OnStopRepelling()
    {
        isRepelling = false;

        StartCoroutine(
            HideRepeller(
            onRepellerHidden: () =>
            {
                isInvincible = isRepelling || isShielded;
            }
            ));
    }

    private IEnumerator ShowRepeller()
    {
        Color color = repellerSpriteRenderer.color;

        while (color.a < 1 && isRepelling)
        {
            color.a = Mathf.Lerp(color.a, 1.2f, Time.deltaTime);

            repellerSpriteRenderer.color = color;

            yield return null;
        }
    }

    private IEnumerator HideRepeller(System.Action onRepellerHidden)
    {
        Color color = repellerSpriteRenderer.color;

        while (color.a > 0 && !isRepelling)
        {
            color.a = Mathf.Lerp(color.a, -0.2f, Time.deltaTime);

            repellerSpriteRenderer.color = color;

            yield return null;
        }

        onRepellerHidden?.Invoke();
    }
    #endregion
}
