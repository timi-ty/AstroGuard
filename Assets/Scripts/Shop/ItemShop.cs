using UnityEngine;
using System.Collections.Generic;

public class ItemShop : MonoBehaviour
{
    #region Shop Content
    public List<Item> Items = new List<Item>();
    #endregion

    private Purchaser Purchaser { get; set; }
    private int lastSelectedItem { get; set; }
    private bool isShopReady { get; set; }

    private event System.Action ShopReady;

    private void Start()
    {
        Purchaser = GetComponent<Purchaser>();

        Purchaser.AddOnInitializedListener(
            () =>
            { 
                isShopReady = true;
                ShopReady?.Invoke();
            });
    }

    public void AddOnShopReadyListener(System.Action onShopReady)
    {
        ShopReady += onShopReady;

        if (isShopReady)
        {
            onShopReady?.Invoke();
        }
    }

    public bool OwnsItem(int itemIndex)
    {
        if (itemIndex == 0 || itemIndex == 10)
        {
            return true;
        }

        if (isShopReady)
        {
            Item item = Items[itemIndex];

            return Purchaser.OwnsItem(item);
        }
        else
        {
            //AlertMessageInfo alertMessageInfo = new AlertMessageInfo()
            //{
            //    titleText = "Oh, No!",
            //    messageText = "Sorry, the Shop is not ready.",
            //    positiveActionText = "Dismiss",
            //    negativeActionText = "Go Home"
            //};

            //UIManager.QueueAlertDialog(alertMessageInfo, GameManager.instance.GoHome, GameManager.instance.GoHome);

            return false;
        }
    }

    public string ItemPrice(int itemIndex)
    {
        if (itemIndex == 0 || itemIndex == 10)
        {
            return "Owned";
        }

        if (isShopReady)
        {
            Item item = Items[itemIndex];

            return Purchaser.ItemPrice(item);
        }
        else
        {
            //AlertMessageInfo alertMessageInfo = new AlertMessageInfo()
            //{
            //    titleText = "Oh, No!",
            //    messageText = "Sorry, the Shop is not ready.",
            //    positiveActionText = "Dismiss",
            //    negativeActionText = "Go Home"
            //};

            //UIManager.QueueAlertDialog(alertMessageInfo, GameManager.instance.GoHome, GameManager.instance.GoHome);

            return "$2.50";
        }
    }

    public Item SelectItem(int itemIndex)
    {
        Item item = Items[itemIndex];

        Analytics.LogItemSelected(item.itemId.ToString());

        if (itemIndex == 0 || itemIndex == 10)
        {
            ActivateItem(itemIndex);
            return item;
        }

        lastSelectedItem = itemIndex;

        if (isShopReady)
        {
            if (Purchaser.OwnsItem(item))
            {
                ActivateItem(itemIndex);
                return item;
            }
            else
            {
                Purchaser.BuyItem(item);
                return null;
            }
        }
        else
        {
            AlertMessageInfo alertMessageInfo = new AlertMessageInfo()
            {
                titleText = "Oh, No!",
                messageText = "Sorry, the Shop is not ready.",
                positiveActionText = "Retry",
                negativeActionText = "Go Home"
            };

            UIManager.QueueAlertDialog(alertMessageInfo, RetrySelect, GameManager.instance.GoHome, "shop_not_ready");
            return null;
        }
    }

    private void RetrySelect()
    {
        SelectItem(lastSelectedItem);
    }

    private void ActivateItem(int itemIndex)
    {
        GameManager.ActivateContent(itemIndex);
    }
}
