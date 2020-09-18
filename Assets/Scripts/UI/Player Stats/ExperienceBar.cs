using UnityEngine;
using System.Collections;
using TMPro;

public class ExperienceBar : ProgressBar
{
    #region Components
    public TextMeshProUGUI progressText;
    #endregion

    public void Refresh()
    {
        int lastLevel = PlayerStats.Instance.ExperienceLevel - 1;
        if (lastLevel < 0) lastLevel = 0;
        int currentLevel = PlayerStats.Instance.ExperienceLevel;

        float xpFloor =  Mathf.Pow(lastLevel, 2) + (5 * lastLevel);
        float xpCeil = Mathf.Pow(currentLevel, 2) + (5 * currentLevel);

        float progress = (PlayerStats.Instance.ExperiencePoints - xpFloor) / (xpCeil - xpFloor);

        string currentLevelText = currentLevel.ToString("D2");

        base.SetProgress(progress, "LEVEL " + currentLevelText);

        progressText.text = PlayerStats.Instance.ExperiencePoints.ToString() + "/" + xpCeil;
    }

    protected override IEnumerator AnimateProgress(float progress)
    {
        while (Progress > progress)
        {
            Progress += Time.deltaTime * 0.25f;
            if (Progress >= 1) Progress = 0;
            yield return null;
        }

        while (Progress < progress)
        {
            Progress += Time.deltaTime * 0.25f;
            yield return null;
        }

        Progress = progress;
    }
}
