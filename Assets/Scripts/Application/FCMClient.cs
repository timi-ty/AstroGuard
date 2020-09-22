using UnityEngine;
using Firebase.Messaging;

public static class FCMClient
{

    private const string DYNAMIC_OBJECTIVE = "dynamicObjective";
    private const string TITLE = "title";

    public static void Initialize()
    {
        FirebaseMessaging.SubscribeAsync("Game_Notification");
        
        FirebaseMessaging.TokenReceived += OnTokenReceived;
        FirebaseMessaging.MessageReceived += OnMessageReceived;
    }

    public static void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        Debug.Log("Received Registration Token: " + token.Token);
    }

    public static void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        Debug.Log("Received a new message");

        if (e.Message.From.Length > 0)
            Debug.Log("from: " + e.Message.From);

        
        e.Message.Data.TryGetValue(DYNAMIC_OBJECTIVE, out string objectiveIndexe);
        Debug.Log("Message Data: " + objectiveIndexe);

        if (e.Message.Notification != null)
        {
            Debug.Log("Notification Title: " + e.Message.Notification.Title);

            if (e.Message.Data.TryGetValue(DYNAMIC_OBJECTIVE, out string objectiveIndex))
            {
                NotificationManager.NotifyObjective(e.Message.Notification.Title, objectiveIndex);
            }
            else
            {
                NotificationManager.Notify(e.Message.Notification.Title, e.Message.Notification.Body);
            }
        }
        else if (e.Message.Data.TryGetValue(DYNAMIC_OBJECTIVE, out string objectiveIndex))
        {
            e.Message.Data.TryGetValue(TITLE, out string title);
            NotificationManager.NotifyObjective(title, objectiveIndex);
        }
    }
}
