//In Progress
using UnityEngine;

public class PlayerBehaviour : PlayerBase
{
    #region Extensions
    [Header("Extensions")]
    public Sword sword;
    public PowerUps powerUps;
    public SpriteRenderer forceField;
    #endregion

    #region Components
    [Header("Components")]
    public SpriteRenderer spriteRenderer;
    #endregion

    #region Effects
    public GameObject deathFX;
    public GameObject respawnFX;
    #endregion

    #region Properties
    private Color originalForcefieldColor { get; set; }
    #endregion


    #region Unity Runtime
    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Rotate();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        PowerUpOrb powerUpOrb = collision.collider.GetComponent<PowerUpOrb>();

        if (powerUpOrb)
        {
            powerUps.ActivatePowerUp(powerUpOrb);

            powerUpOrb.OnCollected();

            return;
        }

        GoldCoin coin = collision.collider.GetComponent<GoldCoin>();

        if (coin)
        {
            PlayerStats.PocketAstroGold(1);

            coin.OnCollected();

            return;
        }
    }
    #endregion

    #region Overriden Methods
    public override void OnInitialize()
    {
        base.OnInitialize();

        GameManager.ActivateContent(PlayerStats.Instance.activeCoreIndex);
        GameManager.ActivateContent(PlayerStats.Instance.activeBladeIndex);

        InvokeRepeating("UpdateColor", 0f, 2.0f);
    }

    protected override void OnScreenPressed()
    {
        sword.StartAccelerating();
    }

    protected override void OnScreenReleased()
    {
        sword.StopAccelerating();
    }

    public override void OnContinue()
    {
        base.OnContinue();

        powerUps.ActivatePowerUp(PowerType.Shield);
    }

    protected override void Respawn()
    {
        Instantiate(respawnFX, transform.position, Quaternion.identity);
        gameObject.SetActive(true);
    }

    public override void Die()
    {
        powerUps.DeactivateAll();
        Instantiate(deathFX, transform.position, Quaternion.identity);
        gameObject.SetActive(false);

        Metrics.LogPlayerDeath();
    }
    #endregion

    private void Rotate()
    {
        Vector3 eulerAngles = transform.eulerAngles;
        eulerAngles.z -= (sword.speed * 0.35f * 360) * Time.fixedUnscaledDeltaTime;
        transform.eulerAngles = eulerAngles;

        forceField.transform.eulerAngles = sword.transform.eulerAngles;
    }

    private void UpdateColor()
    {
        float anger = Session.SessionStrength / (float) Session.MAX_SESSION_STRENGTH;

        Color newColor = ((1 - anger) * originalForcefieldColor) + (anger * Color.red);

        forceField.color = newColor;

        sword.UpdateColor(anger);
    }

    public void ChangeCore(Sprite coreSprite, Color forceFieldColor)
    {
        spriteRenderer.sprite = coreSprite;
        originalForcefieldColor = forceFieldColor;
    }

    public void ChangeBlade(Sprite bladeSprite, Color bladeColor)
    {
        sword.ChangeAppearance(bladeSprite, bladeColor);
    }
}