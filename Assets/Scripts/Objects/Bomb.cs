using UnityEngine;
using System;


public struct BombExplosionInfo
{
    public struct ExplosionConditions
    {
        public bool duringAttraction;
        public bool duringSlowMotion;

        public ExplosionConditions(bool duringAttraction, bool duringSlowMotion)
        {
            this.duringAttraction = duringAttraction;
            this.duringSlowMotion = duringSlowMotion;
        }
    }

    public enum Trigger { Space, Player, Sword, Projectile, Explosion, Ship, Shield }
    public Trigger trigger;
    public DateTime timeOfDeath;
    public ExplosionConditions explosionConditions;

    public BombExplosionInfo(Trigger trigger, DateTime timeOfDeath, ExplosionConditions explosionConditions)
    {
        this.trigger = trigger;
        this.timeOfDeath = timeOfDeath;
        this.explosionConditions = explosionConditions;
    }
}

public class Bomb : OutsideSpawnableBase, IExplosionImpactible
{
    #region Properties
    private float size { get { return transform.localScale.magnitude; } set { transform.localScale = Vector3.one * value; } }
    private bool isExploded { get; set; }
    #endregion

    #region Inspector Parameters
    [Header("Effects")]
    public Explosion explosionFX;
    #endregion

    #region Unity Runtime
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsTriggeredBy(collision.collider.transform, out BombExplosionInfo.Trigger trigger) && !isExploded)
        {
            Explode(trigger);
        }
    }
    #endregion

    #region Overriden Methods
    protected override void ApplyImpulse()
    {
        //modify to remove randomness
        Vector2 destination = ScreenBounds.topEdge.middle;
        Vector2 direction = (destination - mRigidBody.position).normalized;

        float impulseMagnitude = UnityEngine.Random.Range(minImpulseMagnitude, maxImpulseMagnitude);

        mRigidBody.AddForce(direction * impulseMagnitude, ForceMode2D.Impulse);
    }

    protected override void DestroyIfLostInSpace()
    {
        base.DestroyIfLostInSpace();

        Metrics.LogBombDisposal();
    }
    #endregion

    #region Sealed Methods
    private void Explode(BombExplosionInfo.Trigger trigger)
    {
        isExploded = true;

        Explosion explosion = Instantiate(explosionFX, transform.position, Quaternion.identity);
        explosion.Release(size * 4);
        Destroy(gameObject);

        BombExplosionInfo bombExplosionInfo = GetExplosionInfo(trigger);

        Metrics.LogBombExplosion(bombExplosionInfo);

        ObjectiveManager.Refresh();
    }

    private bool IsTriggeredBy(Transform transform, out BombExplosionInfo.Trigger trigger)
    {
        if (transform.CompareTag("Player"))
        {
            trigger = BombExplosionInfo.Trigger.Space;
        }
        else if (transform.CompareTag("Weapon"))
        {
            trigger = BombExplosionInfo.Trigger.Space;
        }
        else if (transform.CompareTag("Ship"))
        {
            trigger = BombExplosionInfo.Trigger.Space;
        }
        else if (transform.CompareTag("Explosion"))
        {
            trigger = BombExplosionInfo.Trigger.Space;
        }
        else if (transform.CompareTag("Projectile"))
        {
            trigger = BombExplosionInfo.Trigger.Space;
        }
        else
        {
            trigger = BombExplosionInfo.Trigger.Space;
            return false;
        }

        return true;
    }

    public void OnExplosionImpact(Transform explosion)
    {
        if (IsTriggeredBy(explosion, out BombExplosionInfo.Trigger trigger))
        {
            Explode(trigger);
        }
    }

    public void SetSize(float size)
    {
        this.size = size;
    }

    private BombExplosionInfo GetExplosionInfo(BombExplosionInfo.Trigger trigger)
    {
        PowerUps powerUps = GameManager.instance.player.powerUps;

        BombExplosionInfo.ExplosionConditions explosionConditions = new BombExplosionInfo.ExplosionConditions(duringAttraction: powerUps.isAttractorActive,
                                                                                                              duringSlowMotion: powerUps.isSlowMoActive);

        return new BombExplosionInfo(trigger, DateTime.Now, explosionConditions);
    }
    #endregion
}
