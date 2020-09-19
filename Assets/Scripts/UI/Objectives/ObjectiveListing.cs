using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveListing : MonoBehaviour
{

    #region Components
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI completionText;
    public Image tickMark;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI xpText;
    public TextMeshProUGUI goldText;
    #endregion

    public void Refresh(Objective objective)
    {
        if (objective.IsCompleted)
        {
            completionText.enabled = false;
            tickMark.enabled = true;

            titleText.text = "COMPLETED";
        }
        else
        {
            completionText.enabled = true;
            tickMark.enabled = false;

            titleText.text = "COMPLETION";
        }

        descriptionText.text = objective.Description;
        xpText.text = "XP: " + objective.XpReward.ToString();
        goldText.text = objective.GoldReward.ToString();
        completionText.text = ((int)objective.Completion).ToString() + "%";
    }
}
