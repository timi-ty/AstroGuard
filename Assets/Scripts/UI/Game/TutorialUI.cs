using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class TutorialUI : MonoBehaviour
{
    public AnimatedText titleText;
    public AnimatedText infoText;

    public Button interactionInterceptor;
    public TextMeshProUGUI continuePrompt;
    public GameObject lrArrows;
    public ProgressBar progressBar;

    private void Start()
    {
        lrArrows.SetActive(false);
        continuePrompt.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(false);
    }

    #region Tutorial Prompts
    public void ShowFirstPrompt(Action onPromptShowed)
    {
        titleText.ClearVisibleText();
        infoText.ClearVisibleText();

        titleText.SetNextText("ASTROGUARD ALERT!");
        infoText.SetNextText("You are the last line of defense between the cruelty of space and the last standing mothership.");

        titleText.InflateText(() => infoText.InflateText(onPromptShowed));
    }

    public void ShowSecondPrompt(Action onPromptShowed)
    {
        titleText.ClearVisibleText();
        infoText.ClearVisibleText();

        titleText.SetNextText("ASTROGUARD ALERT!");
        infoText.SetNextText("Space will throw out all it can to destroy the mothership and stop it from reaching Planet Astron.");

        titleText.InflateText(() => infoText.InflateText(onPromptShowed));
    }

    public void ShowThirdPrompt(Action onPromptShowed)
    {
        titleText.ClearVisibleText();
        infoText.ClearVisibleText();

        titleText.SetNextText("ASTEROID APPROACHING!");
        infoText.SetNextText("Press and hold the screen to spin your blades till they reach deadly speeds.");

        titleText.InflateText(() => infoText.InflateText(onPromptShowed));
    }

    public void ShowFourthPrompt(Action onPromptShowed)
    {
        progressBar.gameObject.SetActive(false);

        titleText.ClearVisibleText();
        infoText.ClearVisibleText();

        titleText.SetNextText("SMASH THE ASTEROID!");
        infoText.SetNextText("Move to makesure your blades smash the asteroid before it touches the ship.");

        titleText.InflateText(() => infoText.InflateText(onPromptShowed));

        lrArrows.SetActive(true);
    }

    public void ShowFifthPrompt(Action onPromptShowed)
    {
        lrArrows.SetActive(false);

        titleText.ClearVisibleText();
        infoText.ClearVisibleText();

        titleText.SetNextText("WELL DONE!");
        infoText.SetNextText("Don't celebrate just yet. Some more tricky asteroids are coming your way.");

        titleText.InflateText(() => infoText.InflateText(onPromptShowed));
    }

    public void ShowSixthPrompt(Action onPromptShowed)
    {
        titleText.ClearVisibleText();
        infoText.ClearVisibleText();

        titleText.SetNextText("BLUE ASTEROIDS!");
        infoText.SetNextText("They will feign in one direction and then move in another. Don't let it trick you.");

        titleText.InflateText(() => infoText.InflateText(onPromptShowed));
    }

    public void ShowSeventhPrompt(Action onPromptShowed)
    {
        titleText.ClearVisibleText();
        infoText.ClearVisibleText();

        titleText.SetNextText("NICELY DONE!");
        infoText.SetNextText("You're getting the hang of it, but we're not done yet.");

        titleText.InflateText(() => infoText.InflateText(onPromptShowed));
    }

    public void ShowEigthPrompt(Action onPromptShowed)
    {
        titleText.ClearVisibleText();
        infoText.ClearVisibleText();

        titleText.SetNextText("PURPLE ASTEROIDS!");
        infoText.SetNextText("These asteroids bounce vigourously off the vaccuum of space. Watch out!");

        titleText.InflateText(() => infoText.InflateText(onPromptShowed));
    }

    public void ShowNinthPrompt(Action onPromptShowed)
    {
        titleText.ClearVisibleText();
        infoText.ClearVisibleText();

        titleText.SetNextText("YOU'RE A NATURAL!");
        infoText.SetNextText("The asteroids of space are no match for you. But space has one more trick up it's sleeves.");

        titleText.InflateText(() => infoText.InflateText(onPromptShowed));
    }

    public void ShowTenthPrompt(Action onPromptShowed)
    {
        titleText.ClearVisibleText();
        infoText.ClearVisibleText();

        titleText.SetNextText("BOMBS!");
        infoText.SetNextText("Bombs are twice as dangerous as asteroids. However, the explosions they cause are super efficient asteroid destroyers.");

        titleText.InflateText(() => infoText.InflateText(onPromptShowed));
    }

    public void ShowEleventhPrompt(Action onPromptShowed)
    {
        titleText.ClearVisibleText();
        infoText.ClearVisibleText();

        titleText.SetNextText("BRILLIANT!");
        infoText.SetNextText("The mothership appreciates your defensive efforts and will help with some pretty nifty power-ups.");

        titleText.InflateText(() => infoText.InflateText(onPromptShowed));
    }

    public void ShowTwelfthPrompt(Action onPromptShowed)
    {
        titleText.ClearVisibleText();
        infoText.ClearVisibleText();

        titleText.SetNextText("SHIELD!");
        infoText.SetNextText("Get the SHIELD power-up to make the mothership protect itself from damage for a short period of time.");

        titleText.InflateText(() => infoText.InflateText(onPromptShowed));
    }

    public void ShowThirteenthPrompt(Action onPromptShowed)
    {
        titleText.ClearVisibleText();
        infoText.ClearVisibleText();

        titleText.SetNextText("ATTRACTOR!");
        infoText.SetNextText("Get the ATTRACTOR power-up to create a field that draws in asteroids and bombs and also protects the ship.");

        titleText.InflateText(() => infoText.InflateText(onPromptShowed));
    }

    public void ShowFourteenthPrompt(Action onPromptShowed)
    {
        titleText.ClearVisibleText();
        infoText.ClearVisibleText();

        titleText.SetNextText("MISSILE LAUNCHER!");
        infoText.SetNextText("Get the MISSILE LAUNCHER power-up to launch projectiles that track down and destroy asteroids and bombs.");

        titleText.InflateText(() => infoText.InflateText(onPromptShowed));
    }

    public void ShowFifteenthPrompt(Action onPromptShowed)
    {
        titleText.ClearVisibleText();
        infoText.ClearVisibleText();

        titleText.SetNextText("SLOW MO!");
        infoText.SetNextText("The mothership has a special SENSOR to help you destroy multiple spaced targets by SLOWING DOWN TIME.");

        titleText.InflateText(() => infoText.InflateText(onPromptShowed));
    }

    public void ShowSixteenthPrompt(Action onPromptShowed)
    {
        titleText.ClearVisibleText();
        infoText.ClearVisibleText();

        titleText.SetNextText("TRAINING COMPLETE!");
        infoText.SetNextText("You are now ready to guard the mother ship to Planet Astron. The world is counting on you.");

        titleText.InflateText(() => infoText.InflateText(onPromptShowed));
    }
    #endregion

    public void CanProceed(bool canProceed, string continuePromptText = "Tap To Continue")
    {
        interactionInterceptor.interactable = canProceed;
        continuePrompt.gameObject.SetActive(canProceed);

        continuePrompt.text = continuePromptText;
    }

    public void SetInteraction(Action interaction)
    {
        interactionInterceptor.onClick.RemoveAllListeners();
        interactionInterceptor.onClick.AddListener(() => interaction?.Invoke());
    }

    public void SetActionProgress(float progress)
    {
        if (!progressBar.gameObject.activeSelf) progressBar.gameObject.SetActive(true);

        progressBar.SetProgressImmediate(progress, "");
    }
}
