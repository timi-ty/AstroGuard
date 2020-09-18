using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BackgroundSet
{
    public Sprite detailSprite;
    public Sprite dustSprite;

    public Color backdropColor;
    public Color dustColor;
}

public class Background : MonoBehaviour
{
    public List<BackgroundSet> backgroundSets;

    #region Background Components
    [Header("Background Components")]
    public SpriteRenderer detail;
    public SpriteRenderer dust;
    public SpriteRenderer backdrop;
    #endregion


    public void OnPlay(int backgroundIndex)
    {
        SetBackground(backgroundIndex);
    }

    #region Utitlity
    public static int GetRandomBackgroundIndex()
    {
        return Random.Range(0, 6);
    }
    public void SetBackground(int backgroundIndex)
    {
        backgroundIndex = Mathf.Clamp(backgroundIndex, 0, backgroundSets.Count);

        detail.sprite = backgroundSets[backgroundIndex].detailSprite;
        dust.sprite = backgroundSets[backgroundIndex].dustSprite;
        backdrop.color = backgroundSets[backgroundIndex].backdropColor;
    }
    #endregion
}
