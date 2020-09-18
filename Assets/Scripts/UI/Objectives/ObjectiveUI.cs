using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class ObjectiveUI : MonoBehaviour
{
    #region Components
    [Header("Components")]
    public Animator animator;
    #endregion

    #region Elements
    [Header("Elements")]
    public List<ObjectiveListing> objectiveListings = new List<ObjectiveListing>(4);
    #endregion

    #region Extras
    [Header("Extras")]
    public AudioClip objectiveCompleteClip;
    #endregion

    #region Properties
    public bool isShowingList { get; set; }
    #endregion

    #region Animator Parameters
    private int SHOW_LIST_ANIM_ID = Animator.StringToHash("showList");
    private int HIDE_LIST_ANIM_ID = Animator.StringToHash("hideList");
    private int NOTIFY_ANIM_ID = Animator.StringToHash("notify");
    #endregion


    public void Notify(Objective objective)
    {
        objectiveListings[1].Refresh(objective);
        if (animator.gameObject.activeInHierarchy)
        {
            animator.SetTrigger(NOTIFY_ANIM_ID);
        }
        AudioManager.PlayUIClip(objectiveCompleteClip);
    }

    /// <summary>
    /// Show the ui for duration in seconds. -1 for indefinite show till hide is called.
    /// </summary>
    /// <param name="duration">in seconds</param>
    public void Show(float duration)
    {
        if (isShowingList) return;

        RefreshAll();
        animator.SetTrigger(SHOW_LIST_ANIM_ID);
        isShowingList = true;

        CancelInvoke("Hide");

        if(duration > 0)
        {
            Invoke("Hide", duration);
        }
    }

    public void Hide()
    {
        if (!isShowingList) return;

        animator.SetTrigger(HIDE_LIST_ANIM_ID);
        isShowingList = false;
    }

    private void RefreshAll()
    {
        for (int i = 0; i < objectiveListings.Count; i++)
        {
            objectiveListings[i].Refresh(ObjectiveManager.GetActiveObjective(i));
        }
    }
}