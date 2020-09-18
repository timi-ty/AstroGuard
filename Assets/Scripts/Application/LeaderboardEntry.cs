[System.Serializable]
public class LeaderboardEntry
{
    public int score;

    public int expLevel;

    public long sessionDuration;

    public string userId;

    public string displayName;

    public LeaderboardEntry()
    {
        userId = FirebaseUtility.CurrentUser?.UserId;
        displayName = FirebaseUtility.CurrentUser?.DisplayName;
    }

    public LeaderboardEntry(int score, int expLevel, long sessionDuration)
    {
        this.score = score;
        this.expLevel = expLevel;
        this.sessionDuration = sessionDuration;
        userId = FirebaseUtility.CurrentUser?.UserId;
        displayName = FirebaseUtility.CurrentUser?.DisplayName;
    }
}