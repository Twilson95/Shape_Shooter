using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class IAPManager : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RegisterStoreServiceCallbacksEarly()
    {
        MethodInfo storeServiceFactory = typeof(UnityIAPServices).GetMethod("StoreService", BindingFlags.Public | BindingFlags.Static);
        object storeService = storeServiceFactory?.Invoke(null, null);
        if (storeService == null)
        {
            return;
        }

        RegisterNoOpStoreCallback(storeService, "OnStoreConnected", nameof(NoOpStoreConnected), nameof(NoOpStoreConnectedWithPayload));
        RegisterNoOpStoreCallback(storeService, "OnStoreDisconnected", nameof(NoOpStoreDisconnected), nameof(NoOpStoreDisconnectedWithPayload));
    }

    static void RegisterNoOpStoreCallback(object storeService, string eventName, string noArgMethodName, string payloadMethodName)
    {
        EventInfo callbackEvent = storeService.GetType().GetEvent(eventName, BindingFlags.Instance | BindingFlags.Public);
        Type callbackType = callbackEvent?.EventHandlerType;
        if (callbackType == null)
        {
            return;
        }

        MethodInfo invokeMethod = callbackType.GetMethod("Invoke");
        int parameterCount = invokeMethod?.GetParameters().Length ?? 0;
        MethodInfo callbackMethod = typeof(IAPManager).GetMethod(
            parameterCount == 0 ? noArgMethodName : payloadMethodName,
            BindingFlags.Static | BindingFlags.NonPublic
        );
        if (callbackMethod == null)
        {
            return;
        }

        Delegate callback = Delegate.CreateDelegate(callbackType, callbackMethod);
        callbackEvent.AddEventHandler(storeService, callback);
    }

    static void NoOpStoreConnected() { }
    static void NoOpStoreConnectedWithPayload(object _) { }
    static void NoOpStoreDisconnected() { }
    static void NoOpStoreDisconnectedWithPayload(object _) { }

    StoreController m_StoreController;

    // Your product IDs. They should match the IDs configured in your store dashboards.
    public string goldProductId1 = "shapeshooter_500_gold_coins";
    public string goldProductId2 = "shapeshooter_1000_gold_coins";
    public string goldProductId3 = "shapeshooter_2500_gold_coins";
    public string goldProductId4 = "shapeshooter_5000_gold_coins";
    public string removeAdsProductId = "shapeshooter_remove_ads";

    public PremiumManager premiumManager;
    public GameController gameController;
    public AdHandler adHandler;
    public OptionsButton options;
    public Button shopButton;

    string m_LastRequestedProductId;

    async void Start()
    {
        await InitializePurchasing();
    }

    async Task InitializePurchasing()
    {
        m_StoreController = UnityIAPServices.StoreController();

        m_StoreController.OnPurchasePending += OnPurchasePending;
        m_StoreController.OnPurchaseFailed += OnPurchaseFailed;
        m_StoreController.OnProductsFetched += OnProductsFetched;
        m_StoreController.OnProductsFetchFailed += OnProductsFetchFailed;
        m_StoreController.OnPurchasesFetched += OnPurchasesFetched;
        m_StoreController.OnPurchasesFetchFailed += OnPurchasesFetchFailed;
        m_StoreController.OnStoreConnected += OnStoreConnected;
        m_StoreController.OnStoreDisconnected += OnStoreDisconnected;

        await ConnectStore();

        var initialProductsToFetch = new List<ProductDefinition>
        {
            new(goldProductId1, ProductType.Consumable),
            new(goldProductId2, ProductType.Consumable),
            new(goldProductId3, ProductType.Consumable),
            new(goldProductId4, ProductType.Consumable),
            new(removeAdsProductId, ProductType.NonConsumable)
        };

        m_StoreController.FetchProducts(initialProductsToFetch);
        shopButton.interactable = true;
    }

    async Task ConnectStore()
    {
        MethodInfo connectWithCallbacks = m_StoreController
            .GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(method =>
            {
                if (method.Name != "Connect")
                {
                    return false;
                }

                ParameterInfo[] parameters = method.GetParameters();
                return parameters.Length == 2
                    && typeof(Delegate).IsAssignableFrom(parameters[0].ParameterType)
                    && typeof(Delegate).IsAssignableFrom(parameters[1].ParameterType);
            });

        if (connectWithCallbacks != null)
        {
            ParameterInfo[] callbackParameters = connectWithCallbacks.GetParameters();
            Delegate onConnected = CreateStoreCallbackDelegate(
                callbackParameters[0].ParameterType,
                nameof(HandleStoreConnectedNoArgs),
                nameof(HandleStoreConnectedWithPayload)
            );
            Delegate onDisconnected = CreateStoreCallbackDelegate(
                callbackParameters[1].ParameterType,
                nameof(HandleStoreDisconnectedNoArgs),
                nameof(HandleStoreDisconnectedWithPayload)
            );

            object connectResult = connectWithCallbacks.Invoke(m_StoreController, new object[] { onConnected, onDisconnected });
            if (connectResult is Task connectTask)
            {
                await connectTask;
            }
            return;
        }

        await m_StoreController.Connect();
    }

    Delegate CreateStoreCallbackDelegate(Type delegateType, string noArgMethodName, string payloadMethodName)
    {
        MethodInfo invokeMethod = delegateType.GetMethod("Invoke");
        ParameterInfo[] parameters = invokeMethod?.GetParameters();

        if (parameters == null || parameters.Length == 0)
        {
            return Delegate.CreateDelegate(
                delegateType,
                this,
                GetType().GetMethod(noArgMethodName, BindingFlags.Instance | BindingFlags.NonPublic)
            );
        }

        return Delegate.CreateDelegate(
            delegateType,
            this,
            GetType().GetMethod(payloadMethodName, BindingFlags.Instance | BindingFlags.NonPublic)
        );
    }

    void HandleStoreConnectedNoArgs()
    {
        OnStoreConnected();
    }

    void HandleStoreConnectedWithPayload(object _)
    {
        OnStoreConnected();
    }

    void HandleStoreDisconnectedNoArgs()
    {
        Debug.Log("Store disconnected.");
    }

    void HandleStoreDisconnectedWithPayload(object payload)
    {
        if (payload is StoreConnectionFailureDescription failure)
        {
            OnStoreDisconnected(failure);
            return;
        }

        Debug.Log($"Store disconnected. Reason: {payload}");
    }

    public void Buy500Gold()
    {
        PurchaseProduct(goldProductId1);
    }

    public void Buy1000Gold()
    {
        PurchaseProduct(goldProductId2);
    }

    public void Buy2500Gold()
    {
        PurchaseProduct(goldProductId3);
    }

    public void Buy5000Gold()
    {
        PurchaseProduct(goldProductId4);
    }

    public void BuyRemoveAds()
    {
        PurchaseProduct(removeAdsProductId);
    }

    void PurchaseProduct(string productId)
    {
        m_LastRequestedProductId = productId;
        m_StoreController?.PurchaseProduct(productId);
    }

    void OnProductsFetched(List<Product> products)
    {
        Debug.Log($"IAP products fetched: {products.Count}");
        m_StoreController.FetchPurchases();
    }

    void OnProductsFetchFailed(ProductFetchFailed failure)
    {
        Debug.Log($"Failed to fetch products. Reason: {failure.FailureReason}");
    }

    void OnPurchasesFetched(Orders orders)
    {
        Debug.Log("Purchases fetched.");

        if (ContainsProductInOrderCollection(orders, removeAdsProductId))
        {
            DisableAds();
        }
    }

    void OnPurchasesFetchFailed(PurchasesFetchFailureDescription failure)
    {
        Debug.Log($"Failed to fetch purchases. Reason: {failure.FailureReason}");
    }

    void OnStoreDisconnected(StoreConnectionFailureDescription failure)
    {
        Debug.Log($"Store disconnected. Reason: {failure.Message}");
    }

    void OnStoreConnected()
    {
        Debug.Log("Store connected.");
    }

    void OnPurchasePending(PendingOrder pendingOrder)
    {
        string purchasedProductId = TryExtractProductIdFromOrder(pendingOrder) ?? m_LastRequestedProductId;

        if (purchasedProductId == goldProductId1)
        {
            Add500Gold();
        }
        else if (purchasedProductId == goldProductId2)
        {
            Add1000Gold();
        }
        else if (purchasedProductId == goldProductId3)
        {
            Add2500Gold();
        }
        else if (purchasedProductId == goldProductId4)
        {
            Add5000Gold();
        }
        else if (purchasedProductId == removeAdsProductId)
        {
            DisableAds();
        }

        Debug.Log($"Purchase Complete - Product: {purchasedProductId}");

        m_StoreController.ConfirmPurchase(pendingOrder);
        m_LastRequestedProductId = null;
    }

    void OnPurchaseFailed(FailedOrder failedOrder)
    {
        Debug.Log($"Purchase failed. Reason: {failedOrder.FailureReason}");
        m_LastRequestedProductId = null;
    }

    string TryExtractProductIdFromOrder(object order)
    {
        if (order == null)
        {
            return null;
        }

        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;

        object info = order.GetType().GetProperty("Info", flags)?.GetValue(order);
        object cartOrdered = info?.GetType().GetProperty("CartOrdered", flags)?.GetValue(info);
        IEnumerable<object> items = cartOrdered?.GetType().GetMethod("Items", flags)?.Invoke(cartOrdered, null) as IEnumerable<object>;

        if (items == null)
        {
            return null;
        }

        foreach (object item in items)
        {
            object product = item?.GetType().GetProperty("Product", flags)?.GetValue(item);
            object definition = product?.GetType().GetProperty("definition", flags)?.GetValue(product)
                ?? product?.GetType().GetProperty("Definition", flags)?.GetValue(product);

            string productId = definition?.GetType().GetProperty("id", flags)?.GetValue(definition) as string
                ?? definition?.GetType().GetProperty("Id", flags)?.GetValue(definition) as string;

            if (!string.IsNullOrEmpty(productId))
            {
                return productId;
            }
        }

        return null;
    }

    bool ContainsProductInOrderCollection(object orders, string productId)
    {
        if (orders == null || string.IsNullOrEmpty(productId))
        {
            return false;
        }

        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
        IEnumerable<object> confirmedOrders = orders.GetType().GetMethod("ConfirmedOrders", flags)?.Invoke(orders, null) as IEnumerable<object>;

        if (confirmedOrders == null)
        {
            return false;
        }

        foreach (object confirmedOrder in confirmedOrders)
        {
            if (TryExtractProductIdFromOrder(confirmedOrder) == productId)
            {
                return true;
            }
        }

        return false;
    }

    public void Add500Gold()
    {
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
    }

    void OnDestroy()
    {
        if (m_StoreController == null)
        {
            return;
        }

        m_StoreController.OnPurchasePending -= OnPurchasePending;
        m_StoreController.OnPurchaseFailed -= OnPurchaseFailed;
        m_StoreController.OnProductsFetched -= OnProductsFetched;
        m_StoreController.OnProductsFetchFailed -= OnProductsFetchFailed;
        m_StoreController.OnPurchasesFetched -= OnPurchasesFetched;
        m_StoreController.OnPurchasesFetchFailed -= OnPurchasesFetchFailed;
        m_StoreController.OnStoreConnected -= OnStoreConnected;
        m_StoreController.OnStoreDisconnected -= OnStoreDisconnected;
    }
}
