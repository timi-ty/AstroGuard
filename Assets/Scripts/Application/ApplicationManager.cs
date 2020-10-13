using UnityEngine;
#if UNITY_IOS
using UnityEngine.SignInWithApple;
#endif


public class ApplicationManager : MonoBehaviour
{
#region Singleton
    public static ApplicationManager instance { get; private set; }
#endregion

#region Constants
    public const string DB_APP_PATH = "app/";
    public const string DB_DLC_PATH = "app/DLC/";
#endregion

#region Properties
    public static string DbUserPath => "users/" + FirebaseUtility.CurrentUser?.UserId + "/";
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
        

        Debug.Log("Loaded Data: " + GameDataManager.GameData?.ToString());
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
            Debug.Log("Saved Data: " + GameDataManager.GameData?.ToString());
        }
    }

    private void OnApplicationQuit()
    {
        FirebaseUtility.Cleanup();

        OnApplicationQuitEvent?.Invoke();
    }

    public void AttemptLoginWithFacebook()
    {
        FacebookUtility.PromptLogIn();
    }

    public void AttemptLoginWithApple()
    {
        #if UNITY_IOS
        var siwa = GetComponentInChildren<SignInWithApple>();
        siwa.Login(OnLoginWithApple);
        #endif
    }

    #if UNITY_IOS
    private void OnLoginWithApple(SignInWithApple.CallbackArgs args)
    {
        if (args.error != null)
        {
            Debug.Log("Errors occurred: " + args.error);
            return;
        }

        UserInfo userInfo = args.userInfo;

        FirebaseUtility.AuthneticateWithApple(userInfo.idToken);

        Debug.Log(
            string.Format("Display Name: {0}\nEmail: {1}\nUser ID: {2}\nID Token: {3}", userInfo.displayName ?? "",
                userInfo.email ?? "", userInfo.userId ?? "", userInfo.idToken ?? ""));
    }
    #endif

    public static void SignOut()
    {
        FirebaseUtility.SignOut();

        FacebookUtility.SignOut();

        Analytics.LogSignOut();
    }
}
