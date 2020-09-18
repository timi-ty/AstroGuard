using UnityEngine;
using TMPro;

public class AstroGoldDisplay : MonoBehaviour
{
    private TextMeshProUGUI textMesh;

    private void Awake()
    {
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
        Refresh();
    }

    public void Refresh()
    {
        textMesh.text = PlayerStats.Instance.AstroGold.ToString();
    }

    public void Refresh(int amount)
    {
        textMesh.text = amount.ToString();
    }
}
