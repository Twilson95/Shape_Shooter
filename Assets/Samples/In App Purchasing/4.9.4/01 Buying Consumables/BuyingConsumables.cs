using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace Samples.Purchasing.Core.BuyingConsumables
{
    public class BuyingConsumables : MonoBehaviour
    {
        StoreController m_StoreController;

        // Your products IDs. They should match the ids of your products in your store.
        public string goldProductId = "com.mycompany.mygame.gold1";
        public string diamondProductId = "com.mycompany.mygame.diamond1";

        public Text GoldCountText;
        public Text DiamondCountText;

        int m_GoldCount;
        int m_DiamondCount;
        string m_LastRequestedProductId;

        async void Start()
        {
            await InitializePurchasing();
            UpdateUI();
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
            m_StoreController.OnStoreDisconnected += OnStoreDisconnected;

            await m_StoreController.Connect();

            var initialProductsToFetch = new List<ProductDefinition>
            {
                new(goldProductId, ProductType.Consumable),
                new(diamondProductId, ProductType.Consumable)
            };

            m_StoreController.FetchProducts(initialProductsToFetch);
        }

        public void BuyGold()
        {
            m_LastRequestedProductId = goldProductId;
            m_StoreController?.PurchaseProduct(goldProductId);
        }

        public void BuyDiamond()
        {
            m_LastRequestedProductId = diamondProductId;
            m_StoreController?.PurchaseProduct(diamondProductId);
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
        }

        void OnPurchasesFetchFailed(PurchasesFetchFailureDescription failure)
        {
            Debug.Log($"Failed to fetch purchases. Reason: {failure.FailureReason}");
        }

        void OnStoreDisconnected(StoreConnectionFailureDescription failure)
        {
            Debug.Log($"Store disconnected. Reason: {failure.FailureReason}");
        }

        void OnPurchasePending(PendingOrder pendingOrder)
        {
            if (m_LastRequestedProductId == goldProductId)
            {
                AddGold();
            }
            else if (m_LastRequestedProductId == diamondProductId)
            {
                AddDiamond();
            }

            Debug.Log($"Purchase Complete - Product: {m_LastRequestedProductId}");

            m_StoreController.ConfirmPurchase(pendingOrder);
            m_LastRequestedProductId = null;
        }

        void OnPurchaseFailed(FailedOrder failedOrder)
        {
            Debug.Log($"Purchase failed. Reason: {failedOrder.FailureReason}");
            m_LastRequestedProductId = null;
        }

        void AddGold()
        {
            m_GoldCount++;
            UpdateUI();
        }

        void AddDiamond()
        {
            m_DiamondCount++;
            UpdateUI();
        }

        void UpdateUI()
        {
            GoldCountText.text = $"Your Gold: {m_GoldCount}";
            DiamondCountText.text = $"Your Diamonds: {m_DiamondCount}";
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
            m_StoreController.OnStoreDisconnected -= OnStoreDisconnected;
        }
    }
}
