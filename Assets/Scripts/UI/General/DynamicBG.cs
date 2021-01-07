using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[ExecuteAlways]
public class DynamicBG : MonoBehaviour
{
    public RectTransform content;
    public float horizontalPadding;
    public float verticalPadding;
    private Image bgImage;
    public bool ignoreWidth;
    public bool ignoreHeight;


    void Start()
    {
        bgImage = GetComponent<Image>();
    }

    void Update()
    {
        if (!content) return;

       if(!ignoreWidth) bgImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, content.rect.width + horizontalPadding * 2);
       if(!ignoreHeight) bgImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, content.rect.height + verticalPadding * 2);
    }
}
