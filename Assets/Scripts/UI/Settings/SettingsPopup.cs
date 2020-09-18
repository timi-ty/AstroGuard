using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : MonoBehaviour
{
    #region Components
    private Animator mAnimator { get; set; }
    public RectTransform popupBox;
    public Toggle sfxToggle;
    public Toggle vibrationToggle;
    public Toggle bgMusicToggle;
    public Button syncProgressButton;
    public Button logOutButton;
    #endregion

    #region Animator Parameters
    private int SHOW_SETTINGS_ANIM_ID = Animator.StringToHash("showSettings");
    private int HIDE_SETTINGS_ANIM_ID = Animator.StringToHash("hideSettings");
    #endregion

    private void Start()
    {
        SizePopupBox();

        mAnimator = GetComponent<Animator>();

        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);

        mAnimator.SetTrigger(SHOW_SETTINGS_ANIM_ID);

        sfxToggle.isOn = Settings.isSfxEnabled;
        vibrationToggle.isOn = Settings.isVibrationEnabled;
        bgMusicToggle.isOn = Settings.isBgMusicEnabled;

        syncProgressButton.interactable = FirebaseUtility.CurrentUser != null;
        logOutButton.interactable = FirebaseUtility.CurrentUser != null;
    }

    public void Hide()
    {
        mAnimator.SetTrigger(HIDE_SETTINGS_ANIM_ID);
    }

    private void Rest()
    {
        gameObject.SetActive(false);
    }

    #region Utility Methods
    private void SizePopupBox()
    {
        (transform as RectTransform).localScale = Vector3.one;

        float width = popupBox.rect.width;

        float height = Mathf.Clamp(width * 1.48f, 870, 1200);

        width = height * 0.6757f;

        popupBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        popupBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }
    #endregion
}
