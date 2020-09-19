using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    #region Singleton
    public static ApplicationManager instance { get; private set; }
    #endregion

    #region Constants
    public const string DB_APP_PATH = "app/";
    public const string DB_DLC_PATH = "app/DLC/";
    public const string DB_USER_PATH = "users/";
    #endregion

    #region Events
    private event System.Action OnApplicationQuitEvent;
    #endregion

    #region Public Static Global Parameters
    public static string DeviceID => SystemInfo.deviceUniqueIdentifier;
    #endregion

    private void Awake()
    {
        #region Singleton
        DontDestroyOnLoad(gameObject);

        if (!instance)
        {
            instance = this;

            OnApplicationStarted();
        }
        else if (!instance.Equals(this))
        {
            Destroy(gameObject);
        }
        #endregion
    }

    private void OnApplicationStarted()
    {
        Debug.Log("Application Started.");
        GameDataManager.LoadGameData();

        FirebaseUtility.InitializeFirebase(true);
        FacebookUtility.InitializeFacebook();

        Debug.Log("Loaded Data: " + GameDataManager.GameData.ToString());
    }

    public static void AddOnApplicationQuitListener(System.Action onApplicationQuit)
    {
        instance.OnApplicationQuitEvent += onApplicationQuit;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            GameDataManager.SaveGameData();
            Debug.Log("Saved Data: " + GameDataManager.GameData.ToString());
        }
    }

    private void OnApplicationQuit()
    {
        FirebaseUtility.Cleanup();

        OnApplicationQuitEvent?.Invoke();
    }

    public void AttemptLogin()
    {
        FacebookUtility.PromptLogIn();
    }

    public static void SignOut()
    {
        FirebaseUtility.SignOut();

        FacebookUtility.SignOut();

        Analytics.LogSignOut();
    }
}
