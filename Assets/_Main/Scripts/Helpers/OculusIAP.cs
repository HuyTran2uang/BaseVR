using Oculus.Platform;
using Oculus.Platform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sku
{
    public string sku;
    public string price;
    public Sku(string sku, string price)
    {
        this.sku = sku;
        this.price = price;
    }
    public void Buy()
    {
        switch (sku)
        {
            case "price_2":
                //GameManager.Instance.Turns += 2;
                break;
            case "price_5":
                //GameManager.Instance.Turns += 5;
                break;
            case "price_10":
                //GameManager.Instance.Turns += 10;
                break;
            case "price_20":
                //GameManager.Instance.Turns += 20;
                break;
            case "price_50":
                //GameManager.Instance.Turns += 50;
                break;
            case "price_100":
                //GameManager.Instance.Turns += 100;
                break;
            case "price_200":
                //GameManager.Instance.Turns += 200;
                break;
            default:
                break;
        }
    }
}

public class OculusIAP : MonoBehaviour
{
    private static OculusIAP _instance;
    public static OculusIAP Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<OculusIAP>();
            }
            if (_instance == null)
            {
                _instance = new GameObject("OculusIAP").AddComponent<OculusIAP>();
            }
            return _instance;
        }
    }

    string[] skus;
    List<Sku> skusList;

    // Dictionary to keep track of SKUs being consumed
    private Dictionary<UInt64, string> skuDictionary = new Dictionary<UInt64, string>();
    void Start()
    {
        skusList = new List<Sku>();

        skusList.Add(new Sku("price_2", "1.9"));
        skusList.Add(new Sku("price_5", "4.9"));
        skusList.Add(new Sku("price_10", "9.9"));
        skusList.Add(new Sku("price_20", "19.9"));
        skusList.Add(new Sku("price_50", "49.9"));
        skusList.Add(new Sku("price_100", "99.9"));
        skusList.Add(new Sku("price_200", "199.9"));

        skus = new string[skusList.Count];
        for (int i = 0; i < skusList.Count; i++)
        {
            skus[i] = skusList[i].price;
        }

        ///Đầu tiên gọi hàm này
        Core.AsyncInitialize().OnComplete(InitCallback);
        //GetPrices();
        //GetPurchases();
    }

    public bool AllocateCoins(string skuName)
    {
        var dict = skusList.ToDictionary(key => key.sku, value => value);
        if (dict.TryGetValue(skuName, out var sku))
        {
            sku.Buy();
            return true;
        }
        return false;
    }

    public Sku GetSku(string sku)
    {
        if (skusList.Exists(x => x.sku == sku))
            return skusList.Find(x => x.sku == sku);

        return null;
    }

    /// <summary>
    /// khởi tạo core flatfom xong
    /// </summary>
    /// <param name="msg"></param>
    private void InitCallback(Message<Oculus.Platform.Models.PlatformInitialize> msg)
    {
        if (msg.IsError)
        {
            Debug.LogError("Error initializing Oculus Platform: " + msg.GetError().Message);
            // Consider retrying initialization or disabling IAP functionality
        }
        else
        {
            Debug.Log("Oculus Platform initialized successfully.");
            Entitlements.IsUserEntitledToApplication().OnComplete(EntitlementCheckCallback);
        }
    }
    private void EntitlementCheckCallback(Message msg)
    {
        if (msg.IsError)
        {
            Debug.LogError("User not entitled to application, cannot proceed.");
            // Application.Quit();
        }
        else
        {
            Debug.Log("User is entitled.");
            GetPrices();
            GetPurchases();
        }
    }
    private void GetPrices()
    {
        IAP.GetProductsBySKU(skus).OnComplete(GetPricesCallback);
    }

    private void GetPricesCallback(Message<ProductList> msg)
    {
        if (msg.IsError) return;
        foreach (var prod in msg.GetProductList())
        {
            //availableItems.text += $"{prod.Name} - {prod.FormattedPrice} \n";
        }
    }
    private void GetPurchases()
    {
        IAP.GetViewerPurchases().OnComplete(GetPurchasesCallback);
    }
    private void GetPurchasesCallback(Message<PurchaseList> msg)
    {
        if (msg.IsError) return;
        foreach (var purch in msg.GetPurchaseList())
        {
            // purchasedItems.text += $"{purch.Sku}-{purch.GrantTime} \n";
            //  AllocateCoins(purch.Sku);
            ConsumePurchase(purch.Sku);
        }
        CoinPurchaseDeductionCheck();
    }
    private void ConsumePurchase(string skuName)
    {
        //cosume without adding coins 
        //IAP.ConsumePurchase(skuName).OnComplete(ConsumePurchaseCallback);
        var request = IAP.ConsumePurchase(skuName);
        skuDictionary[request.RequestID] = skuName;
        request.OnComplete(ConsumePurchaseCallback);
    }

    private void ConsumePurchaseCallback(Message msg)
    {
        if (msg.IsError)
        {
            Debug.LogError("Error consuming purchase: " + msg.GetError().Message);
        }
        else
        {
            if (skuDictionary.TryGetValue(msg.RequestID, out var sku))
            {
                Debug.Log($"Purchase consumed successfully for SKU: {sku}");
                AllocateCoins(sku); // Call AllocateCoins for each consumable purchase
                skuDictionary.Remove(msg.RequestID);
            }
            else
            {
                Debug.Log("Purchase consumed successfully, but SKU not found in dictionary.");
            }
        }
    }

    public void CoinPurchaseDeductionCheck()
    {
        string data = PlayerPrefs.GetString("PurchasedItem", "0");
        if (!string.IsNullOrEmpty(data))
        {
            string[] items = data.Split(new string[] { "@@" }, StringSplitOptions.None);

            foreach (string item in items)
            {
                int value;
                if (int.TryParse(item, out value))
                {
                    Debug.Log("Retrieved value: " + value);


                    //CoinsCollected = CoinsCollected - airCraftPrice[value];
                }
                else
                {
                    Debug.LogError("Failed to parse item to integer: " + item);
                }
            }
        }
        else
        {
            Debug.Log("No data found in PlayerPrefs for 'PurchasedItem'.");
        }
    }

    public void Buy(string skuName)
    {
#if UNITY_EDITOR
        AllocateCoins(skuName);
#else
        IAP.LaunchCheckoutFlow(skuName).OnComplete(BuyCallBack);
#endif
    }
    private void BuyCallBack(Message<Purchase> msg)
    {
        if (msg.IsError) return;

        //purchasedItems.text = string.Empty;
        GetPurchases();
    }
}