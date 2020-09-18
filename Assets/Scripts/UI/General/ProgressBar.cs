using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressBar : MonoBehaviour
{
    #region Components
    public Image foregroundFill;
    public TextMeshProUGUI labelText;
    #endregion

    #region Operational Parameters
    private float _progress;
    protected float Progress
    {
        get => _progress;
        set
        {
            _progress = value;
            foregroundFill.fillAmount = value;
        }
    }

    private int lastAnimationStartframe;
    #endregion

    public virtual void SetProgress(float progress, string label)
    {
        //play pop animation here
        if(labelText != null) labelText.text = label;

        StartCoroutine(AnimateProgress(progress));
    }

    public virtual void SetProgressImmediate(float progress, string label)
    {
        //play pop animation here
        if (labelText != null) labelText.text = label;

        Progress = progress;
    }

    protected virtual IEnumerator AnimateProgress(float progress)
    {
        lastAnimationStartframe = Time.frameCount;
        int myStartFrame = lastAnimationStartframe;

        while (Progress != progress)
        {
            int direction = progress > Progress ? 1 : -1;

            Progress += Time.unscaledDeltaTime * 0.25f * direction;

            float min = direction == -1 ? progress : 0;
            float max = direction == 1 ? progress : 1;

            Progress = Mathf.Clamp(Progress, min, max);

            if (lastAnimationStartframe != myStartFrame) yield break;

            yield return null;
        }

        Progress = progress;
    }
}
