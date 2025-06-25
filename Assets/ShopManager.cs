using UnityEngine;
using TMPro;
using Oculus.Platform;
using Oculus.Platform.Models;
using System.Collections;

public class ShopManager : MonoBehaviour
{
    public TextMeshProUGUI availableItems;
    public TextMeshProUGUI purchasedItems;

    public GameObject waterGun;

    public string[] skus = new[] { "waterconsumable", "watergun" };

    IEnumerator Start()
    {
        waterGun.SetActive(false);

        while(!Core.IsInitialized())
        {
            yield return null;
        }

        GetPrices();
        GetPurchases();
    }

    private void GetPrices()
    {
        IAP.GetProductsBySKU(skus).OnComplete(GetProductsCompleted);
    }

    private void GetProductsCompleted(Message<ProductList> msg)
    {
        if (msg.IsError) 
        {
            availableItems.text = "Fail" + msg.GetError().ToString();
            return;
        }
        else
        {
            availableItems.text = "Items : \n";
        }

        foreach (var prod in msg.GetProductList())
        {
            availableItems.text += prod.Name + "-" + prod.FormattedPrice + "\n";
        }
    }

    private void GetPurchases()
    {
        IAP.GetViewerPurchases().OnComplete(GetPurchasesCompleted);
    }

    private void GetPurchasesCompleted(Message<PurchaseList> msg)
    {
        if (msg.IsError)
        {
            purchasedItems.text = "Fail \n" + msg.GetError().ToString();
            return;
        }
        else
        {
            purchasedItems.text = "Purchased : \n";
        }

        foreach (var purch in msg.GetPurchaseList())
        {
            if(purch.Sku == "watergun")
            {
                waterGun.SetActive(true);
            }

            purchasedItems.text += purch.Sku + "-" + purch.GrantTime + "\n";
        }
    }

    public void BuyWaterGun()
    {
        IAP.LaunchCheckoutFlow("waterconsumable").OnComplete(BuyCubeCallback);
    }

    private void BuyCubeCallback(Message<Purchase> msg)
    {
        if (msg.IsError) 
            return;

        GetPurchases();
    }
}
