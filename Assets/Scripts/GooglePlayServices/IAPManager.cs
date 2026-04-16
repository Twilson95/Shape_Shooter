using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;
using System.Collections;


public class IAPManager : MonoBehaviour, IDetailedStoreListener
{
    IStoreController m_StoreController; // The Unity Purchasing system.

    //Your products IDs. They should match the ids of your products in your store.
    public string goldProductId1 = "shapeshooter_500_gold_coins";
    public string goldProductId2 = "shapeshooter_1000_gold_coins";
    public string goldProductId3 = "shapeshooter_2500_gold_coins";
    public string goldProductId4 = "shapeshooter_5000_gold_coins";
    public string removeAdsProductId = "shapeshooter_remove_ads";

    public PremiumManager premiumManager;
    public GameController gameController;
    public AdHandler adHandler;
    // public Button buyButton;
    public OptionsButton options;
    public Button shopButton;

    void Start()
    {
        InitializePurchasing();
        
    }



    void InitializePurchasing()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        //Add products that will be purchasable and indicate its type.
        builder.AddProduct(goldProductId1, ProductType.Consumable);
        builder.AddProduct(goldProductId2, ProductType.Consumable);
        builder.AddProduct(goldProductId3, ProductType.Consumable);
        builder.AddProduct(goldProductId4, ProductType.Consumable);
        builder.AddProduct(removeAdsProductId, ProductType.NonConsumable);

        UnityPurchasing.Initialize(this, builder);
    }

    public void Buy500Gold()
    {
        m_StoreController.InitiatePurchase(goldProductId1);
    }

    public void Buy1000Gold()
    {
        m_StoreController.InitiatePurchase(goldProductId2);
    }

    public void Buy2500Gold()
    {
        m_StoreController.InitiatePurchase(goldProductId3);
    }

    public void Buy5000Gold()
    {
        m_StoreController.InitiatePurchase(goldProductId4);
    }

    public void BuyRemoveAds()
    {
        m_StoreController.InitiatePurchase(removeAdsProductId);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("In-App Purchasing successfully initialized");
        m_StoreController = controller;
        shopButton.interactable = true;

        Product product = m_StoreController.products.WithID(removeAdsProductId);

        // Check if the product is not null
        if (product != null)
        {
            // Log the product information
            Debug.Log("Product ID: " + product.definition.id);
            Debug.Log("Product Title: " + product.metadata.localizedTitle);
            Debug.Log("Product Description: " + product.metadata.localizedDescription);
            Debug.Log("Product Price: " + product.metadata.localizedPriceString);
            Debug.Log("Product Receipt: " + product.receipt);
        }
        else
        {
            Debug.Log("Product not found: " + removeAdsProductId);
        }

        if (product != null && product.hasReceipt)
        {
            // Owned Non Consumables and Subscriptions should always have receipts.
            // So here the Non Consumable product has already been bought.
            DisableAds();
        }
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        OnInitializeFailed(error, null);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        var errorMessage = $"Purchasing failed to initialize. Reason: {error}.";

        if (message != null)
        {
            errorMessage += $" More details: {message}";
        }

        Debug.Log(errorMessage);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        //Retrieve the purchased product
        var product = args.purchasedProduct;

        //Add the purchased product to the players inventory
        if (product.definition.id == goldProductId1)
        {
            Add500Gold();
        }
        else if (product.definition.id == goldProductId2)
        {
            Add1000Gold();
        }
        else if (product.definition.id == goldProductId3)
        {
            Add2500Gold();
        }
        else if (product.definition.id == goldProductId4)
        {
            Add5000Gold();
        }
        else if (product.definition.id == removeAdsProductId)
        {
            DisableAds();
        }

        Debug.Log($"Purchase Complete - Product: {product.definition.id}");

        //We return Complete, informing IAP that the processing on our side is done and the transaction can be closed.
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.Log($"Purchase failed - Product: '{product.definition.id}'," +
            $" Purchase failure reason: {failureDescription.reason}," +
            $" Purchase failure details: {failureDescription.message}");
    }

    public void Add500Gold()
    {
        Debug.Log("adding 500 gold");
        premiumManager.UpdatePremium(500);
        gameController.SaveData();
    }

    public void Add1000Gold()
    {
        premiumManager.UpdatePremium(1000);
        gameController.SaveData();
    }

    public void Add2500Gold()
    {
        premiumManager.UpdatePremium(2500);
        gameController.SaveData();
    }

    public void Add5000Gold()
    {
        premiumManager.UpdatePremium(5000);
        gameController.SaveData();
    }

    public void DisableAds()
    {
        adHandler.noAds = true;
        options.DisableButton();
        // buyButton.interactable = false;
    }

}