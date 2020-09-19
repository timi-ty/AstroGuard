using UnityEngine;
using Unity.Notifications.Android;

public class NotificationManager : MonoBehaviour
{

    void Start()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "astroguard_notifications",
            Name = "Main Channel",
            Importance = Importance.Default,
            Description = "Game Notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    public static void Notify(string notificationText)
    {
        var notification = new AndroidNotification();
        notification.Title = "Astro Guardian!";
        notification.Text = notificationText;
        notification.LargeIcon = "icon_0";
        notification.FireTime = System.DateTime.Now.AddMinutes(1);

        AndroidNotificationCenter.SendNotification(notification, "astroguard_notifications");
    }
}
