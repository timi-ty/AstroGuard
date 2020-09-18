using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(WatchRewardedAdButtonAttachment))]
public class WatchRewardedAdButton : Button
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        if (IsInteractable())
        {
            WatchRewardedAdButtonAttachment attachment = GetComponent<WatchRewardedAdButtonAttachment>();

            attachment.OnRequestRewardedAd.Invoke(attachment.rewardType);
        }
    }
}