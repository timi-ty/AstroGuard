using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class LevelButton : Button
{
    private event System.Action<int> clickEvent;

    private int eventArg;


    public void Initialize(int level, float width, float height, bool isAvailable, bool isUnlocked, bool isCompleted)
    {
        (transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        (transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

        TextMeshProUGUI tmp = GetComponentInChildren<TextMeshProUGUI>();
        Image tickMark = transform.GetChild(1).GetComponent<Image>();

        if (LevelManager.IsTutorialLevel(level)) tmp.text = "TUTORIAL";
        else tmp.text = isAvailable ? "LEVEL " + level.ToString("D2") : "COMING SOON     LEVEL " + level.ToString("D2");

        tickMark.enabled = false;

        Color txtColor = tmp.color;

        if (isUnlocked)
        {
            interactable = true;

            txtColor.a = 1.0f;

            if (isCompleted)
            {
                tickMark.enabled = true;
            }
        }
        else
        {
            interactable = false;

            txtColor.a = 0.5f;
        }

        tmp.color = txtColor;
    }

    public void SetOnClickListener(System.Action<int> clickEvent, int eventArg)
    {
        this.clickEvent = clickEvent;

        this.eventArg = eventArg;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (IsInteractable())
        {
            clickEvent?.Invoke(eventArg);
        }

        base.OnPointerClick(eventData);
    }
}
