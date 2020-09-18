using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;

public class LeaderboardManager : MonoBehaviour
{
    #region Singleton
    public static LeaderboardManager instance { get; private set; }
    #endregion

    #region Constants
    public const string LEADERBOARD_PATH = ApplicationManager.DB_APP_PATH + "Leaderboard/";
    #endregion

    #region Data
    private List<LeaderboardEntry> top100Enties;
    private List<LeaderboardEntry> playersNeighbourEntries;
    private bool isPlayerInTop;
    private int playerTopEntryIndex;
    private int playerBottomEntryIndex;
    #endregion

    #region Events
    private static event Action<List<LeaderboardEntry>, bool, int> topEntriesLoaded;
    private static event Action<List<LeaderboardEntry>, int> neighbourEntriesLoaded;
    #endregion

    #region Unity Runtime
    void Awake()
    {
        #region Singleton
        if (!instance)
        {
            instance = this;

            Initialize();
        }
        else if (!instance.Equals(this))
        {
            Destroy(gameObject);
        }
        #endregion
    }
    #endregion

    private void Initialize()
    {
        FirebaseUtility.AddOnAuthListener(QueryTop);
        FirebaseUtility.AddOnAuthListener(QueryCloseToUser);
    }

    public static void AddTopEntriesLoadedListener(Action<List<LeaderboardEntry>, bool, int> onTopEntriesLoaded)
    {
        topEntriesLoaded += onTopEntriesLoaded;

        if (instance.top100Enties != null)
        {
            onTopEntriesLoaded?.Invoke(instance.top100Enties, instance.isPlayerInTop, instance.playerTopEntryIndex);
        }
    }

    public static void AddNeighbourEntriesLoadedListener(Action<List<LeaderboardEntry>, int> onNeighbourEntriesLoaded)
    {
        neighbourEntriesLoaded += onNeighbourEntriesLoaded;

        if (instance.playersNeighbourEntries != null)
        {
            onNeighbourEntriesLoaded?.Invoke(instance.top100Enties, instance.playerBottomEntryIndex);
        }
    }

    #region Leaderboard Queries
    private void QueryTop(FirebaseUser user)
    {
        if (user == null) return;

        FirebaseUtility.QueryFromDatabase(LEADERBOARD_PATH, orderByChild: "score", limitToLast: 100,
            (queriedDict) =>
            {
                List<LeaderboardEntry> leaderboardEntries = new List<LeaderboardEntry>();

                isPlayerInTop = false;

                int playerEntryIndexReversed = -1;

                foreach (object queryItem in queriedDict)
                {
                    Dictionary<string, object> entryDict = (Dictionary<string, object>) queryItem;

                    LeaderboardEntry entry = new LeaderboardEntry()
                    {
                        score = (int)(long)entryDict["score"],
                        expLevel = (int)(long)entryDict["expLevel"],
                        sessionDuration = (long)entryDict["sessionDuration"],
                        displayName = (string)entryDict["displayName"],
                        userId = (string)entryDict["userId"]
                    };

                    if (entry.userId?.Equals(user?.UserId) ?? false)
                    {
                        isPlayerInTop = true;
                        playerEntryIndexReversed = leaderboardEntries.Count;
                    }

                    leaderboardEntries.Add(entry);
                }

                leaderboardEntries.Reverse();

                playerTopEntryIndex = leaderboardEntries.Count - 1 - playerEntryIndexReversed;

                top100Enties = leaderboardEntries;

                topEntriesLoaded?.Invoke(leaderboardEntries, isPlayerInTop, playerTopEntryIndex);
            });
    }

    private void QueryCloseToUser(FirebaseUser user)
    {
        if (user == null) return;

        FirebaseUtility.ReadFromDatabase(LEADERBOARD_PATH + FirebaseUtility.CurrentUser.UserId,
            (value) =>
            {
                if (value == null) return;

                Dictionary<string, object> entryDict = (Dictionary<string, object>)value;

                LeaderboardEntry userEntry = new LeaderboardEntry()
                {
                    score = (int)(long)entryDict["score"],
                    expLevel = (int)(long)entryDict["expLevel"],
                    sessionDuration = (long)entryDict["sessionDuration"],
                    displayName = (string)entryDict["displayName"],
                    userId = (string)entryDict["userId"]
                };

                int startAt = Mathf.Clamp(userEntry.score - 50, 0, userEntry.score);
                int endAt = userEntry.score + 50;

                FirebaseUtility.QueryFromDatabase(LEADERBOARD_PATH, orderByChild: "score", startAt, endAt,
                (queriedDict) =>
                {
                    List<LeaderboardEntry> boardEntries = new List<LeaderboardEntry>();

                    int playerEntryIndexReversed = -1;

                    foreach (object queryItem in queriedDict)
                    {
                        Dictionary<string, object> itemDict = (Dictionary<string, object>)queryItem;

                        LeaderboardEntry entry = new LeaderboardEntry()
                        {
                            score = (int)(long)itemDict["score"],
                            sessionDuration = (long)itemDict["sessionDuration"],
                            displayName = (string)itemDict["displayName"],
                            userId = (string)itemDict["userId"]
                        };

                        if (entry.userId?.Equals(user?.UserId) ?? false)
                        {
                            playerEntryIndexReversed = boardEntries.Count;
                        }

                        boardEntries.Add(entry);
                    }

                    boardEntries.Reverse();

                    playerBottomEntryIndex = boardEntries.Count - 1 - playerEntryIndexReversed;

                    playersNeighbourEntries = boardEntries;

                    neighbourEntriesLoaded?.Invoke(boardEntries, playerBottomEntryIndex);
                });
            });
    }
    #endregion

    public static void PostEntry(LeaderboardEntry entry)
    {
        if(FirebaseUtility.CurrentUser?.UserId != null)
        {
            entry.userId = FirebaseUtility.CurrentUser.UserId;
            entry.displayName = FirebaseUtility.CurrentUser.DisplayName;

            string entryAsJson = JsonUtility.ToJson(entry);

            FirebaseUtility.UploadJsonToDatabase(LEADERBOARD_PATH + FirebaseUtility.CurrentUser.UserId, entryAsJson);
        }
    }
}