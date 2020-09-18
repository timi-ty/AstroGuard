using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public struct AlertMessageInfo
{
    public string titleText { get; set; }
    public string messageText { get; set; }
    public string positiveActionText { get; set; }
    public string negativeActionText { get; set; }
    public bool isCelebratory { get; set; }
    public int reward { get; set; }
}

public class AlertDialog : MonoBehaviour
{
    #region Properties 
    private bool isShowing{ get; set; }
    #endregion

    #region Components
    [Header("Components")]
    public RectTransform dialogBox;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI messageText;
    public AstroGoldDisplay astroGoldDisplay;
    public Button positiveButton;
    public Button negativeButton;
    public ParticleSystem celebrationPS;

    private Animator dialogAnimator;
    #endregion

    #region Extras
    public AudioClip popClip;
    public AudioClip celebrationClip;
    #endregion

    #region Cache
    private Stack<AlertDialogContainer> alertDialogs { get; set; } = new Stack<AlertDialogContainer>();
    #endregion

    #region Animator Parameters
    private int SHOW_DIALOG_ANIM_ID = Animator.StringToHash("showDialog");
    private int HIDE_DIALOG_ANIM_ID = Animator.StringToHash("hideDialog");
    #endregion


    private void Start()
    {
        SizeDialogBox();

        dialogAnimator = GetComponent<Animator>();

        UIManager.instance.StartCoroutine(ShowFromQueue());

        gameObject.SetActive(false);
    }

    public void Queue(AlertMessageInfo messageInfo, UnityAction positiveAction, UnityAction negativeAction)
    {
        AlertDialogContainer alertDialog = new AlertDialogContainer()
        {
            messageInfo = messageInfo,
            positiveAction = positiveAction,
            negativeAction = negativeAction
        };

        alertDialogs.Push(alertDialog);
    }

    private IEnumerator ShowFromQueue()
    {
        while (true)
        {
            yield return new WaitWhile(() => GameManager.isInGame || alertDialogs.Count <= 0);

            Show();

            AlertDialogContainer alertDialog = alertDialogs.Pop();

            titleText.text = alertDialog.messageInfo.titleText;
            messageText.text = alertDialog.messageInfo.messageText;
            positiveButton.GetComponentInChildren<TextMeshProUGUI>().text = alertDialog.messageInfo.positiveActionText;
            negativeButton.GetComponentInChildren<TextMeshProUGUI>().text = alertDialog.messageInfo.negativeActionText;

            if(alertDialog.messageInfo.reward > 0)
            {
                astroGoldDisplay.gameObject.SetActive(true);
                astroGoldDisplay.Refresh(alertDialog.messageInfo.reward);
            }
            else
            {
                astroGoldDisplay.gameObject.SetActive(false);
            }
            if (alertDialog.messageInfo.isCelebratory)
            {
                celebrationPS.Play();
                AudioManager.PlayUIClip(celebrationClip);
            }

            alertDialog.positiveAction += Hide;
            alertDialog.negativeAction += Hide;

            positiveButton.onClick.RemoveAllListeners();
            negativeButton.onClick.RemoveAllListeners();

            positiveButton.onClick.AddListener(alertDialog.positiveAction);
            negativeButton.onClick.AddListener(alertDialog.negativeAction);

            isShowing = true;

            yield return new WaitWhile(() => isShowing);
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);

        dialogAnimator.SetTrigger(SHOW_DIALOG_ANIM_ID);

        AudioManager.PlayUIClip(popClip);
    }

    public void Hide()
    {
        dialogAnimator.SetTrigger(HIDE_DIALOG_ANIM_ID);
    }

    private void Rest()
    {
        (dialogBox as RectTransform).localScale = Vector3.zero;
        isShowing = false;
        gameObject.SetActive(false);
    }

    #region Utility Methods
    private void SizeDialogBox()
    {
        float width = dialogBox.rect.width;

        float height = Mathf.Clamp(width * 1.48f, 870, 1200);

        width = height * 0.6757f;

        dialogBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        dialogBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }
    #endregion

    private class AlertDialogContainer
    {
        public AlertMessageInfo messageInfo { get; set; }
        public UnityAction positiveAction { get; set; }
        public UnityAction negativeAction { get; set; }
    }
}