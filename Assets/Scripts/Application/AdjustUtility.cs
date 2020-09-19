using com.adjust.sdk;

public static class AdjustUtility
{

    private const string TokenIntroLevelsCompleted = "ceqk2h";
    private const string TokenObjectiveCompleted = "r2e2vj";
    private const string TokenPowerUpCollected = "nb6zqy";
    private const string TokenPurchasedItem = "h63823";
    private const string TokenSelectedItem = "5sasip";
    private const string TokenSessionEnded = "tfmmqm";
    private const string TokenSessionRestored = "uqtcv5";
    private const string TokenSignIn = "mubtmi";
    private const string TokenSignOut = "er12xp";
    private const string TokenSignUp = "npmj81";

    #region Track Specific Events
    public static void TrackSessionEnded()
    {
        TrackEvent(TokenSessionEnded);
    }

    public static void TrackSessionRestored()
    {
        TrackEvent(TokenSessionRestored);
    }

    public static void TrackItemPurchased(string itemId, decimal price, string currency)
    {
        TrackPurchase(TokenPurchasedItem, itemId, price, currency);
    }

    public static void TrackItemSelected()
    {
        TrackEvent(TokenSelectedItem);
    }

    public static void TrackIntroLevelsCompleted()
    {
        TrackEvent(TokenIntroLevelsCompleted);
    }

    public static void TrackPowerUpCollected()
    {
        TrackEvent(TokenPowerUpCollected);
    }

    public static void TrackObjectiveCompleted()
    {
        TrackEvent(TokenObjectiveCompleted);
    }

    public static void TrackSignIn()
    {
        TrackEvent(TokenSignIn);
    }

    public static void TrackSignUp()
    {
        TrackEvent(TokenSignUp);
    }

    public static void TrackSignOut()
    {
        TrackEvent(TokenSignOut);
    }
    #endregion

    private static void TrackEvent(string eventToken)
    {
        AdjustEvent adjustEvent = new AdjustEvent(eventToken);

        Adjust.trackEvent(adjustEvent);
    }

    private static void TrackPurchase(string eventToken, string ItemId, decimal price, string currency)
    {
        AdjustEvent adjustEvent = new AdjustEvent(eventToken);

        adjustEvent.setRevenue((double)price, currency);
        adjustEvent.setTransactionId(ItemId);

        Adjust.trackEvent(adjustEvent);
    }
}
