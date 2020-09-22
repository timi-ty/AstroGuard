using UnityEngine;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

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
#endif

    }

    public static void Notify(string notificationText)
    {
#if UNITY_ANDROID
        var notification = new AndroidNotification();
        notification.Title = "Astro Guardian!";
        notification.Text = notificationText;
        notification.LargeIcon = "icon_0";
        notification.FireTime = System.DateTime.Now.AddMinutes(1);

        AndroidNotificationCenter.SendNotification(notification, "astroguard_notifications");
#endif
    }
}
