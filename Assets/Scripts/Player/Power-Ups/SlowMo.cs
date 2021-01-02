//In Progress
using UnityEngine;
using System.Collections;
using static GameManager;

public class SlowMo : PowerUpBase
{
    #region Components
    public SlowMoRings slowMoRings;
    #endregion

    #region Inspector Parameters
    public float timeControlSpeed = 3;
    public float slowTimeScale = 0.25f;
    #endregion

    #region Worker Parameters
    private Coroutine controlSlowMoRoutine;
    private Coroutine GoSlowCoroutine;
    private Coroutine GoNormalCoroutine;
    private bool _doneSlowing;
    #endregion


    public override void Activate()
    {
        base.Activate();

        if (controlSlowMoRoutine != null) StopCoroutine(controlSlowMoRoutine);

        float baseDuration = 0.25f;
        float durationUpgrade = 0.25f * (PlayerStats.Instance.Upgradables[PowerType.SlowMo].upgradeProgress / (float) Upgradable.FULL_UPGRADE);

        controlSlowMoRoutine = StartCoroutine(ControlSlowMo(duration: baseDuration + durationUpgrade));
    }

    public override void Deactivate()
    {
        base.Deactivate();

        slowMoRings.StopEmitting();

        Time.timeScale = defaultTimeScale;

        _doneSlowing = false;
    }

    private IEnumerator ControlSlowMo(float duration)
    {
        if (GoNormalCoroutine != null) StopCoroutine(GoNormalCoroutine);

        GoSlowCoroutine = StartCoroutine(GoSlow());

        yield return new WaitUntil(() => _doneSlowing);

        yield return new WaitForSeconds(duration);

        if (GoSlowCoroutine != null) StopCoroutine(GoSlowCoroutine);

        GoNormalCoroutine = StartCoroutine(GoNormal());
    }

    private IEnumerator GoSlow()
    {
        _doneSlowing = false;

        slowMoRings.StartEmitting();

        while(Time.timeScale > slowTimeScale)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, slowTimeScale - slowTimeScale / 10, timeControlSpeed * Time.unscaledDeltaTime);

            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            yield return null;
        }

        _doneSlowing = true;
    }

    private IEnumerator GoNormal()
    {
        slowMoRings.StopEmitting();

        while (Time.timeScale < defaultTimeScale)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, defaultTimeScale + defaultTimeScale / 10, timeControlSpeed * Time.unscaledDeltaTime);

            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            yield return null;
        }

        _doneSlowing = false;

        Deactivate();
    }
}
