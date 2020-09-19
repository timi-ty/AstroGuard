using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Facebook.Unity;

public static class FacebookUtility
{
    public static void InitializeFacebook()
    {
        Debug.Log("Initializing Facebook...");
        if (!FB.IsInitialized)
        {
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            FB.ActivateApp();
        }
    }

    public static void AttemptSilentLogin()
    {
        if (!FB.IsInitialized)
        {
            Debug.Log("Facebook wasn't ready. Sign in will attempt agin when it is.");
            return;
        }

        if (FB.IsLoggedIn)
        {
            HandOverToFirebase(AccessToken.CurrentAccessToken.TokenString);
        }
        else
        {
            var perms = new List<string>() { "public_profile", "email" };
            FB.LogInWithReadPermissions(perms, AuthCallback);
        }
    }

    public static void PromptLogIn()
    {
        if (!FB.IsInitialized)
        {
            Debug.LogWarning("Attempted Facebook login before initialization. Retry?");

            AlertMessageInfo alertMessageInfo = new AlertMessageInfo()
            {
                titleText = "Ooops!",
                messageText = "Sorry, Facebook wasn't ready.",
                positiveActionText = "Retry",
                negativeActionText = "Cancel"
            };

            UIManager.QueueAlertDialog(alertMessageInfo, PromptLogIn, () => { } );

            return;
        }

        var perms = new List<string>() { "public_profile", "email" };

        FB.LogInWithReadPermissions(perms, AuthCallback);
    }

    private static void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(aToken.UserId);
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log(perm);
            }

            HandOverToFirebase(aToken.TokenString);
        }
        else
        {
            AlertMessageInfo alertMessageInfo = new AlertMessageInfo()
            {
                titleText = "LOGIN",
                messageText = "Login cancelled.",
                positiveActionText = "Retry",
                negativeActionText = "Cancel"
            };

            UIManager.QueueAlertDialog(alertMessageInfo, PromptLogIn, () => { });
        }
    }

    private static void HandOverToFirebase(string accessToken)
    {
        FirebaseUtility.AuthneticateWithFacebook(accessToken);
    }

    private static void InitCallback()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private static void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    public static void SignOut()
    {
        FB.LogOut();
    }

    public static void LogAppEvent(string eventName)
    {
        FB.LogAppEvent(eventName);
    }

    public static void LogPurchase(decimal price, string currency)
    {
        FB.LogPurchase(price, currency);
    }
}
