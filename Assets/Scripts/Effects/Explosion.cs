//In Progress
using System.Collections;
using UnityEngine;

public class Explosion : EffectBase
{
    #region Inspector Parameters
    [Header("Explosion Settings")]
    public float impactRadiusAcceleration;
    #endregion

    #region Properties
    private float finalImpactRadius { get; set; }
    private float currentImpactRadius { get; set; }
    #endregion

    #region Components
    [Header("Components")]
    public Transform impactRange;
    #endregion


    private void FixedUpdate()
    {
        ApplyExplosionImpact();
    }

    public void Release(float finalImpactRadius)
    {
        this.finalImpactRadius = finalImpactRadius;

        StartCoroutine(SpreadImpact());
    }

    private IEnumerator SpreadImpact()
    {
        yield return null; //Wait for next frame so that Start() can be called first.

        float explosionLife = 0;
        currentImpactRadius = 0;

        while(currentImpactRadius < finalImpactRadius)
        {
            currentImpactRadius = (impactRadiusAcceleration * Mathf.Pow(explosionLife, 2.0f)) / 2.0f;

            impactRange.transform.localScale = Vector3.one * currentImpactRadius;

            explosionLife += Time.deltaTime;

            yield return null;
        }

        FinishImpactSpread();
    }

    private void ApplyExplosionImpact()
    {
        Collider2D[] collidersInImpactRadius = Physics2D.OverlapCircleAll(transform.position, currentImpactRadius);

        foreach (Collider2D collider in collidersInImpactRadius)
        {
            IExplosionImpactible explodable = collider.GetComponent<IExplosionImpactible>();

            if (explodable != null)
            {
                Debug.Log("Bomb exploding: " + collider.name);

                explodable.OnExplosionImpact(transform);
            }
        }
    }

    private void FinishImpactSpread()
    {
        Destroy(this);
    }
}
