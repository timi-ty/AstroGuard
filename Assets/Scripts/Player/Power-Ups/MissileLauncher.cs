//In Progress
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : PowerUpBase
{
    Dictionary<int, Transform> targets = new Dictionary<int, Transform>();

    #region Inpector Parameters
    [Header("Settings")]
    public float targetSearchSpeed;
    [Header("Prefabs")]
    public Missile missile;
    [Header("Effects")]
    public ParticleSystem launchEffect;
    #endregion

    #region Worker Parameters
    private Vector2 pointA;
    private Vector2 pointB;
    private bool foundTargets;
    private Coroutine launchMissilesCoroutine;
    #endregion

    #region Components
    private SpriteRenderer searchEffect;
    #endregion

    #region Unity Runtime
    private void Start()
    {
        searchEffect = GetComponentInChildren<SpriteRenderer>();

        Deactivate();
    }
    #endregion

    #region Overrides
    public override void Activate()
    {
        base.Activate();

        searchEffect.gameObject.SetActive(true);

        int baseRounds = 1;
        int roundsUpgrade = 5 * (PlayerStats.Instance.Upgradables[PowerType.MissileLauncher].upgradeProgress / Upgradable.FULL_UPGRADE);

        launchMissilesCoroutine = StartCoroutine(LaunchMissiles(baseRounds + roundsUpgrade));
    }

    public override void Deactivate()
    {
        base.Deactivate();

        searchEffect.gameObject.SetActive(false);

        if(launchMissilesCoroutine != null)
        {
            StopCoroutine(launchMissilesCoroutine);
        }
    }
    #endregion

    #region Coroutines
    private IEnumerator LaunchMissiles(int rounds)
    {
        for(int i = 0; i < rounds; i++)
        {
            StartCoroutine(SearchForTargets());

            yield return new WaitUntil(() => foundTargets);

            Instantiate(launchEffect, transform.position, Quaternion.identity, transform);

            foreach (Transform target in targets.Values)
            {
                LaunchMissileAtTarget(target);
            }
        }

        Deactivate();
    }

    private IEnumerator SearchForTargets()
    {
        targets.Clear();

        foundTargets = false;

        Coroutine fadeRoutine = StartCoroutine(FadeInSearchEffect());

        pointA = new Vector2(ScreenBounds.leftEdge.middle.x, transform.position.y);
        pointB = new Vector2(ScreenBounds.rightEdge.middle.x, transform.position.y);

        while(pointB.y < ScreenBounds.max.y + 3)
        {
            pointB.y += targetSearchSpeed * Time.fixedDeltaTime;

            searchEffect.transform.position = new Vector3(ScreenBounds.centre.x, pointB.y);

            FindTargets();

            yield return new WaitForFixedUpdate();
        }

        StopCoroutine(fadeRoutine);

        StartCoroutine(FadeOutSearchEffect());

        foundTargets = true;
    }

    #region Search Effect
    private IEnumerator FadeInSearchEffect()
    {
        float fadeSpeed = 0.18f;

        while(searchEffect.color.a < 0.09)
        {
            Color color = searchEffect.color;
            color.a += fadeSpeed * Time.deltaTime;
            searchEffect.color = color;

            yield return null;
        }
    }

    private IEnumerator FadeOutSearchEffect()
    {
        float fadeSpeed = 0.18f;

        while (searchEffect.color.a > 0)
        {
            Color color = searchEffect.color;
            color.a -= fadeSpeed * Time.deltaTime;
            searchEffect.color = color;

            yield return null;
        }
    }
    #endregion
    #endregion

    #region Action Methods
    private void LaunchMissileAtTarget(Transform target)
    {
        Missile missile = Instantiate(this.missile, transform.position, Quaternion.identity);

        missile.LockTarget(target);
    }

    private void FindTargets()
    {
        Collider2D[] potentialTargets = new Collider2D[8];

        Physics2D.OverlapAreaNonAlloc(pointA, pointB, potentialTargets);

        foreach(Collider2D potentialTarget in potentialTargets)
        {
            if (!potentialTarget) continue;

            if (potentialTarget.CompareTag("Enemy") && !targets.ContainsKey(potentialTarget.GetHashCode()))
            {
                targets.Add(potentialTarget.GetHashCode(), potentialTarget.transform);
            }
        }
    }
    #endregion
}
