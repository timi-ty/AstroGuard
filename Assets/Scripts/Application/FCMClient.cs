using UnityEngine;
using Firebase.Messaging;

public class FCMClient : MonoBehaviour
{
    #region Singleton
    public static FCMClient instance { get; private set; }
    #endregion

    private const string NotificationKey = "notification";

    private void Awake()
    {
        #region Singleton
        DontDestroyOnLoad(gameObject);

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

    public static void Initialize()
    {
        FirebaseMessaging.SubscribeAsync("Game_Notification");

        FirebaseMessaging.TokenReceived += instance.OnTokenReceived;
        FirebaseMessaging.MessageReceived += instance.OnMessageReceived;
    }

    public void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        Debug.Log("Received Registration Token: " + token.Token);
    }

    public void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        Debug.Log("Received a new message");

        if (e.Message.From.Length > 0)
            Debug.Log("from: " + e.Message.From);

        if (e.Message.Data.Count > 0)
        {
            Debug.Log("data:");

            foreach (System.Collections.Generic.KeyValuePair<string, string> messageData in
                     e.Message.Data)
            {
                Debug.Log("  " + messageData.Key + ": " + messageData.Value);
                if (messageData.Key.Equals(NotificationKey))
                {
                    NotificationManager.Notify(messageData.Value);
                }
            }
        }
    }
}
