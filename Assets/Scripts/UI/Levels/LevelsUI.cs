using UnityEngine;

public class LevelsUI : MonoBehaviour
{
    #region Prefabs
    [Header("Button Prefabs")]
    public LevelButton levelButtonPrefab;
    #endregion

    #region Components
    [Header("Content Holder")]
    public RectTransform contentHolder;
    #endregion

    #region Extras
    [Header("Extras")]
    public AudioClip clickClip;
    #endregion


    #region Unity Runtime
    void Start()
    {
        LevelManager.AddOnLevelsLoadedListener(UpdateLevelSelectionScreen);
    }
    #endregion

    public void Show()
    {
        gameObject.SetActive(true);
        UpdateLevelSelectionScreen();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void UpdateLevelSelectionScreen()
    {
        int buttonCount = LevelManager.LevelCount + 500;

        for (int level = contentHolder.childCount + 1; level <= buttonCount; level++)
        {
            Instantiate(levelButtonPrefab, contentHolder);
        }

        RefreshLevels();
    }

    private void OnLevelButtonClicked(int level)
    {
        GameManager.instance.OnPlay(level);
        AudioManager.PlayUIClip(clickClip);
    }

    #region Utility
    public void RefreshLevels()
    {
        int levelButtonsCount = contentHolder.childCount;

        for (int i = 0; i < levelButtonsCount; i++)
        {
            int level = i + 1;

            bool isAvailable = level <= LevelManager.LevelCount;

            LevelButton button = contentHolder.GetChild(i).GetComponent<LevelButton>();

            float desiredWidth = (contentHolder.parent as RectTransform).rect.width;
            float desiredHeight = desiredWidth * 0.13333f;

            bool isUnlocked = LevelManager.IsLevelUnlocked(level) || GameManager.instance.unlockAllLevels;
            bool isCompleted = LevelManager.IsLevelCompleted(level);

            button.Initialize(level, desiredWidth, desiredHeight, isAvailable, isUnlocked, isCompleted);

            button.SetOnClickListener(OnLevelButtonClicked, level);
        }
    }
    #endregion
}
