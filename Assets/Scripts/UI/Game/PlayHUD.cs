﻿using UnityEngine;
using TMPro;

public class PlayHUD : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public ProgressBar levelProgressBar;
    public GameObject pausePanel;
    public ObjectiveUI objectiveUi;
    
    public void Toggle(bool isPlaying)
    {
        gameObject.SetActive(true);

        pausePanel.SetActive(!isPlaying);

        UpdateHud(immediately: true);

        ControlObjectiveUI(isPlaying);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void UpdateHud(bool immediately)
    {
        float levelProgress = GameManager.levelProgress;

        if (!gameObject.activeInHierarchy) return;

        scoreText.text = Session.Score.ToString();

        string levelText = "LEVEL " + GameManager.currentLevel.ToString("D2");

        if (immediately) levelProgressBar.SetProgressImmediate(levelProgress, levelText);
        else levelProgressBar.SetProgress(levelProgress, levelText);
    }


    private void ControlObjectiveUI(bool isPlaying)
    {
        if (objectiveUi.isShowingList && isPlaying)
        {
            objectiveUi.Hide();
        }
        else if (!isPlaying)
        {
            objectiveUi.Show(-1);
        }
        else if (isPlaying)
        {
            objectiveUi.Show(2);
        }
    }

    public void Notify(Objective objective)
    {
        objectiveUi.Notify(objective);
    }
}