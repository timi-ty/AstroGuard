using UnityEngine;
using TMPro;

public class HighscoreDisplay : MonoBehaviour
{
    private TextMeshProUGUI textMesh;

    private void Awake()
    {
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
        Refresh();
    }

    public void Refresh()
    {
        textMesh.text = PlayerStats.Instance.HighScore.ToString();
    }

    public void Refresh(int amount)
    {
        textMesh.text = amount.ToString();
    }
}
