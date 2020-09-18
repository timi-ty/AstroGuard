using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LeaderboardUI : MonoBehaviour
{
    #region Prefabs
    [Header("Prefabs")]
    public LeaderboardListing leaderboardListingPrefab;
    #endregion

    #region Components
    [Header("Content Holders")]
    public RectTransform topContentHolder;
    public RectTransform bottomContentHolder;
    #endregion

    #region Components
    [Header("Scroll Views")]
    public ScrollRect topScrollView;
    public ScrollRect bottomScrollView;
    #endregion

    #region Unity Runtime
    void Start()
    {
        LeaderboardManager.AddTopEntriesLoadedListener(UpdateLeaderboard);
        LeaderboardManager.AddNeighbourEntriesLoadedListener(UpdateBottomLeaderBoard);
    }
    #endregion

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void UpdateLeaderboard(List<LeaderboardEntry> leaderboardEntries, bool isPlayerInTop, int playerEntryIndex)
    {
        for (int i = 0; i < topContentHolder.childCount; i++)
        {
            Destroy(topContentHolder.GetChild(i).gameObject);
        }

        for (int i = 0; i < leaderboardEntries.Count; i++)
        {
            InitializeLeader((i + 1).ToString(), leaderboardEntries[i], 
                topContentHolder, highlight: isPlayerInTop && i == playerEntryIndex);
        }

        if (!isPlayerInTop)
        {
            bottomScrollView.gameObject.SetActive(true);
        }
        else
        {
            bottomScrollView.gameObject.SetActive(false);
        }
    }

    public void UpdateBottomLeaderBoard(List<LeaderboardEntry> boardEntries, int playersEntry)
    {
        for (int i = 0; i < bottomContentHolder.childCount; i++)
        {
            Destroy(bottomContentHolder.GetChild(i).gameObject);
        }

        for (int i = 0; i < boardEntries.Count; i++)
        {
            InitializeLeader("N/A", boardEntries[i], bottomContentHolder, highlight: i == playersEntry);
        }
    }

    #region Utility
    private void InitializeLeader(string rank, LeaderboardEntry leaderboardEntry, RectTransform contentHolder, bool highlight)
    {
        float desiredWidth = (contentHolder.parent as RectTransform).rect.width;
        float desiredHeight = desiredWidth * 0.14124f;

        LeaderboardListing leaderboardListing = Instantiate(leaderboardListingPrefab, transform.position, Quaternion.identity, contentHolder);

        leaderboardListing.Initialize(rank, leaderboardEntry, desiredWidth, desiredHeight, highlight);
    }
    #endregion
}
