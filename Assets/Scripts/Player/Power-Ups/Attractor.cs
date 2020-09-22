//In Progress
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Attractor : PowerUpBase
{
    #region Inspector Parameters
    [Header("Field Settings")]
    public float fieldStrength;
    [Tooltip("The rate of change of the speed at which the radius grows.")]
    public float fieldGrowthSpeed;
    public float finalFieldRadius;
    #endregion

    #region Properties
    private float currentFieldRadius { get; set; }
    private Dictionary<int, IAttractible> attractiblesInField { get; set; } = new Dictionary<int, IAttractible>();
    #endregion

    #region Components
    public Transform fieldTransform;
    #endregion

    #region Unity Runtime
    private void Start()
    {
        Deactivate();
    }

    private void FixedUpdate()
    {
        ApplyAttractionForce();
    }
    #endregion

    #region Worker Parameters
    private Coroutine controlAttractionRoutine;
    private Coroutine spreadCoroutine;
    private Coroutine shrinkCoroutine;
    private bool _doneSpreading;
    #endregion

    public override void Activate()
    {
        base.Activate();

        fieldTransform.gameObject.SetActive(true);

        if (controlAttractionRoutine != null) StopCoroutine(controlAttractionRoutine);

        float baseDuration = 3.0f;
        float durationUpgrade = 5.0f * (PlayerStats.Instance.Upgradables[PowerType.Attractor].upgradeProgress / (float)Upgradable.FULL_UPGRADE);

        controlAttractionRoutine = StartCoroutine(ControlAttractionField(duration: baseDuration + durationUpgrade));
    }

    public override void Deactivate()
    {
        base.Deactivate();

        currentFieldRadius = 0;

        fieldTransform.gameObject.SetActive(false);

        fieldTransform.transform.localScale = Vector3.zero;
    }

    #region Coroutines
    private IEnumerator ControlAttractionField(float duration)
    {
        _doneSpreading = false;

        if(shrinkCoroutine != null) StopCoroutine(shrinkCoroutine);

        spreadCoroutine = StartCoroutine(SpreadAttractionField());

        yield return new WaitUntil(() => _doneSpreading);

        yield return new WaitForSeconds(duration);

        if(spreadCoroutine != null) StopCoroutine(spreadCoroutine);

        shrinkCoroutine = StartCoroutine(ShrinkAttractorField());
    }

    private IEnumerator SpreadAttractionField()
    {
        while (currentFieldRadius < finalFieldRadius)
        {
            currentFieldRadius = Mathf.Lerp(currentFieldRadius, finalFieldRadius * 1.2f, fieldGrowthSpeed * Time.deltaTime);

            fieldTransform.transform.localScale = Vector3.one * currentFieldRadius;

            yield return null;
        }

        _doneSpreading = true;
    }

    private IEnumerator ShrinkAttractorField()
    {
        while (currentFieldRadius > 0)
        {
            currentFieldRadius = Mathf.Lerp(currentFieldRadius, 0 - finalFieldRadius * 0.2f, fieldGrowthSpeed * Time.deltaTime);

            fieldTransform.transform.localScale = Vector3.one * currentFieldRadius;

            yield return null;
        }

        _doneSpreading = false;

        Deactivate();
    }
    #endregion

    private void ApplyAttractionForce()
    {
        Collider2D[] collidersInField = Physics2D.OverlapCircleAll(transform.position, currentFieldRadius);

        Dictionary<int, IAttractible> persistInField = new Dictionary<int, IAttractible>();

        foreach (Collider2D collider in collidersInField)
        {
            IAttractible attractible = collider.GetComponent<IAttractible>();

            if (attractible != null)
            {
                persistInField.Add(attractible.GetHashCode(), attractible);

                if (!attractiblesInField.ContainsKey(attractible.GetHashCode()))
                {
                    attractible.OnEnterAttractionField();
                    attractiblesInField.Add(attractible.GetHashCode(), attractible);
                }

                attractible.RecieveAttractionForce(transform.position, fieldStrength);
            }
        }

        foreach (IAttractible attractible in attractiblesInField.Values)
        {
            attractible.OnExitAttractionField();
        }

        foreach (IAttractible attractible in persistInField.Values)
        {
            attractible.OnEnterAttractionField();
        }
    }
}
