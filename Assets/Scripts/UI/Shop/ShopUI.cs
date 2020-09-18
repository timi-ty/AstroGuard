using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopUI : MonoBehaviour
{
    public ItemShop ItemShop;

    #region Prefabs
    public ItemListing itemListingPrefab;
    public UpgradableListing upgradableListingPrefab;
    #endregion

    #region Content Holders
    [Header("Content")]
    public AstroGoldDisplay astroGoldDisplay;
    public RectTransform coresContentHoler;
    public RectTransform bladesContentHoler;
    public RectTransform upgradablesContentHolder;
    #endregion

    #region Shop Images
    public Sprite attractorUpgradableImage;
    public Sprite missileLauncherUpgradableImage;
    public Sprite slowMoUpgradableImage;
    public Sprite shieldUpgradableImage;
    #endregion

    private void Start()
    {
        PopulateItemShop();

        PopulateUpgradableShop();

        FitGridToScreenWidth(coresContentHoler, 2, 3, Side.Left);
        FitGridToScreenWidth(bladesContentHoler, 1, 3, Side.Right);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        astroGoldDisplay.Refresh();
        PopulateItemShop();
        RefreshUpgradableShop();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void PopulateItemShop()
    {
        ClearItemListings();

        for (int i = 0; i < ItemShop.Items.Count; i++)
        {
            InitializeItemListing(i);
        }
    }

    private void RefreshUpgradableShop()
    {
        foreach (Transform child in upgradablesContentHolder)
        {
            UpgradableListing upgradableListing = child.GetComponent<UpgradableListing>();
            if (upgradableListing)
            {
                Upgradable upgradable = PlayerStats.Instance.Upgradables[upgradableListing.upgradeType];

                upgradableListing.RefreshUpgradeProgress(upgradable.upgradeProgress);
            }
        }
    }

    public void PopulateUpgradableShop()
    {
        foreach (KeyValuePair<PowerType, Upgradable> upgradable in PlayerStats.Instance.Upgradables)
        {
            InitializeUpgradableListing(upgradable.Key);
        }
    }

    public void SelectItem(ItemListing itemListing)
    {
        Item item = ItemShop.SelectItem(itemListing.itemIndex);

        if (item != null)
        {
            switch (item.itemType)
            {
                case ItemType.Core:
                    DeselectAllCores();
                    itemListing.Highlight();
                    break;
                case ItemType.Blade:
                    DeselectAllBlades();
                    itemListing.Highlight();
                    break;
            }
        }
    }

    #region Utility
    private void ClearItemListings()
    {
        for (int i = 0; i < coresContentHoler.childCount; i++)
        {
            Destroy(coresContentHoler.GetChild(i).gameObject);
        }
        for (int i = 0; i < bladesContentHoler.childCount; i++)
        {
            Destroy(bladesContentHoler.GetChild(i).gameObject);
        }
    }

    private void InitializeItemListing(int itemIndex)
    {
        Item item = ItemShop.Items[itemIndex];

        RectTransform holder = item.itemType == ItemType.Blade ? bladesContentHoler : coresContentHoler;

        ItemListing itemListing = Instantiate(itemListingPrefab, holder);

        itemListing.Initialize(itemIndex, item, ItemShop.ItemPrice(itemIndex), ItemShop.OwnsItem(itemIndex), SelectItem);
    }
    private void InitializeUpgradableListing(PowerType upgradableType)
    {
        Sprite resourceImage = attractorUpgradableImage;

        switch (upgradableType)
        {
            case PowerType.MissileLauncher:
                resourceImage = missileLauncherUpgradableImage;
                break;
            case PowerType.Attractor:
                resourceImage = attractorUpgradableImage;
                break;
            case PowerType.Shield:
                resourceImage = shieldUpgradableImage;
                break;
            case PowerType.SlowMo:
                resourceImage = slowMoUpgradableImage;
                break;
        }

        Upgradable upgradable = PlayerStats.Instance.Upgradables[upgradableType];

        UpgradableListing upgradableListing = Instantiate(upgradableListingPrefab, upgradablesContentHolder);

        float desiredHeight = (upgradablesContentHolder.parent as RectTransform).rect.height;
        float desiredWidth = desiredHeight;

        upgradableListing.Initialize(upgradableType, resourceImage, upgradable.upgradeProgress, 
            OnUpgradePressed, PlayerStats.HasAstroGold(), desiredWidth, desiredHeight);
    }

    public void OnUpgradePressed(UpgradableListing upgradableListing)
    {
        PlayerStats.UpgradeWithAstroGold(upgradableListing);
        astroGoldDisplay.Refresh();
    }

    private void DeselectAllCores()
    {
        for (int i = 0; i < coresContentHoler.childCount; i++)
        {
            Transform coreTransform = coresContentHoler.GetChild(i);

            ItemListing itemListing = coreTransform.GetComponent<ItemListing>();

            if(itemListing != null)
            {
                itemListing.Deselect();
            }
        }
    }
    private void DeselectAllBlades()
    {
        for (int i = 0; i < bladesContentHoler.childCount; i++)
        {
            Transform bladeTransform = bladesContentHoler.GetChild(i);

            ItemListing itemListing = bladeTransform.GetComponent<ItemListing>();

            if (itemListing != null)
            {
                itemListing.Deselect();
            }
        }
    }

    private void FitGridToScreenWidth(RectTransform contentHolder, int columnCount, int totalColumnCount, Side anchorSide)
    {
        RectTransform scrollViewRect = (contentHolder.parent.parent as RectTransform);

        RectTransform itemShopRect = (scrollViewRect.parent as RectTransform);

        float gridWidth = itemShopRect.rect.width * (columnCount/ (float) totalColumnCount);

        switch (anchorSide)
        {
            case Side.Left:
                scrollViewRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, gridWidth);
                break;
            case Side.Right:
                scrollViewRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, gridWidth);
                break;
        }


        int totalPadding = Mathf.RoundToInt((contentHolder.parent as RectTransform).rect.width / 30.0f);
        int totalSpacing = Mathf.RoundToInt((contentHolder.parent as RectTransform).rect.width / 10.0f);
        int columnWidth = Mathf.RoundToInt(((contentHolder.parent as RectTransform).rect.width - totalSpacing - totalPadding) / columnCount);

        int eachSpacing = Mathf.RoundToInt(totalSpacing / 4.0f);


        GridLayoutGroup gridLayoutGroup = contentHolder.GetComponent<GridLayoutGroup>();

        gridLayoutGroup.cellSize = new Vector2(columnWidth, columnWidth);

        gridLayoutGroup.spacing = new Vector2(eachSpacing, eachSpacing);

        gridLayoutGroup.padding = new RectOffset(totalPadding / 2, totalPadding / 2, totalPadding / 2, totalPadding / 2);

        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;

        gridLayoutGroup.constraintCount = columnCount;
    }
    #endregion
}