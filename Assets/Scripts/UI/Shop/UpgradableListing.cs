using UnityEngine;
using UnityEngine.UI;


public class UpgradableListing : MonoBehaviour
{
    public ProgressBar upgradeProgressBar;
    public Image resourceImage;
    public UpgradeButton upgradeButton;

    #region Extras
    public AudioClip upgradeSFX;
    #endregion

    public PowerType upgradeType { get; set; }

    public void Initialize(PowerType upgradeType, Sprite resourceImage, int upgradeProgress, 
        System.Action<UpgradableListing> onUpgradePressed, bool hasAstroGold, float width, float height)
    {
        (transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        (transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

        this.upgradeType = upgradeType;
        this.resourceImage.sprite = resourceImage;

        RefreshUpgradeProgress(upgradeProgress);

        upgradeButton.Initialize(OnDown, onUpgradePressed, OnUp, this, hasAstroGold);
    }

    private void OnDown()
    {
        AudioManager.PlayUILooping(upgradeSFX);
    }

    private void OnUp()
    {
        AudioManager.StopUILooping();
    }


    public void RefreshUpgradeProgress(int upgradeProgress)
    {
        float progress = upgradeProgress / (float)Upgradable.FULL_UPGRADE;
        progress = Mathf.Clamp(progress, 0, 1);
        upgradeProgressBar.SetProgressImmediate(progress, upgradeProgress.ToString() + "/" + Upgradable.FULL_UPGRADE.ToString());
    }
}
