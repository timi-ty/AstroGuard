using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemListing : Button
{
    private Image highlitorImage;
    private TextMeshProUGUI priceText;
    private Image resourceImage;
    private Image decoratorImage;


    public int itemIndex { get; set; }

    private event System.Action<ItemListing> clickedEvent;

    public void Initialize(int itemIndex, Item item, string price, bool isOwned, System.Action<ItemListing> onClicked)
    {
        highlitorImage = GetComponent<Image>();
        resourceImage = transform.GetChild(0).GetComponent<Image>();
        decoratorImage = transform.GetChild(1).GetComponent<Image>();
        priceText = GetComponentInChildren<TextMeshProUGUI>();


        this.itemIndex = itemIndex;
        resourceImage.sprite = item.itemResource;
        decoratorImage.sprite = item.decoratorResource;

        priceText.text = isOwned ? "Owned" : price;

        switch (item.itemType)
        {
            case ItemType.Core:
                decoratorImage.color = item.itemColor;
                break;
            case ItemType.Blade:
                resourceImage.color = item.itemColor;
                decoratorImage.enabled = false;
                break;
        }

        clickedEvent = onClicked;

        highlitorImage.enabled = false;
    }

    public void RefreshOwnership(bool isOwned)
    {
        if(isOwned)
            priceText.text = "Owned";

        resourceImage.color = isOwned ? Color.white : Color.grey;
    }

    public void Highlight()
    {
        highlitorImage.enabled = true;
    }

    public void Deselect()
    {
        highlitorImage.enabled = false;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        if (IsInteractable())
        {
            clickedEvent?.Invoke(this);
        }
    }
}
