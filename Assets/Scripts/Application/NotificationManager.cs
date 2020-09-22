using UnityEngine;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif
using System.Collections.Generic;
using System.Collections;
using System;

public class NotificationManager : MonoBehaviour
{

    void Start()
    {
#if UNITY_ANDROID
        var channel = new AndroidNotificationChannel()
        {
            Id = "astroguard_notifications",
            Name = "Main Channel",
            Importance = Importance.Default,
            Description = "Game Notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

#elif UNITY_IOS
        RequestIOSPermissions();
#endif
    }

    public void RequestIOSPermissions()
    {
        StartCoroutine(RequestAuthorization());
    }

    public static void Notify(string notificationTitle, string notificationText)
    {
#if UNITY_ANDROID
        var androidNotification = new AndroidNotification();
        androidNotification.Title = notificationTitle;
        androidNotification.Text = notificationText;
        androidNotification.LargeIcon = "icon_0";
        androidNotification.FireTime = System.DateTime.Now.AddMinutes(1);
        androidNotification.ShouldAutoCancel = true;
        androidNotification.Style = NotificationStyle.BigTextStyle;

        AndroidNotificationCenter.SendNotification(androidNotification, "astroguard_notifications");

#elif UNITY_IOS
        var timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = new TimeSpan(0, 1, 0),
            Repeats = false
        };

        var iosNotification = new iOSNotification()
        {
            Title = notificationTitle,
            Body = notificationText,
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            Trigger = timeTrigger,
        };

        iOSNotificationCenter.ScheduleNotification(iosNotification);
#endif
    }

    public static void NotifyObjective(string notificationTitle, string objectiveIndex)
    {
        FirebaseUtility.ReadFromDatabase(ObjectiveManager.DbActiveObjectivesPath + objectiveIndex,
            value =>
            {
                Dictionary<string, object> keyValuePairs = (Dictionary<string, object>)value;

                string description = (string) keyValuePairs["description"];
                int xpReward = (int) (long) keyValuePairs["xpReward"];
                int goldReward = (int)(long)keyValuePairs["goldReward"];

#if UNITY_ANDROID
                var notification = new AndroidNotification();
                notification.Title = notificationTitle;
                notification.Text = description + string.Format("to get {0} Astro Gold Coins and {1} Experience Points!", goldReward, xpReward);
                notification.LargeIcon = "icon_0";
                notification.FireTime = System.DateTime.Now.AddMinutes(1);
                notification.ShouldAutoCancel = true;
                notification.Style = NotificationStyle.BigTextStyle;

                AndroidNotificationCenter.SendNotification(notification, "astroguard_notifications");

#elif UNITY_IOS
                var timeTrigger = new iOSNotificationTimeIntervalTrigger()
                {
                    TimeInterval = new TimeSpan(0, 1, 0),
                    Repeats = false
                };

                var iosNotification = new iOSNotification()
                {
                    Title = notificationTitle,
                    Body = description + string.Format("to get {0} Astro Gold Coins and {1} Experience Points!", goldReward, xpReward),
                    ShowInForeground = true,
                    ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
                    Trigger = timeTrigger,
                };

                iOSNotificationCenter.ScheduleNotification(iosNotification);
#endif
            });
    }

    IEnumerator RequestAuthorization()
    {
#if UNITY_ANDROID
        yield break;
#elif UNITY_IOS
        var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge;
        using (var req = new AuthorizationRequest(authorizationOption, true))
        {
            while (!req.IsFinished)
            {
                yield return null;
            };

            string res = "\n RequestAuthorization:";
            res += "\n finished: " + req.IsFinished;
            res += "\n granted :  " + req.Granted;
            res += "\n error:  " + req.Error;
            res += "\n deviceToken:  " + req.DeviceToken;
            Debug.Log(res);
        }
=======
        var notification = new AndroidNotification();
        notification.Title = "Astro Guardian!";
        notification.Text = notificationText;
        notification.LargeIcon = "icon_0";
        notification.FireTime = System.DateTime.Now.AddMinutes(1);

        AndroidNotificationCenter.SendNotification(notification, "astroguard_notifications");
>>>>>>> 37899f878c50e477006ccc20cc90b0b1d95f18fd
#endif
    }
}
