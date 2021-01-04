using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class PlayerStats
{
    #region Singleton
    public static PlayerStats Instance { get; set; } = new PlayerStats();
    private PlayerStats()
    {
        HighScores = new SortedList<long, LeaderboardEntry>();

        activeCoreIndex = 0;
        activeBladeIndex = 10;

        Upgradables = new Dictionary<PowerType, Upgradable>();

        Upgradables.Add(PowerType.Attractor, new Upgradable());
        Upgradables.Add(PowerType.SlowMo, new Upgradable());
        Upgradables.Add(PowerType.Shield, new Upgradable());
        Upgradables.Add(PowerType.MissileLauncher, new Upgradable());
    }
    #endregion

    #region Properties
    public static string DbUserRewardsPath => ApplicationManager.DbUserPath + "Rewards/";
    #endregion

    #region Status
    private SortedList<long, LeaderboardEntry> HighScores { get; set; } = new SortedList<long, LeaderboardEntry>();
    public int ExperiencePoints { get; private set; }
    public int ExperienceLevel
    {
        get
        {
            /*Experience level is simply the nth term in an arithmentic 
            progression starting at 6 with a common difference of 2.*/
            return Mathf.CeilToInt((-5 + Mathf.Sqrt(25 + 4 * ExperiencePoints)) / 2);
        }
    }
    public int AstroGold { get; private set; }
    public int HighScore 
    { 
        get 
        {
            int highestScore = 0;

            foreach (LeaderboardEntry entry in Instance.HighScores.Values)
            {
                highestScore = Mathf.Max(highestScore, entry.score);
            }

            return highestScore;
        } 
    }
    public int TempAstroGoldPocket { get; private set; }
    public int activeCoreIndex { get; set; }
    public int activeBladeIndex { get; set; }
    public Dictionary<PowerType, Upgradable> Upgradables { get; set; }
    #endregion

    #region Utitlity Methods
    public static void InsertHighScore(int score, long sessionStartTimeInMillis)
    {
        Debug.Log("Now Time: " + sessionStartTimeInMillis);

        long sessionDuration = CurrentTime.MilliSeconds - sessionStartTimeInMillis;

        LeaderboardEntry newEntry = new LeaderboardEntry(score, Instance.ExperienceLevel, sessionDuration);

        if(Instance.HighScores.Count >= 10)
        {
            OverwriteLowestEntry(newEntry, sessionStartTimeInMillis);
        }
        else
        {
            Instance.HighScores[sessionStartTimeInMillis] = newEntry;
        }

        Debug.Log("HighScore Count: " + Instance.HighScores.Count);

        PostIfHighestEntry(newEntry);
    }

    private static void OverwriteLowestEntry(LeaderboardEntry incomingEntry, long entryKey)
    {
        if (Instance.HighScores.Count < 1) return;

        int lowestScore = Instance.HighScores.Values[0].score;
        long lowestEntryKey = Instance.HighScores.Keys[0];

        foreach(KeyValuePair<long, LeaderboardEntry> entry in Instance.HighScores)
        {
            if(entry.Value.score < lowestScore)
            {
                lowestScore = entry.Value.score;
                lowestEntryKey = entry.Key;
            }
        }

        Instance.HighScores.Remove(lowestEntryKey);
        Instance.HighScores[entryKey] = incomingEntry;
    }

    private static void PostIfHighestEntry(LeaderboardEntry incomingEntry)
    {
        int highestScore = int.MinValue;

        foreach (LeaderboardEntry entry in Instance.HighScores.Values)
        {
            highestScore = Mathf.Max(highestScore, entry.score);
        }

        if(incomingEntry.score >= highestScore)
        {
            LeaderboardManager.PostEntry(incomingEntry);
        }
    }

    public static void ObjectiveReward(int xpReward, int goldReward)
    {
        Instance.ExperiencePoints += xpReward;

        int gold = Mathf.Clamp(goldReward - 10, 0, int.MaxValue);

        RewardAstroGold(gold);

        for (int i = 0; i < 10; i++)
        {
            GameManager.SpawnGoldCoin();
        }
    }

    public static void ClaimRewardedAstroGold()
    {
        if (FirebaseUtility.CurrentUser?.UserId == null) return;

        string astroGoldPath = DbUserRewardsPath + "/AstroGold";

        FirebaseUtility.ReadFromDatabase(astroGoldPath,
            (value) =>
            {
                Dictionary<string, object> AstroGoldDict = (Dictionary<string, object>)value;

                long goldValue = 0;

                foreach (object gold in AstroGoldDict.Values)
                {
                    goldValue += (long)gold; 
                }

                if (CanClaimReward())
                {
                    FirebaseUtility.WriteToDatabase(astroGoldPath, null,
                        () =>
                        {
                            Instance.AstroGold += (int)goldValue;

                            AlertMessageInfo alertMessageInfo = new AlertMessageInfo()
                            {
                                titleText = "GOLD! GOLD! GOLD!",

                                messageText = "Congrats! you just claimed some Astro Gold. " +
                                "Head to the store to empower your power-ups with your new gold!",

                                positiveActionText = "Go to Store",
                                negativeActionText = "Dismiss",
                                isCelebratory = true,
                                reward = (int)goldValue
                            };
                            if (goldValue > 0)
                            {
                                UIManager.QueueAlertDialog(alertMessageInfo, UIManager.instance.ShowStoreUI, () => { }, "claimed_reward");
                            }
                        });
                }
                else
                {
                    AlertMessageInfo alertMessageInfo = new AlertMessageInfo()
                    {
                        titleText = "YOUR GOLD AWAITS!",

                        messageText = "You have " + goldValue.ToString() + " AstroGold rewards waiting for you to claim." +
                        "Sync your game data now and claim the rewards!",

                        positiveActionText = "Sync Game Data",
                        negativeActionText = "Dismiss"
                    };
                    if (goldValue > 0)
                    {
                        UIManager.QueueAlertDialog(alertMessageInfo, FirebaseUtility.SyncGameData, () => { }, "awaiting_reward");
                    }
                }
                    
            });
    }

    public static void RewardAstroGold(int goldReward)
    {
        if (FirebaseUtility.CurrentUser?.UserId == null) return;

        FirebaseUtility.PushToDatabase(DbUserRewardsPath + "/AstroGold", goldReward, null);
    }

    public static void PocketAstroGold(int gold)
    {
        Instance.TempAstroGoldPocket += gold;
    }

    public static void DiscardPocketedAstroGold()
    {
        Instance.TempAstroGoldPocket = 0;
    }

    public static void DepositPocketedAstroGold()
    {
        if (FirebaseUtility.CurrentUser?.UserId == null || Instance.TempAstroGoldPocket < 1) return;

        FirebaseUtility.PushToDatabase(DbUserRewardsPath + "/AstroGold", Instance.TempAstroGoldPocket,
            () =>
            {
                Instance.TempAstroGoldPocket = 0;
            });
    }

    public static bool HasAstroGold()
    {
        return Instance.AstroGold > 0;
    }

    public static bool CanClaimReward()
    {
        if (FirebaseUtility.CurrentUser == null || GameDataManager.GameData.UserID == null) return false;

        return GameDataManager.GameData.UserID == FirebaseUtility.CurrentUser.UserId;
    }

    public static void UpgradeWithAstroGold(UpgradableListing upgradableListing)
    {
        Upgradable upgradable = Instance.Upgradables[upgradableListing.upgradeType];

        if (Instance.AstroGold > 0)
        {
            Instance.AstroGold -= 1;

            upgradable.upgradeProgress++;

            Instance.Upgradables[upgradableListing.upgradeType] = upgradable;

            upgradableListing.RefreshUpgradeProgress(upgradable.upgradeProgress);
        }
    }
    #endregion

    #region Action Methods
    public void Wipe()
    {
        Instance = new PlayerStats();
    }
    #endregion
}
