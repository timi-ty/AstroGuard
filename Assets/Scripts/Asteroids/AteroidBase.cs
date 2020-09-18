//In Progress
using UnityEngine;
using System;

[Serializable]
public struct AsteroidDeathInfo
{
    public struct DeathConditions
    {
        public bool duringAttraction;
        public bool duringSlowMotion;
        public bool duringDarkRush;

        public DeathConditions(bool duringAttraction, bool duringSlowMotion, bool duringDarkRush)
        {
            this.duringAttraction = duringAttraction;
            this.duringSlowMotion = duringSlowMotion;
            this.duringDarkRush = duringDarkRush;
        }
    }

    public enum Killer { Space, Sword, Projectile, Explosion, Ship, Shield, PowerUpOrb }
    public Killer killer;
    public DateTime timeOfDeath;
    public DeathConditions deathConditions;

    public AsteroidDeathInfo(Killer killer, DateTime timeOfDeath, DeathConditions deathConditions)
    {
        this.killer = killer;
        this.timeOfDeath = timeOfDeath;
        this.deathConditions = deathConditions;
    }
}

public abstract class AteroidBase : MonoBehaviour, IExplosionImpactible, IAttractible
{
    #region Properties
    protected float speed { get; set; }
    private float originalSize { get; set; }
    private float size 
    {   get => transform.localScale.x;

        set 
        { 
            transform.localScale = Vector3.one * value * originalSize;
        } 
    }
    private bool isDead { get; set; }
    protected bool isInAttractionField { get; set; }

    public enum Type { Zero, One, Two}
    public Type type { get; set; }

    private AsteroidCommander mEnemyCommander { get; set; }
    #endregion

    #region Components
    private Collider2D mCollider;
    private MeshRenderer meshRenderer;
    private TrailRenderer trailRenderer;
    protected Rigidbody2D mRigidBody;
    #endregion

    #region Inspector Parameters
    [Header("Effects")]
    public AsteroidDebris rockDebris;
    #endregion

    #region Unity Runtime
    protected virtual void Start()
    {
        mCollider = GetComponentInChildren<Collider2D>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();
        mRigidBody = GetComponent<Rigidbody2D>();

        SetTrailColor();

        InvokeRepeating("DieIfLostInSpace", 3.0f, 3.0f);

        StartMove();
    }

    protected virtual void FixedUpdate()
    {
        Move();
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        OnHit(collision.collider.transform);
    }
    #endregion

    #region Abstract Methods
    protected abstract void StartMove();
    protected abstract void Move();
    #endregion

    #region Virtual Methods
    protected virtual void OnHit(Transform hitter)
    {
        if(HitByKiller(hitter, out AsteroidDeathInfo.Killer killer))
        {
            Die(killer);
        }
    }
    public virtual void OnExplosionImpact(Transform explosion)
    {
        OnHit(explosion);
    }

    public virtual void OnEnterAttractionField()
    {
        isInAttractionField = true;
    }

    public virtual void RecieveAttractionForce(Vector2 sourcePoint, float forceMagnitude)
    {
        Vector2 forceDirection = (sourcePoint - mRigidBody.position).normalized;

        mRigidBody.AddForce(forceDirection * forceMagnitude, ForceMode2D.Force);
    }

    public virtual void OnExitAttractionField()
    {
        isInAttractionField = false;
    }
    #endregion

    #region Sealed Methods
    public void SetParams(AsteroidCommander enemyCommander, float speed, float size)
    {
        mEnemyCommander = enemyCommander;

        originalSize = size;

        this.speed = speed;
        this.size = size;
    }

    private void Die(AsteroidDeathInfo.Killer killer)
    {
        if (isDead) return;

        CreateDeathEffect();

        AsteroidDeathInfo deathInfo = GetDeathInfo(killer);

        mEnemyCommander.OnAsteroidDeath(deathInfo, type, mRigidBody.position);

        Destroy(gameObject);

        isDead = true;
    }

    private void SetTrailColor()
    {
        if (!trailRenderer) return;

        Color.RGBToHSV(meshRenderer.material.color, out float hue, out float sat, out float val);

        Color startColor = Color.HSVToRGB(hue, sat, val);

        Color endColor = Color.HSVToRGB(hue, sat - 0.3f, val);

        startColor.a = 0.5f;
        endColor.a = 0;

        trailRenderer.startColor = startColor;
        trailRenderer.endColor = endColor;
    }

    private void CreateDeathEffect()
    {
        AsteroidDebris debris = Instantiate(rockDebris, transform.position, Quaternion.identity);

        debris.Initialize(meshRenderer.material.color, GameManager.instance.ship.particleFloor);
    }

    private void DieIfLostInSpace()
    {
        if (ScreenBounds.IsOutOfPlayableArea(mCollider.bounds))
        {
            Die(AsteroidDeathInfo.Killer.Space);
        }
    }

    private bool HitByKiller(Transform hitterTransform, out AsteroidDeathInfo.Killer killer)
    {
        if (IsHitterShielded(hitterTransform))
        {
            killer = AsteroidDeathInfo.Killer.Shield;
        }
        else if (hitterTransform.CompareTag("Weapon"))
        {
            killer = AsteroidDeathInfo.Killer.Sword;
        }
        else if (hitterTransform.CompareTag("Projectile"))
        {
            killer = AsteroidDeathInfo.Killer.Projectile;
        }
        else if (hitterTransform.CompareTag("Explosion"))
        {
            killer = AsteroidDeathInfo.Killer.Explosion;
        }
        else if (hitterTransform.CompareTag("Ship"))
        {
            killer = AsteroidDeathInfo.Killer.Ship;
        }
        else if (hitterTransform.CompareTag("PowerUpOrb"))
        {
            killer = AsteroidDeathInfo.Killer.PowerUpOrb;
        }
        else
        {
            killer = AsteroidDeathInfo.Killer.Space;
            return false;
        }

        return true;
    }

    private AsteroidDeathInfo GetDeathInfo(AsteroidDeathInfo.Killer killer)
    {
        PowerUps powerUps = mEnemyCommander.player.powerUps;

        AsteroidDeathInfo.DeathConditions deathConditions = new AsteroidDeathInfo.DeathConditions(duringAttraction: powerUps.isAttractorActive,
                                                                                            duringSlowMotion: powerUps.isSlowMoActive,
                                                                                            duringDarkRush: powerUps.isDarkRushActive);

        return new AsteroidDeathInfo(killer, DateTime.Now, deathConditions);
    }

    private bool IsHitterShielded(Transform hitter)
    {
        IShieldable shieldable = hitter.GetComponent<IShieldable>();

        if(shieldable != null)
        {
            return shieldable.IsShielded();
        }

        return false;
    }
    #endregion
}
