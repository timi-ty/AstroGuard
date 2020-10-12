using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class TransitionPanel : MonoBehaviour
{
    #region Properties
    private float _transitionProgress;
    public float transitionProgress
    {
        get => _transitionProgress;
        set
        {
            _transitionProgress = value;
            EffectTransitionProgress(_transitionProgress);
        }
    }
    #endregion

    #region Components
    [Header("Components")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subTitleText;
    public TextMeshProUGUI tipText;
    public Image bgImage;
    #endregion

    #region Resources
    [Header("Resources")]
    public List<string> tips = new List<string>();
    #endregion


    public void StartTransition(string title, string subTitle, bool showTip)
    {
        gameObject.SetActive(true);

        transitionProgress = 0;

        titleText.text = title;
        subTitleText.text = subTitle;

        if(GameManager.currentLevel == 1)
        {
            tipText.text = tips[4];
        }
        else
        {
            tipText.text = showTip ? tips[Random.Range(0, tips.Count)] : "";
        }
    }

    private void EffectTransitionProgress(float progress)
    {
        Color titleColor = titleText.color;
        Color subTitleColor = subTitleText.color;
        Color tipColor = tipText.color;
        Color bgColor = bgImage.color;

        titleColor.a = subTitleColor.a = tipColor.a = bgColor.a = progress;

        titleText.color = titleColor;
        subTitleText.color = subTitleColor;
        tipText.color = tipColor;
        bgImage.color = bgColor;
    }

    public void FinishTransition()
    {
        transitionProgress = 0;

        titleText.text = subTitleText.text = tipText.text = "";

        gameObject.SetActive(false);
    }
}
