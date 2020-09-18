using UnityEngine;
using UnityEngine.Events;

public class WatchRewardedAdButtonAttachment : MonoBehaviour
{

    public RewardType rewardType;

    public RequestRewardedAd OnRequestRewardedAd;

    [System.Serializable]
    public class RequestRewardedAd : UnityEvent<RewardType>
    {

    }
}
