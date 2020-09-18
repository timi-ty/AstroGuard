using UnityEngine;

public enum ItemType { Core, Blade}
public enum ItemId { Core01, Core02, Core03, Core04, Core05, Core06, Core07, Core08, Core09, Core10, Blade01, Blade02, Blade03, Blade04, Blade05 }

[System.Serializable]
public class Item
{
    public ItemId itemId;
    public ItemType itemType;
    public Sprite itemResource;
    public Sprite decoratorResource;
    public Color itemColor;
}
