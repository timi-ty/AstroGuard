using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;


// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
public class Purchaser : MonoBehaviour, IStoreListener
{
    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    // Product identifiers for all products capable of being purchased: 
    // "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
    // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
    // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

    // General product identifiers for the consumable, non-consumable, and subscription products.
    // Use these handles in the code to reference which product to purchase. Also use these values 
    // when defining the Product Identifiers on the store. Except, for illustration purposes, the 
    // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
    // specific mapping to Unity Purchasing's AddProduct, below.
    #region Non Consumable Product Ids
    public const string kProductIDCore02 = "Core02";
    public const string kProductIDCore03 = "Core03";
    public const string kProductIDCore04 = "Core04";
    public const string kProductIDCore05 = "Core05";
    public const string kProductIDCore06 = "Core06";
    public const string kProductIDCore07 = "Core07";
    public const string kProductIDCore08 = "Core08";
    public const string kProductIDCore09 = "Core09";
    public const string kProductIDCore10 = "Core10";
    public const string kProductIDBlade02 = "Blade02";
    public const string kProductIDBlade03 = "Blade03";
    public const string kProductIDBlade04 = "Blade04";
    public const string kProductIDBlade05 = "Blade05";

    public const string kProductIDNoAds = "NoAds";
    #endregion

    #region Events
    private event Action PurchaserInitialized;
    #endregion

    void Start()
    {
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }
    }

    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add a product to sell / restore by way of its identifier, associating the general identifier
        // with its store-specific identifiers.
        builder.AddProduct(kProductIDCore02, ProductType.NonConsumable);
        builder.AddProduct(kProductIDCore03, ProductType.NonConsumable);
        builder.AddProduct(kProductIDCore04, ProductType.NonConsumable);
        builder.AddProduct(kProductIDCore05, ProductType.NonConsumable);
        builder.AddProduct(kProductIDCore06, ProductType.NonConsumable);
        builder.AddProduct(kProductIDCore07, ProductType.NonConsumable);
        builder.AddProduct(kProductIDCore08, ProductType.NonConsumable);
        builder.AddProduct(kProductIDCore09, ProductType.NonConsumable);
        builder.AddProduct(kProductIDCore10, ProductType.NonConsumable);
        builder.AddProduct(kProductIDBlade02, ProductType.NonConsumable);
        builder.AddProduct(kProductIDBlade03, ProductType.NonConsumable);
        builder.AddProduct(kProductIDBlade04, ProductType.NonConsumable);
        builder.AddProduct(kProductIDBlade05, ProductType.NonConsumable);
        builder.AddProduct(kProductIDNoAds, ProductType.NonConsumable);
        // Continue adding the non-consumable product.


        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
    }

    public void AddOnInitializedListener(Action onInitialized)
    {
        PurchaserInitialized += onInitialized;

        if (IsInitialized())
        {
            onInitialized?.Invoke();
        }
    }


    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public bool PurchasedNoAds()
    {
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(kProductIDNoAds);

            if (product != null)
            {
                return product.hasReceipt;
            }
        }

        return false;
    }

    public bool OwnsItem(Item item)
    {
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(ProductIdFromItem(item));

            if (product != null)
            {
                return product.hasReceipt;
            }
        }

        return false;
    }

    public string ItemPrice(Item item)
    {
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(ProductIdFromItem(item));

            if(product != null)
            {
                return product.metadata.localizedPriceString;
            }
        }
        return "";
    }

    public void BuyItem(Item item)
    {
        if (IsInitialized())
        {
            BuyProductID(ProductIdFromItem(item));
        }
    }

    public void BuyNoAds()
    {
        if (!IsInitialized())
        {
            Debug.Log("BuyNoAd FAIL. Not initialized.");
        }
        if (PurchasedNoAds())
        {
            AlertMessageInfo alertMessageInfo = new AlertMessageInfo()
            {
                titleText = "HAHAHA!",
                messageText = "Hey! You already disabled ads, but thanks so much for trying again ;) !",
                positiveActionText = "Go to store",
                negativeActionText = "Dismiss"
            };

            UIManager.QueueAlertDialog(alertMessageInfo, UIManager.instance.ShowStoreUI, () => { });
        }
        else
        {
            BuyProductID(kProductIDNoAds);
        }
    }

    private void BuyProductID(string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                AlertMessageInfo alertMessageInfo = new AlertMessageInfo()
                {
                    titleText = "Oh, No!",
                    messageText = "Sorry, the Shop is not ready.",
                    positiveActionText = "Dismiss",
                    negativeActionText = "Go Home"
                };

                UIManager.QueueAlertDialog(alertMessageInfo, () => { }, GameManager.instance.GoHome);
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.Log("BuyProductID FAIL. Not initialized.");
            AlertMessageInfo alertMessageInfo = new AlertMessageInfo()
            {
                titleText = "Oh, No!",
                messageText = "Sorry, the Shop is not ready.",
                positiveActionText = "Dismiss",
                negativeActionText = "Go Home"
            };

            UIManager.QueueAlertDialog(alertMessageInfo, () => { }, GameManager.instance.GoHome);
        }
    }


    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) => {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }


    //  
    // --- IStoreListener
    //

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;

        PurchaserInitialized?.Invoke();
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        // A consumable product has been purchased by this user.
        if (IsConsumable(args.purchasedProduct.definition.id))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // The consumable item has been successfully purchased.
        }
        // Or ... a non-consumable product has been purchased by this user.
        else if (IsNonConsumable(args.purchasedProduct.definition.id))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // TODO: The non-consumable item has been successfully purchased, grant this item to the player.
        }
        // Or ... a subscription product has been purchased by this user.
        else if (IsSubscription(args.purchasedProduct.definition.id))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // TODO: The subscription item has been successfully purchased, grant this to the player.
        }
        // Or ... an unknown product has been purchased by this user. Fill in additional products here....
        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
        }

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 

        AlertMessageInfo alertMessageInfo = new AlertMessageInfo()
        {
            titleText = "THANK YOU!",
            messageText = "You successfully bought an exciting item from our shop. We love you!",
            positiveActionText = "Keep Shoping",
            negativeActionText = "Dismiss",
            isCelebratory = true
        };

        UIManager.QueueAlertDialog(alertMessageInfo, UIManager.instance.ShowStoreUI, UIManager.instance.ShowStoreUI);

        Analytics.LogItemPurchased(args.purchasedProduct.definition.id, args.purchasedProduct.metadata.localizedPrice, args.purchasedProduct.metadata.isoCurrencyCode);

        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }

    #region Utility
    private static string ProductIdFromItem(Item item)
    {
        switch (item.itemId)
        {
            case ItemId.Core02:
                return kProductIDCore02;
            case ItemId.Core03:
                return kProductIDCore03;
            case ItemId.Core04:
                return kProductIDCore04;
            case ItemId.Core05:
                return kProductIDCore05;
            case ItemId.Core06:
                return kProductIDCore06;
            case ItemId.Core07:
                return kProductIDCore07;
            case ItemId.Core08:
                return kProductIDCore08;
            case ItemId.Core09:
                return kProductIDCore09;
            case ItemId.Core10:
                return kProductIDCore10;
            case ItemId.Blade02:
                return kProductIDBlade02;
            case ItemId.Blade03:
                return kProductIDBlade03;
            case ItemId.Blade04:
                return kProductIDBlade04;
            case ItemId.Blade05:
                return kProductIDBlade05;
        }
        return "";
    }

    private static bool IsConsumable(string productId)
    {
        return false;
    }

    private static bool IsNonConsumable(string productId)
    {
        if (string.Equals(productId, kProductIDCore02, StringComparison.Ordinal))
        {
            return true;
        }
        else if (string.Equals(productId, kProductIDCore03, StringComparison.Ordinal))
        {
            return true;
        }
        else if (string.Equals(productId, kProductIDCore04, StringComparison.Ordinal))
        {
            return true;
        }
        else if (string.Equals(productId, kProductIDCore05, StringComparison.Ordinal))
        {
            return true;
        }
        else if (string.Equals(productId, kProductIDCore06, StringComparison.Ordinal))
        {
            return true;
        }
        else if (string.Equals(productId, kProductIDCore07, StringComparison.Ordinal))
        {
            return true;
        }
        else if (string.Equals(productId, kProductIDCore08, StringComparison.Ordinal))
        {
            return true;
        }
        else if (string.Equals(productId, kProductIDCore09, StringComparison.Ordinal))
        {
            return true;
        }
        else if (string.Equals(productId, kProductIDCore10, StringComparison.Ordinal))
        {
            return true;
        }
        else if (string.Equals(productId, kProductIDBlade02, StringComparison.Ordinal))
        {
            return true;
        }
        else if (string.Equals(productId, kProductIDBlade03, StringComparison.Ordinal))
        {
            return true;
        }
        else if (string.Equals(productId, kProductIDBlade04, StringComparison.Ordinal))
        {
            return true;
        }
        else if (string.Equals(productId, kProductIDBlade05, StringComparison.Ordinal))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private static bool IsSubscription(string productId)
    {
        return false;
    }
    #endregion
}