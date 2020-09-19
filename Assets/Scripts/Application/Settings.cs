//In Progress
using UnityEngine;

public struct SettingsKeys
{
    public const string ENABLE_SFX = "setting.sfxToggle";
    public const string ENABLE_VIBRATION = "setting.vibrationToggle";
    public const string ENABLE_BG_MUSIC = "setting.bgMusicToggle";
}

public class Settings : MonoBehaviour
{
    #region Singleton
    public static Settings instance { get; private set; }
    #endregion

    #region Exposed Settings
    public static bool isSfxEnabled => PlayerPrefs.GetInt(SettingsKeys.ENABLE_SFX, 1) == 1;
    public static bool isVibrationEnabled => PlayerPrefs.GetInt(SettingsKeys.ENABLE_VIBRATION, 1) == 1;
    public static bool isBgMusicEnabled => PlayerPrefs.GetInt(SettingsKeys.ENABLE_BG_MUSIC, 1) == 1;
    #endregion

    #region Unity Runtime
    void Awake()
    {
        #region Singleton
        if (!instance)
        {
            instance = this;
        }
        else if (!instance.Equals(this))
        {
            Destroy(gameObject);
        }
        #endregion
    }
    #endregion

    public void OnSfxToggle(bool value)
    {
        Debug.Log("Sfx toggled to: " + value);
        PlayerPrefs.SetInt(SettingsKeys.ENABLE_SFX, value ? 1 : 0);

        AudioManager.CheckAudioSettings();
    }

    public void OnVibrationToggle(bool value)
    {
        PlayerPrefs.SetInt(SettingsKeys.ENABLE_VIBRATION, value ? 1 : 0);

        AudioManager.CheckAudioSettings();
    }

    public void OnBgMusicToggle(bool value)
    {
        PlayerPrefs.SetInt(SettingsKeys.ENABLE_BG_MUSIC, value ? 1 : 0);

        AudioManager.CheckAudioSettings();
    }

    public void ClearProgress()
    {
        AlertMessageInfo alertMessageInfo = new AlertMessageInfo()
        {
            titleText = "CLEAR PROGRESS?",
            messageText = "Are you sure you want to clear your progress? " +
            "\n\nThis will also erase your online save!",
            positiveActionText = "CLEAR PROGRESS",
            negativeActionText = "CANCEL"
        };

        UIManager.QueueAlertDialog(alertMessageInfo, ClearAndRefresh, () => { });
    }

    private void ClearAndRefresh()
    {
        GameDataManager.ResetProgress();
        UIManager.ShowMainUI();
    }

    public void LogOut()
    {
        AlertMessageInfo alertMessageInfo = new AlertMessageInfo()
        {
            titleText = "LOG OUT?",
            messageText = "You can't earn rewards while logged out." +
            "\n You can't post your highscores while logged out." +
            "\nYou may loose your local save if you log in with another account." +
            "\nYou should sync your Game Data first",
            positiveActionText = "LOG OUT",
            negativeActionText = "CANCEL"
        };

        UIManager.QueueAlertDialog(alertMessageInfo, ApplicationManager.SignOut, () => { });
    }

    public void SyncProgress()
    {
        FirebaseUtility.SyncGameData();
    }
}