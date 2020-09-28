using UnityEngine;

public class Analytics : MonoBehaviour
{

    public static void LogSessionEnded(int score)
    {
        AdjustUtility.TrackSessionEnded();

        FacebookUtility.LogAppEvent("session_ended");

        if (!FirebaseUtility.IsFirebaseSafeToUse) return;

        FirebaseUtility.RecordCustomEvent("session_ended",
            new Firebase.Analytics.Parameter[]
            {
                new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterLevel, GameManager.currentLevel),
                new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterScore, score),
                new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterCharacter, PlayerStats.Instance.ExperienceLevel)
            });
    }

    public static void LogSessionRestored(int score)
    {
        AdjustUtility.TrackSessionRestored();

        FacebookUtility.LogAppEvent("session_restored");

        if (!FirebaseUtility.IsFirebaseSafeToUse) return;

        FirebaseUtility.RecordCustomEvent("session_restored",
            new Firebase.Analytics.Parameter[]
            {
                new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterLevel, GameManager.currentLevel),
                new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterScore, score),
                new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterCharacter, PlayerStats.Instance.ExperienceLevel)
            });
    }

    public static void LogItemPurchased(string itemId, decimal price, string currency)
    {
        AdjustUtility.TrackItemPurchased(itemId, price, currency);

        if (!FirebaseUtility.IsFirebaseSafeToUse) return;

        FirebaseUtility.RecordCustomEvent("item_purchased",
            new Firebase.Analytics.Parameter[]
            {
                new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterItemId, itemId),
                new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterPrice, itemId),
                new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterCharacter, PlayerStats.Instance.ExperienceLevel)
            });
    }

    public static void LogItemSelected(string itemId)
    {
        AdjustUtility.TrackItemSelected();

        FacebookUtility.LogAppEvent("item_selected");

        if (!FirebaseUtility.IsFirebaseSafeToUse) return;

        FirebaseUtility.RecordCustomEvent("item_selected",
            new Firebase.Analytics.Parameter[]
            {
                new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterItemId, itemId),
                new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterCharacter, PlayerStats.Instance.ExperienceLevel)
            });
    }

    public static void LogIntroLevelsCompleted()
    {
        AdjustUtility.TrackIntroLevelsCompleted();

        FacebookUtility.LogAppEvent("intro_completed");

        if (!FirebaseUtility.IsFirebaseSafeToUse) return;

        FirebaseUtility.RecordCustomEvent("intro_completed",
            new Firebase.Analytics.Parameter[]
            {
                new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterCharacter, PlayerStats.Instance.ExperienceLevel)
            });
    }

    public static void LogPowerUpCollected(string powerType)
    {
        AdjustUtility.TrackPowerUpCollected();

        FacebookUtility.LogAppEvent("power_up_collection");

        if (!FirebaseUtility.IsFirebaseSafeToUse) return;

        FirebaseUtility.RecordCustomEvent("power_up_collection",
            new Firebase.Analytics.Parameter[]
            {
                new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterLevel, GameManager.currentLevel),
                new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterItemName, powerType)
            });
    }

    public static void LogObjectiveCompleted(string objectiveDescription)
    {
        AdjustUtility.TrackObjectiveCompleted();

        FacebookUtility.LogAppEvent("objective_completed");

        if (!FirebaseUtility.IsFirebaseSafeToUse) return;

        FirebaseUtility.RecordCustomEvent("objective_completed",
                            new Firebase.Analytics.Parameter[]
                            {
                                new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterLevel, GameManager.currentLevel),
                                new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterCharacter, PlayerStats.Instance.ExperienceLevel),
                                new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterItemName, objectiveDescription)
                            });
    }

    public static void LogSignIn()
    {
        AdjustUtility.TrackSignIn();

        FacebookUtility.LogAppEvent("sign_in");
    }

    public static void LogSignUp()
    {
        AdjustUtility.TrackSignUp();

        FacebookUtility.LogAppEvent("sign_up");
    }

    public static void LogSignOut()
    {
        AdjustUtility.TrackSignOut();

        FacebookUtility.LogAppEvent("sign_out");
    }
}
