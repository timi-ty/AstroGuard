using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardListing : MonoBehaviour
{

    public TextMeshProUGUI nameRankText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;
    public Image highlitorImage;

    public void Initialize(string rank, LeaderboardEntry leaderboardEntry, float width, float height, bool highllght)
    {
        (transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        (transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

        nameRankText.text = rank + ". " + leaderboardEntry.displayName;
        scoreText.text = leaderboardEntry.score.ToString("D3");
        levelText.text = "Level " + leaderboardEntry.expLevel.ToString("D2");

        highlitorImage.enabled = highllght;
        Color textColor = highllght ? new Color(0.5333334f, 0.854902f, 1, 1) : Color.white;

        nameRankText.color = scoreText.color = levelText.color = textColor;
    }
}
