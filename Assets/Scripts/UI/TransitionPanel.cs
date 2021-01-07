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
    public List<Image> bgImages;
    public AstroGoldDisplay goldDisplay;
    public GameObject tip;
    public GameObject main;
    public GameObject overlay;
    #endregion

    #region Resources
    [Header("Resources")]
    public List<string> tips = new List<string>();
    #endregion


    public void StartTransition(string title, string subTitle, bool showTip, int astroGold = -1)
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

        goldDisplay.Refresh(astroGold);
        goldDisplay.gameObject.SetActive(astroGold > 0);

        tip.SetActive(!string.IsNullOrEmpty(tipText.text));
        main.SetActive(!string.IsNullOrEmpty(titleText.text) || !string.IsNullOrEmpty(subTitleText.text));

        overlay.SetActive(!main.activeSelf);
    }

    private void EffectTransitionProgress(float progress)
    {
        Color titleColor = titleText.color;
        Color subTitleColor = subTitleText.color;
        Color tipColor = tipText.color;
        List<Color> bgColors = new List<Color>();
        foreach(Image image in bgImages)
        {
            bgColors.Add(image.color);
        }

        titleColor.a = subTitleColor.a = tipColor.a = progress;
        for (int i = 0; i < bgColors.Count; i++)
        {
            Color color = bgColors[i];
            color.a = progress;
        }

        titleText.color = titleColor;
        subTitleText.color = subTitleColor;
        tipText.color = tipColor;
        for (int i = 0; i < bgImages.Count; i++)
        {
            Image image = bgImages[i];
            image.color = bgColors[i];
        }
    }

    public void FinishTransition()
    {
        transitionProgress = 0;

        titleText.text = subTitleText.text = tipText.text = "";

        gameObject.SetActive(false);
    }
}
